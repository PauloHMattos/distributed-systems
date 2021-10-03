using System;
using System.Diagnostics;
using System.Text;
using System.Net;
using TP3.Networking;
using TP3.Common;
using System.Collections.Concurrent;

namespace TP3.Coordinator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 128 is the max on TP3.pdf
            var coordinator = new Coordinator(128);
            coordinator.Start();

            while(true)
            {
                coordinator.Update();
            }
        }
    }

    internal class RequestMessageHandler : MessageHandler
    {
        public override MessageType MessageId => MessageType.Request;

        private readonly ConcurrentQueue<Connection> _queue;

        public RequestMessageHandler(ConcurrentQueue<Connection> queue)
        {
            _queue = queue;
        }

        public override void Handle(Connection connection, int? id)
        {
            _queue.Enqueue(connection);
        }
    }

    internal class ReleaseMessageHandler : MessageHandler
    {
        public override MessageType MessageId => MessageType.Request;

        private readonly ConcurrentQueue<Connection> _queue;

        public ReleaseMessageHandler(ConcurrentQueue<Connection> queue)
        {
            _queue = queue;
        }

        public override void Handle(Connection connection, int? id)
        {
            if (!_queue.TryPeek(out var current))
            {
                Console.WriteLine("[ReleaseMessageHandler] Invalid Release. Empty Queue");
                connection.Disconnect();
                return;
            }

            if (current != connection)
            {
                Console.WriteLine($"[ReleaseMessageHandler] Invalid Release. {current.Index} != {connection.Index}");
                connection.Disconnect();
                return;
            }

            Debug.Assert(_queue.TryDequeue(out current));
            Debug.Assert(current == connection);
            
            if (!_queue.TryPeek(out current))
            {
                return;
            }

            var grantMessage = Message.Build(MessageType.Grant);
            current.SendMessage(grantMessage.Data);
        }
    }

    public class Coordinator
    {
        private readonly int _maxProcesses;
        private readonly Peer _peer;
        private readonly MessageHandlerCollection _messageHandlerCollection;
        private readonly ConcurrentQueue<Connection> _queue;

        public Coordinator(int maxProcesses)
        {
            _maxProcesses = maxProcesses;
            _peer = Peer.CreateServerPeer(Console.Out, 512, 512,  _maxProcesses, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _queue = new ConcurrentQueue<Connection>();
            _messageHandlerCollection = new MessageHandlerCollection();

            _messageHandlerCollection.AddHandler(new RequestMessageHandler(_queue));
            _messageHandlerCollection.AddHandler(new ReleaseMessageHandler(_queue));
        }

        private void OnMessageReceived(ReadOnlySpan<byte> message, Connection connection)
        {
            _messageHandlerCollection.Handle(connection, message);
        }

        private void OnDisconnected(Connection connection)
        {
        }

        private void OnConnected(Connection connection)
        {
            Console.WriteLine($"Connected! {connection.EndPoint}");
            
            var message = Message.Build(MessageType.SetId);
            Console.WriteLine(message.ToString());
            connection.SendMessage(message.Data);
        }

        public void Start()
        {
            _peer.Listen(27000);
        }

        public void Update()
        {
            _peer.Update();
        }
    }
}
