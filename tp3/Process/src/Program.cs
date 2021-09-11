using System;
using System.Net;
using TP3.Networking;

namespace TP3.Process
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var process = new Process();
            process.Start();

            while(true)
            {
                process.Update();
            }
        }
    }


    public class Process
    {
        private readonly Peer _peer;

        public Process()
        {
            _peer = Peer.CreateClientPeer(Console.Out, 512, 512, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);
        }

        private void OnMessageReceived(ReadOnlySpan<byte> message, Connection connection)
        {
        }

        private void OnDisconnected(Connection connection)
        {
        }

        private void OnConnected(Connection connection)
        {
            Console.WriteLine("Connected!");
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
