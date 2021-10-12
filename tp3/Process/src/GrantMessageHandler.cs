using System;
using System.IO;
using System.Text;
using TP3.Networking;
using TP3.Common;
using System.Threading;

namespace TP3.Process
{
    internal class GrantMessageHandler : MessageHandler
    {
        public override MessageType MessageId => MessageType.Grant;

        private readonly ProcessData _processData;

        public GrantMessageHandler(ProcessData processData)
        {
            _processData = processData;
        }

        public override void Handle(Connection connection, int? id)
        {
            Console.WriteLine("[GrantMessageHandler.Handle]");
            var timeString = DateTime.Now.ToString("hh:mm:ss.fff tt");
            var resultString = $"Id: {_processData.Index}, time: {timeString}\n";

            using (var file = File.Open("resultado.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                Console.Write(resultString);
                file.Write(Encoding.UTF8.GetBytes(resultString));
            }

            Thread.Sleep(_processData.SleepTime * 1000);
            _processData.Release();

            if (_processData.RepetitionsCounter >= _processData.RepetitionsNumber)
            {
                // Sleep to have time to flush messages in the Peer Thread 
                Thread.Sleep(100);
                connection.Disconnect();
                return;
            }
            _processData.Request();
        }
    }
}
