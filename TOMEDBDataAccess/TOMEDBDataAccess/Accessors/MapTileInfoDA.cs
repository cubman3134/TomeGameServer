using Models.Game;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMEDBDataAccess.Accessors
{
    public class MapTileInfoDA
    {
        public static List<MapTileInfo> GetAllMapTileInfoByChunkId(long chunkId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@ChunkId", chunkId)
            };
            DatabaseInfo.Instance.ReadRecords<MapTileInfo>("pp_MTIReadAllByChunkId", parameters, out var response);
            return response;
        }

        public static bool InsertRecord(MapTileInfo mapTileInfo)
        {
            var ignorableProperties = new List<string>()
            {
                nameof(mapTileInfo.MapTileId)
            };
            return DatabaseInfo.Instance.UpdateRecord<MapTileInfo>("pp_MTIInsertRecord", mapTileInfo, ignorableProperties);
        }

        public static bool DeleteByChunkId(long chunkId)
        {
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@ChunkId", chunkId)
            };
            return DatabaseInfo.Instance.DeleteRecords("pp_MTIDeleteAllByChunkId", parameters);
        }
    }
}
