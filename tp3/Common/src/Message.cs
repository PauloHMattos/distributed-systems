using System;
using System.Text;

namespace TP3.Common
{
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
}