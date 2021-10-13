using System;
using System.IO;
using TP3.Networking;
using System.Collections.Generic;

namespace TP3.Common
{
    public class MessageHandlerCollection 
    {
        private readonly Dictionary<MessageType, MessageHandler> _handlers;
        private readonly TextWriter _logger;

        public MessageHandlerCollection(TextWriter logger)
        {
            _handlers = new Dictionary<MessageType, MessageHandler>();
            _logger = logger;
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
                _logger.WriteLine($"[R] Unknow Message={messageId} - {connection.Index} - {DateTime.Now.ToString("hh:mm:ss.fff tt")}");
                return false;
            }

            _logger.WriteLine($"[R] {handler.MessageId} - {connection.Index} - {DateTime.Now.ToString("hh:mm:ss.fff tt")}");
            handler.Handle(connection, id);
            return true;
        }
    }
}