using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Game.Server.Player
{
    public class ChunkPlayerMessage : ServerMessageBase
    {
        public float XLocation { get; set; }
        public float YLocation { get; set; }
    }
}
