using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TP3.Networking
{
    public partial class Peer
    {
        private readonly Socket _socket;
        private readonly Dictionary<IPEndPoint, Connection> _connections;
        private readonly TextWriter _logger;
        private readonly int _maxConnections;
        private readonly Stopwatch _watch;
        private readonly long _timeoutTime;
        private readonly PeerThreading _peerThreading;

        private byte _nextConnectionId;
        private Action<Connection>? _onConnected;
        private Action<Connection>? _onDisconnected;
        private ReadOnlySpanAction<byte, Connection>? _onMessageReceived;
        private IPEndPoint? _boundEndPoint;

        private Peer(TextWriter logger,
                     int inputBufferLength,
                     int outputBufferLength,
                     int maxConnections,
                     int timeoutTime)
        {
            _logger = logger;
            _watch = new Stopwatch();
            _connections = new Dictionary<IPEndPoint, Connection>();
            _maxConnections = maxConnections;
            _timeoutTime = timeoutTime;
            _socket = new Socket(AddressFamily.InterNetwork,
                                 SocketType.Dgram,
                                 ProtocolType.Udp)
                                 {
                                     Blocking = false
                                 };

            SetConnReset(_socket);
            _peerThreading = new PeerThreading(_socket, inputBufferLength);
    
            _watch.Start();
        }

        private void SetConnReset(Socket socket)
        {
            try
            {
                const uint IOC_IN = 0x80000000;
                const uint IOC_VENDOR = 0x18000000;
                socket.IOControl(unchecked((int)(IOC_IN | IOC_VENDOR | 12)), new byte[] { System.Convert.ToByte(false) }, null);
            }
            catch
            {
            }
        }

        public static Peer CreateClientPeer(TextWriter logger,
                                            int inputBufferLength,
                                            int outputBufferLength,
                                            int timeoutTime)
        {
            return new Peer(logger, inputBufferLength, outputBufferLength, 1, timeoutTime);
        }

        public static Peer CreateServerPeer(TextWriter logger,
                                            int inputBufferLength,
                                            int outputBufferLength,
                                            int maxConnections,
                                            int timeoutTime)
        {
            return new Peer(logger, inputBufferLength, outputBufferLength, maxConnections, timeoutTime);
        }

        public void AttachCallbacks(Action<Connection>? onConnected,
                                 Action<Connection>? onDisconnected,
                                 ReadOnlySpanAction<byte, Connection>? onMessageReceived)
        {
            _onConnected = onConnected;
            _onDisconnected = onDisconnected; 
            _onMessageReceived = onMessageReceived;
        }

        public void Listen(int port)
        {
            Bind(IPAddress.Loopback, port);

            _peerThreading.Start();
        }

        public void Connect(IPAddress address, int port)
        {
            Bind(IPAddress.Any, 0);

            var endPoint = new IPEndPoint(address, port);
            _logger.WriteLine($"{_boundEndPoint} Peer connecting to {endPoint}");

            var connection = new Connection(endPoint, _nextConnectionId++, _watch.ElapsedMilliseconds);
            _connections.Add(connection.EndPoint, connection);

            // Implement connection retry?
            SendUnconnected(endPoint, new byte[2] { (byte)PacketType.Command, (byte)CommandId.ConnectionRequested });
            _peerThreading.Start();
        }

        public void Disconnect(Connection connection)
        {
            _logger.WriteLine($"{connection.EndPoint} disconnected");
            _connections.Remove(connection.EndPoint);
            _onDisconnected?.Invoke(connection);
        }

        public void SendMessage(Connection connection, ReadOnlySpan<byte> message)
        {
            var payload = new byte[message.Length + 1];
            payload[0] = (byte)PacketType.ApplicationMessage;
            message.CopyTo(payload.AsSpan(0));
            SendTo(connection, payload);
        }

        public void Update()
        {
            while (_peerThreading.InputChannel.Reader.TryRead(out var packet))
            {
                var packetType = (PacketType)packet.Payload[0];

                if (!_connections.TryGetValue(packet.RemoteEndPoint, out var connection))
                {
                    if (packetType == PacketType.Command)
                    {
                        HandleUnconnectedCommand(packet.RemoteEndPoint, packet.Payload.AsSpan(1));
                    }
                    continue;
                }

                connection.LastReceivedTime = _watch.ElapsedMilliseconds;

                switch (packetType)
                {
                    case PacketType.KeepAlive:
                        // Don't do nothing 
                        // _logger.WriteLine("Received keep alive");
                        break;

                    case PacketType.Command:
                        HandleCommand(connection, packet.Payload.AsSpan(1));
                        break;
                        
                    case PacketType.ApplicationMessage:
                        HandleMessage(connection, packet.Payload.AsSpan(1));
                        break;
                }
            }

            var currentMilliseconds = _watch.ElapsedMilliseconds;
            foreach(var (endPoint, connection) in _connections)
            {
                if (currentMilliseconds - connection.LastReceivedTime > _timeoutTime)
                {
                    // Disconnect timed out connections
                    Disconnect(connection);
                }
                else if (currentMilliseconds - connection.LastSendTime > _timeoutTime / 4)
                {
                    // Send a simple message to avoid timeout
                    SendKeepAlive(connection);
                }
            }
        }

        private void Bind(IPAddress address, int port)
        {
            _boundEndPoint = new IPEndPoint(address, port);
            _socket.Bind(_boundEndPoint);
            _logger.WriteLine($"Bound to {_boundEndPoint}");
        }

        private void HandleUnconnectedCommand(IPEndPoint remoteEndPoint, Span<byte> command)
        {
            var commandId = (CommandId)command[0];
            switch (commandId)
            {
                case CommandId.ConnectionRequested:
                    HandleConnectionRequest(remoteEndPoint);
                    break;

                default:
                    throw new InvalidOperationException($"Unconnected remotes can send only connection request packets: {commandId}");
            }
        }

        private void HandleCommand(Connection connection, Span<byte> command)
        {
            var commandId = (CommandId)command[0];
            switch (commandId)
            {
                case CommandId.ConnectionRefused:
                    Debug.Assert(_connections.Remove(connection.EndPoint));
                    break;
                    
                case CommandId.ConnectionAccepted:
                    _onConnected?.Invoke(connection);
                    break;

                default:
                    throw new InvalidOperationException($"Unknow command {commandId}");
            }
        }

        private void HandleConnectionRequest(IPEndPoint remoteEndPoint)
        {
            // If the server is full, rejects the connection
            if (_connections.Count >= _maxConnections)
            {
                SendUnconnected(remoteEndPoint, new byte[2] { (byte)PacketType.Command, (byte)CommandId.ConnectionRefused });
                return;
            }

            // Creates connection and send tho the client it's index
            var connection = new Connection(remoteEndPoint, _nextConnectionId++, _watch.ElapsedMilliseconds);
            _connections.Add(connection.EndPoint, connection);

            SendTo(connection, new byte[2] { (byte)PacketType.Command, (byte)CommandId.ConnectionAccepted });
        }

        private void HandleMessage(Connection connection, Span<byte> message)
        {
            _onMessageReceived?.Invoke(message, connection);
        }

        private void SendUnconnected(IPEndPoint destination, byte[] packet)
        {
            ScheduleSend(destination, packet);
        }

        private void SendKeepAlive(Connection connection)
        {
            // _logger.WriteLine("SendKeepAlive");
            SendTo(connection, new byte[1] { (byte)PacketType.KeepAlive });
        }


        private void SendTo(Connection connection, byte[] payload)
        {
            connection.LastSendTime = _watch.ElapsedMilliseconds;
            ScheduleSend(connection.EndPoint, payload);
        }

        private void ScheduleSend(IPEndPoint destination, byte[] payload)
        {
            Debug.Assert(_peerThreading.OutputChannel.Writer.TryWrite(new Packet()
            {
                Payload = payload,
                RemoteEndPoint = destination
            }));
        }
    }
}