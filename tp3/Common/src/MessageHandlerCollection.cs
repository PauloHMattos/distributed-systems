using System;
using TP3.Networking;
using System.Collections.Generic;

namespace TP3.Common
{
    public class MessageHandlerCollection 
    {
        private readonly Dictionary<MessageType, MessageHandler> _handlers;

        public MessageHandlerCollection()
        {
            _handlers = new Dictionary<MessageType, MessageHandler>();
        }

        public void AddHandler(MessageHandler handler)
        {
            _handlers.Add(handler.MessageId, handler);
        }

        public bool Handle(Connection connection, ReadOnlySpan<byte> message)
        {
            Message.Parse(message, out var messageId, out var id);

            if (!_handlers.TryGetValue(messageId, out var handler))
            {
                return false;
            }

            handler.Handle(connection, id);
            return true;
        }
    }
}