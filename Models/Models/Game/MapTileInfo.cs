using CommonData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Game
{
    public class MapTileInfo : ModelBase
    {
        public long MapTileId { get; set; }
        public float XPosition { get; set; }
        public float YPosition { get; set; }
        public MapTileTypes MapTileType { get; set; }
        public float ChunkId { get; set; }
        public bool IsWalkable { get; set; }
    }
}
