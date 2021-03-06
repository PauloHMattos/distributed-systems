using System;
using TP3.Networking;
using TP3.Common;

namespace TP3.Process
{
    internal class ProcessData
    {
        public int? Index { get; private set; }
        public int SleepTime { get; }
        public int RepetitionsNumber { get; }
        public int RepetitionsCounter { get; private set; }
        public Connection? Connection { get; private set; }
        public bool Initialized => Index.HasValue;

        public ProcessData(int sleepTime, int repetitionsNumber)
        {
            SleepTime = sleepTime;
            RepetitionsNumber = repetitionsNumber;
        }

        public void Initialize(Connection connection, int index)
        {
            if (Initialized)
            {
                throw new InvalidOperationException();
            }
            Connection = connection;
            Index = index;
            Console.WriteLine($"[ProcessData] index = {index}");
        }

        public void Request()
        {
            var requestMessage = Message.Build(MessageType.Request);
            Connection!.SendMessage(requestMessage.Data);
        }

        public void Release()
        {
            RepetitionsCounter += 1;
            Console.WriteLine("Sending Release");
            var requestMessage = Message.Build(MessageType.Release);
            Connection!.SendMessage(requestMessage.Data);
        }
    }
}
