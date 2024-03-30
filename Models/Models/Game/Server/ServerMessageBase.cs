using CommonData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Game.Server
{
    public abstract class ServerMessageBase
    {
        public Type ServerMessageType { get; set; }
        public string SerializedResponse { get; set; }

        public abstract void HandleMessage();
    }
}
