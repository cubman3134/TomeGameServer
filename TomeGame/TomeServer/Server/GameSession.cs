using Models.Game.Server;
using Models.Game.Server.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using TOMEBL.Server;
using TomeServer.Server.NetCoreServer;
using TomeServer.Server;

namespace TomeServer.Server
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

        private bool SendMessage(object serverMessage)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full };
            var serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(serverMessage, settings);
            return SendAsync(serializedMessage);
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full };
            var deserializedMessage = Newtonsoft.Json.JsonConvert.DeserializeObject(message, settings);

            var response = ServerMessageBL.ProcessMessageFromClient((ServerMessageBase)deserializedMessage);
            if (response != null)
            {
                SendMessage(response);
            }
            // Multicast message to all connected sessions
            //Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            /*if (messageType == typeof(DisconnectServerMessage))
            {
                Disconnect();
            }*/
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Game TCP session caught an error with code {error}");
        }
    }
}
