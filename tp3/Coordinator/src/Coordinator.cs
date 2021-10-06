using System;
using TP3.Networking;
using TP3.Common;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

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
        private readonly Thread _userInterfaceThread;
        public Connection? CurrentProcessWithLock { get; private set; }

        public Coordinator(int maxProcesses)
        {

            _maxProcesses = maxProcesses;
            _logger = File.CreateText("log.txt");
            _peer = Peer.CreateServerPeer(_logger, 512, 512,  _maxProcesses, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _queue = new ConcurrentQueue<Connection>();
            _messageHandlerCollection = new MessageHandlerCollection();

            _messageHandlerCollection.AddHandler(new RequestMessageHandler(this));
            _messageHandlerCollection.AddHandler(new ReleaseMessageHandler(this));

            _peerUpdateThread = new Thread(UpdatePeer);
            _userInterfaceThread = new Thread(StartUserInterface);
        }

        private void OnMessageReceived(ReadOnlySpan<byte> message, Connection connection)
        {
            _messageHandlerCollection.Handle(connection, message);
        }

        private void OnDisconnected(Connection connection)
        {
            if (connection == CurrentProcessWithLock)
            {
                Grant();
            }
        }

        private void OnConnected(Connection connection)
        {
            var message = Message.Build(MessageType.SetId, connection.Index);
            Console.WriteLine(message.ToString());
            connection.SendMessage(message.Data);
        }

        public void Start()
        {
            _peer.Listen(27000);
            _peerUpdateThread.Start(_peer);
            _userInterfaceThread.Start(_queue);
        }

        public void Update()
        {
        }

        public static void UpdatePeer(object? peer)
        {
            while(true)
            {
                ((Peer)peer!).Update();
                // Sleep a bit to keep the CPU cool
                Thread.Sleep(50);
            }
        }

        public static void StartUserInterface(object? queue)
        {
            Console.Write("Waiting for one of the following commands:\n\n");
            Console.Write("\t1: Prints the current requests queue.\n");
            Console.Write("\t2: Prints how many times each process has been served.\n");
            Console.Write("\t3: Finishes the execution.\n");
            while (true) {
                switch (Console.ReadLine())
                {
                    case "1":
                        foreach (var connection in (ConcurrentQueue<Connection>)queue!) {
                            Console.WriteLine(connection.Index);
                        }
                        break;
                    case "2":
                        // TODO
                        break;
                    case "3":
                        Console.WriteLine("Finishing the execution...");
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public void Grant()
        {
            CurrentProcessWithLock = null;

            while (_queue.TryDequeue(out var current))
            {
                if (!current.Active)
                {
                    continue;
                }

                CurrentProcessWithLock = current;
                var grantMessage = Message.Build(MessageType.Grant);
                current.SendMessage(grantMessage.Data);
            }
        }

        public void Enqueue(Connection connection)
        {
            _queue.Enqueue(connection);
            if (CurrentProcessWithLock is null)
            {
                Grant();
            }
        }

        public bool Release(Connection connection)
        {
            if (connection != CurrentProcessWithLock)
            {
                Console.WriteLine($"[Release] Invalid Release. {connection.Index} != {CurrentProcessWithLock?.Index}");
                return false;
            }
            return true;
        }
    }
}
