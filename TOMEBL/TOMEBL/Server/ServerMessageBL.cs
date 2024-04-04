using Models.Game.Server.Connection;
using Models.Game.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Game.Server.Player;
using TOMEBL.Map;
using Models.Game.Server.ToClient.Player;
using CommonData.Enums;

namespace TOMEBL.Server
{
    public class ServerMessageBL
    {
        public static ServerMessageBase ProcessMessageFromClient(ServerMessageBase message)
        {
            switch (message)
            {
                case ChunkPlayerMessage chunkMessage:
                    return new ChunkPlayerMessageResponse() { MapTiles = MapBL.GetOrGenerateMapInfoAtCoordinates(chunkMessage.XLocation, chunkMessage.YLocation) };
                default:
                    break;
            }
            return null;
        }
    }
}
