using System.Net;

namespace TP3.Networking
{
    public class Connection
    {
        public byte Index;
        public IPEndPoint EndPoint;
        public long LastReceivedTime;
        public long LastSendTime;

        public Connection(IPEndPoint endPoint, byte index, long connectionTime)
        {
            EndPoint = endPoint;
            Index = index;
            LastReceivedTime = connectionTime;
        }
    }

}