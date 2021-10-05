using System;
using TP3.Networking;
using TP3.Common;
using System.Collections.Concurrent;
using System.Threading;

namespace TP3.Coordinator
{
    public class Coordinator
    {
        private readonly int _maxProcesses;
        private readonly Peer _peer;
        private readonly MessageHandlerCollection _messageHandlerCollection;
        private readonly ConcurrentQueue<Connection> _queue;
        private readonly Thread _peerUpdateThread;
        public Connection? CurrentProcessWithLock { get; private set; }

        public Coordinator(int maxProcesses)
        {
            _maxProcesses = maxProcesses;
            _peer = Peer.CreateServerPeer(Console.Out, 512, 512,  _maxProcesses, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _queue = new ConcurrentQueue<Connection>();
            _messageHandlerCollection = new MessageHandlerCollection();

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
