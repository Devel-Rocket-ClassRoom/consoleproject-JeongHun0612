using DungeonGame.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public struct Pos
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Pos(int row = -1, int col = -1)
        {
            Row = row;
            Col = col;
        }
        public bool IsValid() => Row >= 0 && Col >= 0;
        public bool IsEqual(Pos other) => Row == other.Row && Col == other.Col;
    }

    internal class GameManager
    {
        private static readonly GameManager _instance = new GameManager();
        private static bool _isInitalize = false;
        private static bool _isRunning = false;

        public static GameManager Instance
        {
            get
            {
                if (!_isInitalize)
                {
                    _instance.Initialize();
                    _isInitalize = true;
                }

                return _instance;
            }
        }

        private DataManager _dataManager;
        private RenderManager _renderManager;

        private Map _map;
        private Player _player;

        private int _currentStage;
        private int _currentFloor;

        public DataManager DataManager => _dataManager;
        public RenderManager RenderManager => _renderManager;

        public void Initialize()
        {
            _renderManager = new RenderManager();
            _renderManager.Initialize();

            _dataManager = new DataManager();
            _dataManager.Initialize();

            _player = new Player("플레이어", 2, 10);
            _map = new Map();

            _currentStage = 0;
            _currentFloor = 0;

            Console.Clear();
            _renderManager.DrawBorder(PanelType.Map);
            _renderManager.DrawBorder(PanelType.Status);
            _renderManager.DrawBorder(PanelType.Log);
        }

        public void StartGame()
        {
            // 플레이어 초기화
            _player.Reset();

            // 맵 생성 및 초기화
            StageData stageData = _dataManager.StageTable.GetStageData(_currentStage);
            MapData mapData = stageData.GetMapData(_currentFloor);
            _map.GenerateRandomRoom(mapData);

            _player.SetStartPos(_map.CurrentRoom);

            _isRunning = true;
            while (_isRunning)
            {
                Render();
                Update();
            }
        }

        public void Update()
        {
            if (_player.IsDead)
            {
            }

            // 플레이어 액션
            PlayerAction();

            // 몬스터 액션
            EnemyAction();
        }

        private void PlayerAction()
        {
            Pos nextPos = _player.Move();

            if (!nextPos.IsValid() || !_map.CurrentRoom.IsInBound(nextPos))
            {
                _renderManager.DrawText(PanelType.Log, 0, 0, $"해당 위치로는 이동할 수 없습니다.");
                return;
            }

            // 다음 위치에 몬스터가 존재하면 공격
            foreach (var enemy in _map.CurrentRoom.Enemies)
            {
                if (nextPos.IsEqual(enemy.Pos))
                {
                    enemy.TakeDemage(_player.Demage);

                    if (enemy.IsDead)
                    {
                        _renderManager.ClearPanel(PanelType.Status);
                        _renderManager.DrawText(PanelType.Log, 0, 0, $"{enemy.Name}을(를) 제거 하였습니다.");
                        _map.CurrentRoom.RemoveEnemy(enemy);
                    }

                    return;
                }
            }

            Tile tile = _map.CurrentRoom.GetTile(nextPos.Row, nextPos.Col);
            if (tile.IsWalkable())
            {
                // 플레이어 위치 이동
                _player.MoveTo(nextPos);

                _renderManager.ClearPanel(PanelType.Log);
                _renderManager.DrawText(PanelType.Log, 0, 0, $"{_player.Name} 이동 - [{_player.Pos.Row}, {_player.Pos.Col}]");
            }
            else if (tile.Type == TileType.Door)
            {
                Door door = _map.CurrentRoom.GetDoor(nextPos);
                _map.ChangeRoom(door.TargetRoomId);

                _player.MoveTo(door.TargetSpawnPos);

                _renderManager.ClearPanel(PanelType.Status);
                _renderManager.ClearPanel(PanelType.Map);
            }
        }

        private void EnemyAction()
        {
            foreach (var enemy in _map.CurrentRoom.Enemies)
            {
                enemy.Action(_map.CurrentRoom, _player);
            }
        }

        private void Render()
        {
            // 맵 출력
            _map.CurrentRoom.PrintRoom(_renderManager, _player);

            _renderManager.DrawText(PanelType.Status, 0, 0, $"{_player.Name}[{_player.Symbol}] HP-[{_player.Hp}/{_player.MaxHp}]");

            int localY = 2;
            foreach (var enemy in _map.CurrentRoom.Enemies)
            {
                _renderManager.DrawText(PanelType.Status, 0, localY, $"{enemy.Name}[{enemy.Symbol}] HP-[{enemy.Hp}/{enemy.MaxHp}]");
                localY++;
            }
        }
    }
}
