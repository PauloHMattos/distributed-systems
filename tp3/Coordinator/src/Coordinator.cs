using System;
using TP3.Networking;
using TP3.Common;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace TP3.Coordinator
{
    public class Coordinator
    {
        private readonly int _maxProcesses;
        private readonly Peer _peer;
        private readonly TextWriter _logger;
        private readonly MessageHandlerCollection _messageHandlerCollection;
        private readonly ConcurrentQueue<Connection> _queue;
        private readonly Thread _peerUpdateThread;
        public Connection? CurrentProcessWithLock { get; private set; }

        private readonly ConcurrentDictionary<Connection, int> _executionsCounter;

        public Coordinator(int maxProcesses)
        {
            _maxProcesses = maxProcesses;
            var streamWriter = File.CreateText("log.txt");
            streamWriter.AutoFlush = true;
            _logger = streamWriter;
            _peer = Peer.CreateServerPeer(_logger, 512, 512,  _maxProcesses, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _executionsCounter = new ConcurrentDictionary<Connection, int>();
            _queue = new ConcurrentQueue<Connection>();
            _messageHandlerCollection = new MessageHandlerCollection(_logger);

            _messageHandlerCollection.AddHandler(new RequestMessageHandler(this));
            _messageHandlerCollection.AddHandler(new ReleaseMessageHandler(this));

            _peerUpdateThread = new Thread(UpdatePeer);
        }

        private void OnMessageReceived(ReadOnlySpan<byte> message, Connection connection)
        {
            _messageHandlerCollection.Handle(connection, message);
        }

        private void OnDisconnected(Connection connection)
        {
            if (connection == CurrentProcessWithLock)
            {
                CurrentProcessWithLock = null;
            }
        }

        private void OnConnected(Connection connection)
        {
            _executionsCounter.TryAdd(connection, 0);
            _logger.WriteLine($"[S] {MessageType.SetId} - {connection.Index} - {DateTime.Now.ToString("hh:mm:ss.fff tt")}");
            var message = Message.Build(MessageType.SetId, connection.Index);
            connection.SendMessage(message.Data);
        }

        public void Start()
        {
            _peer.Listen(27000);
            _peerUpdateThread.Start(this);
        }

        public void Update()
        {
            Console.Write ("> ");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("|");
                    foreach (var connection in _queue)
                    {
                        Console.Write($"{connection.Index} -> ");
                    }
                    Console.WriteLine($"end");
                    break;

                case "2":
                    foreach (var kvp in _executionsCounter)
                    {
                        Console.WriteLine($"| {kvp.Key.Index} = {kvp.Value}");
                    }
                    break;

                case "3":
                    Console.WriteLine("Finishing the execution...");
                    Environment.Exit(0);
                    break;
            }
        }

        public static void UpdatePeer(object? state)
        {
            var coordinator = ((Coordinator)state!);
            var peer = coordinator._peer;
            while(true)
            {
                peer.Update();
                
                if (coordinator.CurrentProcessWithLock is null)
                {
                    coordinator.Grant();
                }

                // Sleep a bit to keep the CPU cool
                Thread.Sleep(50);
            }
        }

        public void Grant()
        {
            while (_queue.TryDequeue(out var current))
            {
                if (!current.Active)
                {
                    continue;
                }

                CurrentProcessWithLock = current;
                _logger.WriteLine($"[S] {MessageType.Grant} - {current.Index} - {DateTime.Now.ToString("hh:mm:ss.fff tt")}");

                var grantMessage = Message.Build(MessageType.Grant);
                current.SendMessage(grantMessage.Data);

                if (_executionsCounter.TryGetValue(current, out var count))
                {
                    _executionsCounter[current] = ++count;
                }
                else
                {
                    Debug.Fail("Connection not found in dictionary");
                }
                break;
            }
        }

        public void Enqueue(Connection connection)
        {
            _queue.Enqueue(connection);
        }

        public bool Release(Connection connection)
        {
            if (connection != CurrentProcessWithLock)
            {
                Console.WriteLine($"[Release] Invalid Release. {connection.Index} != {CurrentProcessWithLock?.Index}");
                return false;
            }
            CurrentProcessWithLock = null;
            return true;
        }
    }
}
