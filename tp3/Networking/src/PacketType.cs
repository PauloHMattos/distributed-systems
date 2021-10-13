namespace TP3.Networking
{
    internal enum PacketType : byte
    {
        // Peer layer messages
        Command = 1,
        KeepAlive = 2,
        ApplicationMessage = 3,
    }
}