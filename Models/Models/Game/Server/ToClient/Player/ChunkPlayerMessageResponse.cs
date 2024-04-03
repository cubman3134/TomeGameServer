using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Game.Server.ToClient.Player
{
    public class ChunkPlayerMessageResponse : ServerMessageBase
    {
        public List<MapTileInfo> MapTiles { get; set; }
    }
}
