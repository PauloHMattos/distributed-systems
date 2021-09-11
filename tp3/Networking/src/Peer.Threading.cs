using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;

namespace TP3.Networking
{
    public partial class Peer
    {
        private class PeerThreading
        {
            private readonly Thread _workerThread;
            private readonly Socket _socket;
            private readonly byte[] _recvBuffer;

            public Channel<Packet> OutputChannel { get; }
            public Channel<Packet> InputChannel { get; }

            public PeerThreading(Socket socket, int inputBufferLength)
            {
                _workerThread = new Thread(PeerWorkerThread);
                _socket = socket;
                _recvBuffer = new byte[inputBufferLength];

                OutputChannel = Channel.CreateUnbounded<Packet>();
                InputChannel = Channel.CreateUnbounded<Packet>();
            }

            public void Start()
            {
                _workerThread.Start();
            }

            private void PeerWorkerThread()
            {
                try
                {
                    while (true)
                    {
                        FlushSendChannel();
                        Receive();
                        Thread.Yield();
                    }
                }
                catch (ThreadAbortException)
                {
                    InputChannel.Writer.Complete();
                }
            }

            private void Receive()
            {
                while (_socket.Poll(0, SelectMode.SelectRead))
                {
                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    var packetLength = _socket.ReceiveFrom(_recvBuffer, _recvBuffer.Length, SocketFlags.None, ref remoteEndPoint);
                    if (packetLength == 0)
                    {
                        return;
                    }
                    
                    var payload = new byte[packetLength];
                    _recvBuffer.AsSpan(0, packetLength).CopyTo(payload);

                    var packet = new Packet()
                    {
                        Payload = payload,
                        RemoteEndPoint = (IPEndPoint)remoteEndPoint
                    };
                    Debug.Assert(InputChannel.Writer.TryWrite(packet));
                }
            }

            private void FlushSendChannel()
            {
                while (OutputChannel.Reader.TryRead(out var packet))
                {
                    var payload = packet.Payload;
                    _socket.SendTo(payload, 0, payload.Length, SocketFlags.None, packet.RemoteEndPoint);
                }
            }
        }
    }
}