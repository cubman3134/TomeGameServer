using CommonData;
using Models.Game.Server;
using NetCoreServer;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using TcpClient = NetCoreServer.TcpClient;

namespace GameClient
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(object message)
        {
            Message = message;
        }

        public object Message { get; set; }
    }

    public class GameClient : TcpClient
    {
        public GameClient(string address, int port) : base(address, port) { }

        public event EventHandler<MessageReceivedEventArgs> RaiseMessageReceivedEvent;

        public override bool Connect()
        {
            return ConnectAsync();
        }

        public override bool Disconnect()
        {
            DisconnectAndStop();
            return true;
        }

        public void SendMessage(object data)
        {
            var serializedData = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(serializedData);
            _ = SendAsync(bytes);
        }

        protected virtual void OnRaiseMessageReceivedEvent(MessageReceivedEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<MessageReceivedEventArgs> raiseEvent = RaiseMessageReceivedEvent;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                // Format the string to send inside the CustomEventArgs parameter
                e.Message += $" at {DateTime.Now}";

                // Call to raise the event.
                raiseEvent(this, e);
            }
        }

        private void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);
            var deserializedBaseMessage = JsonSerializer.Deserialize<ServerMessageBase>(message);
            Type messageType = deserializedBaseMessage?.ServerMessageType ?? typeof(ServerMessageBase);
            var deserializedMessage = JsonSerializer.Deserialize(message, messageType) ?? new object();
            OnRaiseMessageReceivedEvent(new MessageReceivedEventArgs(deserializedMessage));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;
    }
}