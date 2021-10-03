using System;
using System.Text;
using TP3.Networking;
using System.Collections.Generic;

namespace TP3.Common
{
    public enum MessageType : byte
    {
        SetId = 1,
        Request,
        Grant,
        Release,
    }

    public class Message
    {
        private const char SEPARATOR = '|';
        private static Encoding ENCODING => Encoding.ASCII;

        private readonly string _message;
        public readonly byte[] Data;

        private Message(string str)
        {
            _message = str;
            Data = ENCODING.GetBytes(str);
        }

        public override string ToString()
        {
            return _message;
        }

        public static Message Build(MessageType type, int? id = null)
        {
            var builder = new StringBuilder();
            builder.Append((byte)type);
            if (id.HasValue)
            {
                builder.Append(SEPARATOR);
                builder.Append(id.Value);
            }
            return new Message(builder.ToString());
        }

        public static void Parse(ReadOnlySpan<byte> data, out MessageType messageId, out int? id)
        {
            var str = ENCODING.GetString(data);
            var strData = str.Split(SEPARATOR);
            messageId = (MessageType)int.Parse(strData[0]);
            
            id = null;
            if (strData.Length > 1)
            {
                id = int.Parse(strData[1]);
            }
        }
    }

    public abstract class MessageHandler
    {
        public abstract MessageType MessageId { get; }

        public abstract void Handle(Connection connection, int? id);
    }

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
            Console.WriteLine("Received a message");
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