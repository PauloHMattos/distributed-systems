using System;
using System.Net;
using TP3.Networking;

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


    public class Coordinator
    {
        private readonly int _maxProcesses;
        private readonly Peer _peer;

        public Coordinator(int maxProcesses)
        {
            _maxProcesses = maxProcesses;
            _peer = Peer.CreateServerPeer(Console.Out, 512, 512,  _maxProcesses, 5000);
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
            _peer.Listen(27000);
        }

        public void Update()
        {
            _peer.Update();
        }
    }
}
