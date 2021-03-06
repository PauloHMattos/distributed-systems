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
        public bool Active;

        private readonly Peer _peer;

        public Connection(Peer peer, IPEndPoint endPoint, byte index, long connectionTime)
        {
            _peer = peer;
            EndPoint = endPoint;
            Index = index;
            LastReceivedTime = connectionTime;
            Active = true;
        }

        public void SendMessage(ReadOnlySpan<byte> message)
        {
            _peer.SendMessage(this, message);
        }

        public void Disconnect()
        {
            _peer.SendImmediatelyTo(this, new byte[2] { (byte)PacketType.Command, (byte)CommandId.Disconnect });
            _peer.Disconnect(this);
        }
    }

}