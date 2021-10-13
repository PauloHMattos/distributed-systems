using TP3.Networking;
using TP3.Common;

namespace TP3.Coordinator
{
    internal class RequestMessageHandler : MessageHandler
    {
        public override MessageType MessageId => MessageType.Request;

        private readonly Coordinator _coordinator;

        public RequestMessageHandler(Coordinator queue)
        {
            _coordinator = queue;
        }

        public override void Handle(Connection connection, int? id)
        {
            _coordinator.Enqueue(connection);
        }
    }
}
