using CommonData;
using CommonData.Enums;
using Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TOMEDBDataAccess.Accessors;

namespace TOMEBL.Map
{
    public class MapBL
    {
        public static Tuple<float, float> GetChunkStartLocationBasedOnCoordinates(float xCoordinate, float yCoordinate)
        {
            int startLocationX = (int)(xCoordinate / Constants.ChunkDiameter);
            int startLocationY = (int)(yCoordinate / Constants.ChunkDiameter);
            return Tuple.Create((float)startLocationX, (float)startLocationY);
        }

        public static List<MapTileInfo> CreateMapForChunk(ChunkInfo chunk)
        {
            var mapTiles = new List<MapTileInfo>();
            for (var currentX = chunk.XPositionStart; currentX < chunk.XPositionStart + Constants.ChunkDiameter; currentX++)
            {
                for (var currentY = chunk.YPositionStart; currentY < chunk.YPositionStart + Constants.ChunkDiameter; currentY++)
                {
                    MapTileInfo mapTile = new MapTileInfo()
                    {
                        ChunkId = chunk.ChunkId,
                        IsWalkable = true,
                        MapTileType = MapTileTypes.Grass,
                        XPosition = currentX,
                        YPosition = currentY
                    };
                    MapTileInfoDA.InsertRecord(mapTile);
                    mapTiles.Add(mapTile);
                }
            }
            return mapTiles;
        }

        public static List<MapTileInfo> GetOrGenerateMapInfoAtCoordinates(float xCoordinate, float yCoordinate)
        {
            var chunkStartCoordinates = GetChunkStartLocationBasedOnCoordinates(xCoordinate, yCoordinate);
            var chunk = ChunkInfoDA.GetChunkInfoByStartingCoordinates(chunkStartCoordinates.Item1, chunkStartCoordinates.Item2);
            if (chunk == null)
            {
                chunk = new ChunkInfo()
                {
                    XPositionStart = xCoordinate,
                    YPositionStart = yCoordinate,
                };
                ChunkInfoDA.InsertRecord(chunk);
            }
            var mapTiles = MapTileInfoDA.GetAllMapTileInfoByChunkId(chunk.ChunkId);
            if (mapTiles != null && mapTiles.Count != 0)
            {
                return mapTiles;
            }
            return CreateMapForChunk(chunk);
        }
    }
}
