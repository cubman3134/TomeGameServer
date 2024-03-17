using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System;
using System.Net.Http.Headers;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
//using Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Models
{
    public class ClientInfo
    {
        private static ClientInfo _instance;
        public static ClientInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientInfo();
                }
                return _instance;
            }
        }
        public static int DataBufferSize = 4096;

        public string Ip = "127.0.0.1";
        public int Port = 7010;
        public int MyId = 0;
        public TCPInfo TCPData;

        public void ConnectToServer()
        {
            TCPData.Connect();
        }

        public ClientInfo()
        {
            TCPData = new TCPInfo();
        }

        public class TCPInfo
        {
            public TcpClient Socket;

            public NetworkStream Stream;
            public byte[] ReceiveBuffer;

            public void Connect()
            {
                Socket = new TcpClient()
                {
                    ReceiveBufferSize = DataBufferSize,
                    SendBufferSize = DataBufferSize,
                };

                ReceiveBuffer = new byte[DataBufferSize];
                Socket.BeginConnect(Instance.Ip, Instance.Port, ConnectCallback, Socket);
            }

            private void ConnectCallback(IAsyncResult result)
            {
                Socket.EndConnect(result);
                if (!Socket.Connected)
                {
                    return;
                }
                Stream = Socket.GetStream();
                Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = Stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        // todo: disconnect
                        return;
                    }
                    byte[] data = new byte[byteLength];
                    Array.Copy(ReceiveBuffer, data, byteLength);

                    // todo handle data
                    Stream.BeginRead(ReceiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // todo: disconnect
                }
            }
        }
    }
}
