using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TomeServer.Server.NetCoreServer;

namespace TomeServer.Server
{
    public class GameServer : TcpServer
    {
        public GameServer(IPAddress address, int port) : base(address, port)
        {
            
        }

        private void GameServer_OnConnected(object sender, TcpSharp.OnServerConnectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override TcpSession CreateSession() { return new GameSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Game TCP server caught an error with code {error}");
        }
    }
}
