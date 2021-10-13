using System;
using TP3.Networking;
using TP3.Common;

namespace TP3.Process
{
    internal class SetIdMessageHandler : MessageHandler
    {
        public override MessageType MessageId => MessageType.SetId;

        private readonly ProcessData _processData;

        public SetIdMessageHandler(ProcessData processData)
        {
            _processData = processData;
        }

        public override void Handle(Connection connection, int? id)
        {
            if (!id.HasValue)
            {
                Console.WriteLine("[SetIdMessageHandler] Id not provided");
                connection.Disconnect();
                return;
            }
            _processData.Initialize(connection, id.Value);
            _processData.Request();
        }
    }
}
