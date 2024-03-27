using CommonData.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TOMEDBDataAccess.Accessors;

namespace TOMEUnitTests
{
    [TestClass]
    public class MapTileTests
    {
        [TestMethod]
        public void MapTileInfo_GetTilesForChunk_NULL()
        {
            var mapTiles = MapTileInfoDA.GetAllMapTileInfoByChunkId(-1);
            Assert.IsTrue(mapTiles.Count == 0);
        }

        [TestMethod]
        public void MapTileInfo_InsertReadAndDelete_1()
        {
            long chunkToTest = int.MaxValue;
            var mapTile = new Models.Game.MapTileInfo()
            {
                IsWalkable = true,
                ChunkId = chunkToTest,
                MapTileId = 1,
                MapTileType = MapTileTypes.Grass,
                XPosition = 1,
                YPosition = 1,
            };
            bool success = MapTileInfoDA.InsertRecord(mapTile);
            Assert.IsTrue(success);
            var mapTiles = MapTileInfoDA.GetAllMapTileInfoByChunkId(mapTile.MapTileId);
            Assert.IsTrue(mapTiles.Count == 1);
            success = MapTileInfoDA.DeleteByChunkId(chunkToTest);
            Assert.IsTrue(success);
            mapTiles = MapTileInfoDA.GetAllMapTileInfoByChunkId(chunkToTest);
            Assert.IsTrue(mapTiles.Count == 0);
        }
    }
}
