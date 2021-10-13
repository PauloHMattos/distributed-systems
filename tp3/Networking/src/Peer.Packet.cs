using System.Net;

namespace TP3.Networking
{
    public partial class Peer
    {
        private struct Packet
        {
            public byte[] Payload;
            public IPEndPoint RemoteEndPoint;
        }
    }
}