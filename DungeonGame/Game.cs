using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    struct Pos
    {
        public int row;
        public int col;

        public Pos(int row = -1, int col = -1)
        {
            this.row = row;
            this.col = col;
        }

        public bool IsValid()
        {
            return this.row > 0 && this.col > 0;
        }

        public bool IsEqual(Pos pos)
        {
            return row == pos.row && col == pos.col;
        }
    }

    internal class Game
    {
        private int _totalTurn = 0;
        private int _currentStage = 0;

        private Map _map;
        private Player _player;
        private List<Enemy> _enemyList = new List<Enemy>();

        private List<StageData> _stageDatas = new List<StageData>()
        {
            { new StageData(10, 25, 3, 1, 8, 10, 1, 2) },
            { new StageData(15, 20, 5, 2, 8, 10, 1, 2) },
            { new StageData(20, 25, 7, 3, 10, 12, 1, 2) },
            { new StageData(20, 30, 9, 4, 10, 12, 1, 2) },
            { new StageData(20, 35, 11, 6, 12, 14, 2, 3) },
        };

        public void Initialize()
        {
            _totalTurn = 0;
            _currentStage = 0;

            _player = new Player("플레이어", 2, 10);

            if (_enemyList == null)
                _enemyList = new List<Enemy>();

            StageData stage = GetStageData(_currentStage);
            SettingStage(stage);
        }

        public void Update()
        {
            while (true)
            {
                if (_player.IsDead)
                {
                    Console.WriteLine("GameOver!");
                    break;
                }

                if (_enemyList.Count <= 0)
                {
                    _currentStage++;

                    if (_currentStage >= _stageDatas.Count)
                    {
                        Console.WriteLine("GameWin!");
                        break;
                    }

                    StageData stage = GetStageData(_currentStage);
                    SettingStage(stage);
                }

                _map.PrintMap();
                Console.WriteLine($"{_player.Name} HP : {_player.Hp}/{_player.MaxHp}");

                foreach (var enemy in _enemyList)
                {
                    Console.WriteLine($"{enemy.Name} HP : {enemy.Hp}/{enemy.MaxHp}");
                }

                // 플레이어 이동
                Pos nextPlayerPos = _player.Move();

                if (!nextPlayerPos.IsValid())
                {
                    // TODO 올바른 값을 입력하라는 로그 발생
                    Console.Clear();
                    continue;
                }

                //if (_map.GetTile(nextPlayerPos) == Map.C_WALL)
                //{
                //    // TODO 벽이여서 이동할 수 없다는 로그 발생
                //    Console.Clear();
                //    continue;
                //}

                if (_map.IsWalkable(nextPlayerPos))
                {
                    // 기존 플레이어가 위치 하던 타일을 바닥으로 변경
                    _map.SetTile(_player.Pos.row, _player.Pos.col, Map.C_FLOOR);

                    // 플레이어 위치 이동
                    _player.MoveTo(nextPlayerPos);
                    // 이동한 위치의 타일을 플레이어로 변경
                    _map.SetTile(nextPlayerPos.row, nextPlayerPos.col, Map.C_PLAYER);
                }
                else if (_map.GetTile(nextPlayerPos) == Map.C_ENEMY)
                {
                    foreach (var enemy in _enemyList)
                    {
                        if (enemy.Pos.IsEqual(nextPlayerPos))
                        {
                            enemy.TakeDemage(_player.Demage);

                            if (enemy.IsDead)
                            {
                                _map.SetTile(enemy.Pos.row, enemy.Pos.col, Map.C_FLOOR);
                                _enemyList.Remove(enemy);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    // TODO 다른 형태의 타일에 대한 처리
                }

                // Enemy 이동
                foreach (var enemy in _enemyList)
                {
                    Pos nextEnemyPos = enemy.Move(_player.Pos);

                    if (!nextEnemyPos.IsValid() || _map.GetTile(nextEnemyPos) == Map.C_WALL)
                        continue;

                    if (_map.IsWalkable(nextEnemyPos))
                    {
                        // 기존 몬스터가 위치 하던 타일을 바닥으로 변경
                        _map.SetTile(enemy.Pos.row, enemy.Pos.col, Map.C_FLOOR);

                        // 몬스터 위치 이동
                        enemy.MoveTo(nextEnemyPos);
                        // 이동한 위치의 타일을 몬스터로 변경
                        _map.SetTile(nextEnemyPos.row, nextEnemyPos.col, Map.C_ENEMY);
                    }
                    else if (_map.GetTile(nextEnemyPos) == Map.C_PLAYER)
                    {
                        _player.TakeDemage(enemy.Demage);

                        if (_player.IsDead)
                        {
                            _map.SetTile(_player.Pos.row, _player.Pos.col, Map.C_FLOOR);
                        }
                    }
                    else
                    {
                        // TODO 다른 형태의 타일에 대한 처리
                    }
                }

                //Console.Clear();
                _totalTurn++;
            }
        }

        public StageData GetStageData(int index)
        {
            if (_stageDatas == null || _stageDatas.Count == 0)
                return null;

            if (index > _stageDatas.Count - 1)
            {
                Console.WriteLine("Index가 StageData가 가지고 있는 데이터 크기보다 큽니다.");
                index = _stageDatas.Count - 1;
            }

            return _stageDatas[index];
        }

        public void SettingStage(StageData stage)
        {
            _map = new Map();
            _map.CreateMap(stage.mapRow, stage.mapCol, stage.mapWallCount);

            _player.SetStartPos(_map);

            Random rnd = new Random();
            for (int i = 0; i < stage.enemyCount; i++)
            {
                int rndHp = rnd.Next(stage.enemyMinHp, stage.enemyMaxHp + 1);
                int rndDemage = rnd.Next(stage.enemyMinDemage, stage.enemyMaxDemage + 1);

                Enemy enemy = new Enemy($"몬스터{i + 1}", rndDemage, rndHp);
                _enemyList.Add(enemy);
                enemy.SetStartPos(_map);
            }

            Console.Clear();
        }
    }
}
