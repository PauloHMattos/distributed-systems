using System;
using System.Net;
using System.IO;
using System.Text;
using TP3.Networking;
using TP3.Common;
using System.Threading;

namespace TP3.Process
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var process = new Process();
            process.Start();

            while(true)
            {
                process.Update();
            }
        }
    }

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

            using var file = File.Open("resultado.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            {
                Console.Write(resultString);
                file.Write(Encoding.UTF8.GetBytes(resultString));
            }

            Thread.Sleep(_processData.SleepTime * 1000);
            _processData.Release();
            _processData.Request();
        }
    }

    internal class ProcessData
    {
        public int? Index { get; private set; }
        public int SleepTime { get; }
        public Connection? Connection { get; private set; }
        public bool Initialized => Index.HasValue;

        public ProcessData(int sleepTime)
        {
            SleepTime = sleepTime;
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
            Console.WriteLine("Sending request");
            var requestMessage = Message.Build(MessageType.Request);
            Connection!.SendMessage(requestMessage.Data);
        }

        public void Release()
        {
            Console.WriteLine("Sending Release");
            var requestMessage = Message.Build(MessageType.Release);
            Connection!.SendMessage(requestMessage.Data);
        }
    }

    internal class Process
    {
        public ProcessData Data;

        private readonly Peer _peer;
        private readonly MessageHandlerCollection _messageHandlerCollection;

        public Process()
        {
            // Sleeping interval in seconds, which should be gotten from CLI.
            var k = 2;
            Data = new ProcessData(k);

            _peer = Peer.CreateClientPeer(Console.Out, 512, 512, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _messageHandlerCollection = new MessageHandlerCollection();

            _messageHandlerCollection.AddHandler(new SetIdMessageHandler(Data));
            _messageHandlerCollection.AddHandler(new GrantMessageHandler(Data));
        }

        private void OnMessageReceived(ReadOnlySpan<byte> message, Connection connection)
        {
            _messageHandlerCollection.Handle(connection, message);
        }

        private void OnDisconnected(Connection connection)
        {
        }

        private void OnConnected(Connection connection)
        {
            Console.WriteLine($"OnConnected  {connection.EndPoint}");
        }

        public void Start()
        {
            _peer.Connect(IPAddress.Parse("127.0.0.1"), 27000);
        }

        public void Update()
        {
            _peer.Update();
        }
    }
}
