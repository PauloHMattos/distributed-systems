using TP3.Networking;

namespace TP3.Common
{
    public abstract class MessageHandler
    {
        public abstract MessageType MessageId { get; }

        public abstract void Handle(Connection connection, int? id);
    }
}