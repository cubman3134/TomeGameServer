using Models.Game.Server;
using Models.Game.Server.Connection;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tome2DServer.Server
{
    class GameSession : TcpSession
    {
        public GameSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"Game TCP session with Id {Id} connected!");

            // Send invite message
            //string message = "Hello from TCP game! Please send a message or '!' to disconnect the client!";
            //SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Game TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);
            var deserializedBaseMessage = JsonSerializer.Deserialize<ServerMessageBase>(message);
            Type messageType = deserializedBaseMessage?.ServerMessageType ?? typeof(ServerMessageBase);
            var deserializedMessage = JsonSerializer.Deserialize(message, messageType) ?? new object();
            ((ServerMessageBase)deserializedMessage).HandleMessage();
            // Multicast message to all connected sessions
            //Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            if (messageType == typeof(DisconnectServerMessage))
            {
                Disconnect();
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Game TCP session caught an error with code {error}");
        }
    }
}
