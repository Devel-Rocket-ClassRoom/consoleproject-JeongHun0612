using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Data
{
    public class MapData
    {
        public int RoomCount { get; }
        public int EnemyCountMinInRoom { get; }
        public int EnemyCountMaxInRoom { get; }
        public List<EnemyType> SpawnEnemyTypes { get; } = new();

        public MapData(int roomCount, int enemyCountMinInRoom, int enemyCountMaxInRoom, List<EnemyType> spawnEnemyTypes)
        {
            RoomCount = roomCount;
            EnemyCountMinInRoom = enemyCountMinInRoom;
            EnemyCountMaxInRoom = enemyCountMaxInRoom;
            SpawnEnemyTypes = spawnEnemyTypes;
        }
    }

    public class StageData
    {
        public int Id { get; }
        public string Name { get; }
        public List<MapData> MapDatas { get; }
        public int FloorCount => MapDatas.Count;

        public StageData(int id, string name, List<MapData> mapDatas)
        {
            Id = id;
            Name = name;
            MapDatas = mapDatas;
        }

        public MapData GetMapData(int index)
        {
            if (index > MapDatas.Count - 1)
                index = MapDatas.Count - 1;

            return MapDatas[index];
        }
    }

    internal class StageTable
    {
        private List<StageData> _rows = new List<StageData>();
        public List<StageData> Rows => _rows;

        public StageTable(List<StageData> rows)
        {
            _rows = rows;
        }

        public StageData GetStageData(int index)
        {
            if (index > _rows.Count - 1)
                index = _rows.Count - 1;

            return _rows[index];
        }
    }
}
