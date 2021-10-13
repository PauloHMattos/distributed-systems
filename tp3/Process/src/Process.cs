using System;
using System.Net;
using TP3.Networking;
using TP3.Common;
using System.Threading;

namespace TP3.Process
{
    internal class Process
    {
        public ProcessData Data;

        private readonly Peer _peer;
        private readonly MessageHandlerCollection _messageHandlerCollection;

        public Process(int sleepTime, int repetitionsNumber)
        {
            Data = new ProcessData(sleepTime, repetitionsNumber);

            _peer = Peer.CreateClientPeer(Console.Out, 512, 512, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _messageHandlerCollection = new MessageHandlerCollection(Console.Out);

            _messageHandlerCollection.AddHandler(new SetIdMessageHandler(Data));
            _messageHandlerCollection.AddHandler(new GrantMessageHandler(Data));
        }

        private void OnMessageReceived(ReadOnlySpan<byte> message, Connection connection)
        {
            _messageHandlerCollection.Handle(connection, message);
        }

        private void OnDisconnected(Connection connection)
        {
            Environment.Exit(0);
        }

        private void OnConnected(Connection connection)
        {
            Console.WriteLine($"OnConnected  {connection.EndPoint}");
        }

        public void Start()
        {
            _peer.Connect(IPAddress.Parse("127.0.0.1"), 27000);
            Console.WriteLine($"Start");
        }

        public void Update()
        {
            _peer.Update();
            Thread.Yield();
        }
    }
}
