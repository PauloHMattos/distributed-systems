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
        }
    }

    internal class GrantMessageHandler : MessageHandler
    {
        public override MessageType MessageId => MessageType.Grant;

        private readonly int _k;

        public GrantMessageHandler(int k)
        {
            _k = k;
        }

        public override void Handle(Connection connection, int? id)
        {
            if (!id.HasValue)
            {
                Console.WriteLine("[SetIdMessageHandler] Id not provided");
                connection.Disconnect();
                return;
            }

            using var file = File.Open("resultado.txt", FileMode.Append);
            var builder = new StringBuilder();
            var timeString = DateTime.Now.ToString("hh:mm:ss");
            builder.Append($"Id: {id}, time: {timeString}");

            file.Write(Encoding.UTF8.GetBytes(builder.ToString()));

            Thread.Sleep(_k * 1000);
        }
    }

    internal class ProcessData
    {
        public int? Index { get; private set; } = null;
        public Connection? Connection { get; private set; }
        public bool Initialized => Index.HasValue;

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
    }

    internal class Process
    {
        public ProcessData Data;

        private readonly Peer _peer;
        private readonly MessageHandlerCollection _messageHandlerCollection;

        public Process()
        {
            Data = new ProcessData();
            // Sleeping interval in seconds, which should be gotten from CLI.
            var k = 2;

            _peer = Peer.CreateClientPeer(Console.Out, 512, 512, 5000);
            _peer.AttachCallbacks(OnConnected, OnDisconnected, OnMessageReceived);

            _messageHandlerCollection = new MessageHandlerCollection();

            _messageHandlerCollection.AddHandler(new SetIdMessageHandler(Data));
            _messageHandlerCollection.AddHandler(new GrantMessageHandler(k));
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
            var requestMessage = Message.Build(MessageType.Request);
            connection.SendMessage(requestMessage.Data);
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
