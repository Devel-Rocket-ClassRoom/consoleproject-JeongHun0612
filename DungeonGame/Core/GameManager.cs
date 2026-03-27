using DungeonGame.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private StageData _stageData;

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

            _stageData = _dataManager.StageTable.GetStageData(_currentStage);

            _currentStage = 0;
            _currentFloor = 0;

            Console.Clear();
            _renderManager.DrawBorder(PanelType.Map);
            _renderManager.DrawBorder(PanelType.Status);
            _renderManager.DrawBorder(PanelType.Log);
        }

        public void StartGame()
        {
            // 게임 데이터 초기화
            _currentFloor = 0;

            // 플레이어 초기화
            _player.Reset();

            // 맵 생성 및 초기화
            ResetNextFloor();

            // 렌더링 클리어
            _renderManager.AllClearPanel();

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
                GameStop("플레이어가 사망하였습니다.");
                return;
            }

            // 플레이어 액션
            PlayerAction();

            // 몬스터 액션
            EnemiesAction();
        }

        private void ResetNextFloor()
        {
            if (_stageData == null)
                _stageData = _dataManager.StageTable.GetStageData(_currentStage);

            MapData mapData = _stageData.GetMapData(_currentFloor);

            // 맵 초기화 및 생성
            _map.ResetMap();
            if (_currentFloor == _stageData.FloorCount)
            {
                _map.GenerateBossFloor(mapData);
            }
            else
            {
                _map.GenerateNormalFloor(mapData);
            }

            // 플레이어 위치 초기화
            _player.SetStartPos(_map.CurrentRoom);

            // 스테이터스 패널 클리어
            _renderManager.ClearPanel(PanelType.Status);
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

                if (tile.Type == TileType.Key)
                {
                    tile.SetType(TileType.Floor);
                    _renderManager.DrawText(PanelType.Log, 0, 0, $"{_player.Name}가 열쇠를 획득하였습니다.");
                    _player.HasKey = true;
                }
            }
            else if (tile.Type == TileType.Door)
            {
                Room currentRoom = _map.CurrentRoom;

                if (currentRoom.RoomType == RoomType.Boss)
                {
                    _renderManager.DrawText(PanelType.Log, 0, 0, $"이전 방으로 이동할 수 없습니다.");
                    return;
                }

                Door door = currentRoom.GetDoor(nextPos);
                _map.ChangeRoom(door.TargetRoomId);

                _player.MoveTo(door.TargetSpawnPos);

                _renderManager.ClearPanel(PanelType.Status);
                _renderManager.ClearPanel(PanelType.Map);
            }
            else if (tile.Type == TileType.Stair)
            {
                Room currentRoom = _map.CurrentRoom;
                if (currentRoom.RoomType == RoomType.Boss && _map.GetTotalEnemiesCount() == 0)
                {
                    // 스테이지 클리어
                    GameStop("스테이지를 클리어 하였습니다.");
                    return;
                }

                if (_player.HasKey || _map.GetTotalEnemiesCount() == 0)
                {
                    _currentFloor++;
                    _player.HasKey = false;
                    ResetNextFloor();
                    _renderManager.DrawText(PanelType.Log, 0, 0, $"{_currentFloor + 1}층으로 이동하였습니다.");
                }
                else
                {
                    if (_map.GetTotalEnemiesCount() > 0)
                        _renderManager.DrawText(PanelType.Log, 0, 0, $"필드에 몬스터가 남아 있어 다음 층으로 올라갈 수 없습니다.");
                    else if (!_player.HasKey)
                        _renderManager.DrawText(PanelType.Log, 0, 0, $"열쇠를 습득하지 않아 다음 층으로 올라갈 수 없습니다.");
                }
            }
        }

        private void EnemiesAction()
        {
            foreach (var enemy in _map.CurrentRoom.Enemies)
            {
                enemy.Action(_map.CurrentRoom, _player);
            }
        }

        private void Render()
        {
            // 맵 출력
            _renderManager.ClearPanel(PanelType.Map);
            _renderManager.DrawText(PanelType.Map, 0, 0, $"키 :  {_player.HasKey}");
            _renderManager.DrawText(PanelType.Map, 0, 1, $"남은 몬스터 수 : {_map.GetTotalEnemiesCount()}");
            _map.CurrentRoom.PrintRoom(_renderManager, _player);

            // 스테이터스 출력
            _renderManager.DrawText(PanelType.Status, 0, 0, $"{_player.Name}[{_player.Symbol}] HP-[{_player.Hp}/{_player.MaxHp}]");

            int localY = 2;
            foreach (var enemy in _map.CurrentRoom.Enemies)
            {
                _renderManager.DrawText(PanelType.Status, 0, localY, $"{enemy.Name}[{enemy.Symbol}] HP-[{enemy.Hp}/{enemy.MaxHp}]");
                localY++;
            }
        }

        private void GameStop(string message)
        {
            // 게임 리셋
            _isRunning = false;

            _renderManager.DrawText(PanelType.Log, 0, 0, $"{message} 다시하기 [R] 게임 종료 [Q]");

            Console.SetCursorPosition(0, 0);
            string input = Console.ReadLine();
            if (input == "R" || input == "r")
            {
                StartGame();
            }
            return;
        }
    }
}
