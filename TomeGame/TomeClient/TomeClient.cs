using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TomeClient
{
    public class TomeClient : TcpSharp.TcpSharpSocketClient
    {
        public TomeClient(string address, int port) : base(address, port) { }

        public new bool Connect(out string error)
        {
            error = string.Empty;
            try
            {
                base.Connect();
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
            return true;
        }

        public new bool Disconnect()
        {
            try
            {
                base.Disconnect();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void SendMessage(object data)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full };
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(data, typeof(object), settings);
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            SendBytes(bytes);
        }
    }
}
