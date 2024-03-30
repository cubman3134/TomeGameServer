using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tome2DServer.Server
{
    public class GameServer : TcpServer
    {
        public GameServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new GameSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Game TCP server caught an error with code {error}");
        }
    }
}
