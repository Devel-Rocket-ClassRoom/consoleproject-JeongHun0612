using DungeonGame.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class DataManager
    {
        private const string FolderPath = "./GameData";

        private readonly EnemyTable defaultEnemyTable = new(rows: new List<EnemyData>
        {
            new EnemyData(EnemyType.Slime, "슬라임", 'S', 1, 4, 2),
            new EnemyData(EnemyType.Zombie, "좀비", 'Z', 2, 6, 2),
            new EnemyData(EnemyType.Goblin, "고블린", 'G', 2, 6, 2),
        });

        private readonly StageTable defaultStageTable = new(rows: new List<StageData>
        {
            new StageData(id: 1, name: "고블린의 숲",
                mapDatas: new List<MapData>
                {
                    new MapData(
                        roomCount: 6,
                        enemyCountMinInRoom: 2,
                        enemyCountMaxInRoom: 3,
                        spawnEnemyTypes: new List<EnemyType>
                        {
                            EnemyType.Slime,
                            EnemyType.Zombie
                        }
                    ),
                    new MapData(
                        roomCount: 8,
                        enemyCountMinInRoom: 2,
                        enemyCountMaxInRoom: 4,
                        spawnEnemyTypes: new List<EnemyType>
                        {
                            EnemyType.Slime,
                            EnemyType.Zombie,
                            EnemyType.Goblin
                        }
                    )
                }
            )
        });

        private EnemyTable _enemyTable;
        private StageTable _stageTable;

        public EnemyTable EnemyTable => _enemyTable;
        public StageTable StageTable => _stageTable;

        public void Initialize()
        {
            _enemyTable = LoadData<EnemyTable>();
            if (_enemyTable == null)
            {
                SaveData(defaultEnemyTable);
                _enemyTable = defaultEnemyTable;
            }

            _stageTable = LoadData<StageTable>();
            if (_stageTable == null)
            {
                SaveData(defaultStageTable);
                _stageTable = defaultStageTable;
            }
        }

        public void SaveData<T>(T data, string fileName = null)
        {
            if (fileName == null)
                fileName = typeof(T).Name;

            string filePath = GetFilePath(fileName);

            try
            {
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string json = JsonSerializer.Serialize<T>(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public T? LoadData<T>(string fileName = null)
        {
            if (fileName == null)
                fileName = typeof(T).Name;

            string filePath = GetFilePath(fileName);

            try
            {
                string json = File.ReadAllText(filePath);
                Console.WriteLine("JSON 데이터 \n" + json);

                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return default;
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(FolderPath, $"{fileName}.json");
        }
    }
}
