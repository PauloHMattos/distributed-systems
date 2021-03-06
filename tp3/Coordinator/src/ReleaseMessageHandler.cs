using TP3.Networking;
using TP3.Common;

namespace TP3.Coordinator
{
    internal class ReleaseMessageHandler : MessageHandler
    {
        public override MessageType MessageId => MessageType.Release;

        private readonly Coordinator _coordinator;

        public ReleaseMessageHandler(Coordinator queue)
        {
            _coordinator = queue;
        }

        public override void Handle(Connection connection, int? id)
        {
            if (!_coordinator.Release(connection))
            {
                connection.Disconnect();
            }
        }
    }
}
