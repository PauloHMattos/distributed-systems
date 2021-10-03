using System;
using System.Net;

namespace TP3.Networking
{
    public class Connection
    {
        public byte Index;
        public IPEndPoint EndPoint;
        public long LastReceivedTime;
        public long LastSendTime;

        private readonly Peer _peer;

        public Connection(Peer peer, IPEndPoint endPoint, byte index, long connectionTime)
        {
            _peer = peer;
            EndPoint = endPoint;
            Index = index;
            LastReceivedTime = connectionTime;
        }

        public void SendMessage(ReadOnlySpan<byte> message)
        {
            _peer.SendMessage(this, message);
        }

        public void Disconnect()
        {
            _peer.Disconnect(this);
        }
    }

}