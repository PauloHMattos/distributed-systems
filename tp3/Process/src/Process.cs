using System;
using System.Net;
using TP3.Networking;
using TP3.Common;

namespace TP3.Process
{
    internal class Process
    {
        public ProcessData Data;

        private readonly Peer _peer;
        private readonly MessageHandlerCollection _messageHandlerCollection;

        public Process()
        {
            // Sleeping interval in seconds, which should be gotten from CLI.
            var k = 2;
            Data = new ProcessData(k);

            _peer = Peer.CreateClientPeer(Console.Out, 512, 512, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _messageHandlerCollection = new MessageHandlerCollection();

            _messageHandlerCollection.AddHandler(new SetIdMessageHandler(Data));
            _messageHandlerCollection.AddHandler(new GrantMessageHandler(Data));
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
            Console.WriteLine($"OnConnected  {connection.EndPoint}");
        }

        public void Start()
        {
            _peer.Connect(IPAddress.Parse("127.0.0.1"), 27000);
        }

        public void Update()
        {
            _peer.Update();
        }
    }
}
