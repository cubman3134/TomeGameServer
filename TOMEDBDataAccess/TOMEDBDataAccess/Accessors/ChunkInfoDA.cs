using Models.Game;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMEDBDataAccess.Accessors
{
    public class ChunkInfoDA
    {
        public static bool InsertRecord(ChunkInfo chunkInfo)
        {
            var ignorableProperties = new List<string>()
            {
                nameof(chunkInfo.ChunkId)
            };
            return DatabaseInfo.Instance.UpdateRecord<ChunkInfo>("pp_CIInsertRecord", chunkInfo, ignorableProperties);
        }

        public static ChunkInfo GetChunkInfoByStartingCoordinates(float xPositionStart, float yPositionStart)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@XPositionStart", xPositionStart),
                new SqlParameter("@YPositionStart", yPositionStart),
            };
            DatabaseInfo.Instance.ReadRecord<ChunkInfo>("pp_CIReadByStartingCoordinates", parameters, out var response);
            return response;
        }
    }
}
