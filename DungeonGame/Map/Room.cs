using DungeonGame.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DungeonGame
{
    public enum Direction
    {
        Left, Right, Up, Down
    }

    public enum RoomType
    {
        Start, Normal, Key, Stair, Boss
    }

    public readonly struct RoomGridPos
    {
        public int X { get; }
        public int Y { get; }

        public RoomGridPos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public RoomGridPos Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return new RoomGridPos(X, Y - 1);
                case Direction.Right:
                    return new RoomGridPos(X, Y + 1);
                case Direction.Up:
                    return new RoomGridPos(X - 1, Y);
                case Direction.Down:
                    return new RoomGridPos(X + 1, Y);
            }

            return default;
        }

        public bool IsEqual(RoomGridPos other) => X == other.X && Y == other.Y;
    }

    internal class Room
    {
        private readonly Dictionary<Direction, Door> _doorDicts = new Dictionary<Direction, Door>();

        private int _roomId;
        private RoomType _roomType;
        private int _width;
        private int _height;
        private RoomGridPos _gridPos;

        private bool _hasKey;
        private bool _hasStair;

        private Tile[,] _tiles;
        private List<Enemy> _enemies = new List<Enemy>();

        public int RoomId => _roomId;
        public RoomType RoomType => _roomType;
        public int Width => _width;
        public int Height => _height;
        public RoomGridPos GridPos => _gridPos;
        public int DoorCount => _doorDicts.Count;
        public IReadOnlyList<Enemy> Enemies => _enemies;

        public Room(int id, RoomType type, int width, int height, RoomGridPos gridPos, MapData mapData)
        {
            _roomId = id;
            _roomType = type;
            _width = width;
            _height = height;
            _gridPos = gridPos;

            _tiles = new Tile[height, width];
            CreateBorderWall(height, width);
            InitializeByRoomType(mapData);
        }

        private void InitializeByRoomType(MapData mapData)
        {
            switch (_roomType)
            {
                case RoomType.Normal:
                    SpawnEnemies(mapData);
                    break;
                case RoomType.Key:
                    CreateKey(GetCenterPos());
                    break;
                case RoomType.Stair:
                    SpawnEnemies(mapData);
                    CreateStair(GetCenterPos());
                    break;
                case RoomType.Boss:
                    {
                        Pos centerPos = GetCenterPos();
                        SpawnBoss(centerPos);
                        CreateStair(new Pos(2, centerPos.Col));
                    }

                    break;
            }
        }

        public void SetTile(TileType type, int row, int col)
        {
            _tiles[row, col] = new Tile(type, new Pos(row, col));
        }

        public Tile GetTile(int row, int col)
        {
            return _tiles[row, col];
        }

        public bool HasDoor(Direction dir)
        {
            return _doorDicts.ContainsKey(dir);
        }

        public void AddDoor(Direction dir, int targetRoomId, Pos targetSpawnPos)
        {
            if (_doorDicts.ContainsKey(dir))
                return;

            Pos doorPos = GetDoorPos(dir);
            Door door = new Door(doorPos, dir, targetRoomId, targetSpawnPos);
            _doorDicts.Add(dir, door);

            SetTile(TileType.Door, doorPos.Row, doorPos.Col);
        }

        public Door GetDoor(Pos pos)
        {
            return _doorDicts.Values.FirstOrDefault(door => door.Pos.Row == pos.Row && door.Pos.Col == pos.Col);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        public bool CanMoveTo(Pos pos, Enemy self)
        {
            if (!IsInBound(pos))
                return false;

            if (!GetTile(pos.Row, pos.Col).IsWalkable())
                return false;

            if (HasEnemyAt(pos, self))
                return false;

            return true;
        }

        public bool HasEnemyAt(Pos pos, Enemy self)
        {
            foreach (var enemy in _enemies)
            {
                if (ReferenceEquals(enemy, self))
                    continue;

                if (enemy.Pos.IsEqual(pos))
                    return true;
            }

            return false;
        }

        public bool IsInBound(Pos pos)
        {
            return pos.Row < _height && pos.Col < _width;
        }

        public void SetRoomType(RoomType roomType, MapData mapData)
        {
            _roomType = roomType;
            _enemies.Clear();

            _hasKey = false;
            _hasStair = false;
            InitializeByRoomType(mapData);
        }

        public Direction? GetUnconnectedRandomDirection(Random random)
        {
            List<Direction> dirs = new List<Direction>();

            if (!HasDoor(Direction.Left)) dirs.Add(Direction.Left);
            if (!HasDoor(Direction.Right)) dirs.Add(Direction.Right);
            if (!HasDoor(Direction.Up)) dirs.Add(Direction.Up);
            if (!HasDoor(Direction.Down)) dirs.Add(Direction.Down);

            if (dirs.Count == 0)
                return null;

            return dirs[random.Next(dirs.Count)];
        }

        public Pos GetDoorPos(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return new Pos(_height / 2, 0);
                case Direction.Right:
                    return new Pos(_height / 2, _width - 1);
                case Direction.Up:
                    return new Pos(0, _width / 2);
                default:
                    return new Pos(_height - 1, _width / 2);
            }
        }

        public Pos GetTargetSpawnPos(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return new Pos(_height / 2, 1);
                case Direction.Right:
                    return new Pos(_height / 2, _width - 2);
                case Direction.Up:
                    return new Pos(1, _width / 2);
                default:
                    return new Pos(_height - 2, _width / 2);
            }
        }

        public void PrintRoom(RenderManager screenManager, Player player)
        {
            Rect contentRect = screenManager.GetPanel(PanelType.Map).GetContentRect();

            int offsetX = (contentRect.Width - _width) / 2;
            int offsetY = (contentRect.Height - _height) / 2;

            if (offsetX < 0) offsetX = 0;
            if (offsetY < 0) offsetY = 0;

            // 타일 출력
            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    char symbol = _tiles[row, col].GetSymbol();
                    screenManager.DrawChar(PanelType.Map, offsetX + col, offsetY + row, symbol);
                }
            }

            // 몬스터 출력
            foreach (var enemy in _enemies)
            {
                screenManager.DrawChar(
                   PanelType.Map,
                   offsetX + enemy.Pos.Col,
                   offsetY + enemy.Pos.Row,
                   enemy.Symbol);
            }

            // 플레이어 출력
            screenManager.DrawChar(
                PanelType.Map,
                offsetX + player.Pos.Col,
                offsetY + player.Pos.Row,
                player.Symbol);
        }

        private void CreateBorderWall(int row, int col)
        {
            // 외각선은 벽으로 할당
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if (r == 0 || r == row - 1 || c == 0 || c == col - 1)
                    {
                        SetTile(TileType.Wall, r, c);
                    }
                    else
                    {
                        SetTile(TileType.Floor, r, c);
                    }
                }
            }
        }

        public void CreateKey(Pos pos)
        {
            SetTile(TileType.Key, pos.Row, pos.Col);
            _hasKey = true;
        }

        public void CreateStair(Pos pos)
        {
            SetTile(TileType.Stair, pos.Row, pos.Col);
            _hasStair = true;
        }

        public void SpawnBoss(Pos pos)
        {
            Enemy boss = CreateEnemy(EnemyType.Dragon);
            _enemies.Add(boss);
            boss.MoveTo(new Pos(pos.Row, pos.Col));
        }

        public void SpawnEnemies(MapData mapData)
        {
            if (mapData == null)
                return;

            if (mapData.SpawnEnemyTypes == null || mapData.SpawnEnemyTypes.Count == 0)
                return;

            Random random = new Random();

            int enemyCount = random.Next(mapData.EnemyCountMinInRoom, mapData.EnemyCountMaxInRoom + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                EnemyType rndType = mapData.SpawnEnemyTypes[random.Next(mapData.SpawnEnemyTypes.Count)];
                Enemy spawnEnemy = CreateEnemy(rndType);

                if (spawnEnemy != null)
                {
                    _enemies.Add(spawnEnemy);
                    spawnEnemy.SetStartPos(this);
                }
            }
        }

        private Enemy CreateEnemy(EnemyType type)
        {
            var dataManager = GameManager.Instance.DataManager;
            EnemyData enemyData = dataManager.EnemyTable.GetEnemyData(type);

            switch (type)
            {
                case EnemyType.Slime:
                    return new Slime(enemyData.Name, enemyData.Demage, enemyData.MaxHp, enemyData.MoveTurn);
                case EnemyType.Zombie:
                    return new Zombie(enemyData.Name, enemyData.Demage, enemyData.MaxHp, enemyData.MoveTurn);
                case EnemyType.Goblin:
                    return new Goblin(enemyData.Name, enemyData.Demage, enemyData.MaxHp, enemyData.MoveTurn);
                case EnemyType.Dragon:
                    return new Dragon(enemyData.Name, enemyData.Demage, enemyData.MaxHp, enemyData.MoveTurn);
            }
            return null;
        }

        private Pos GetCenterPos()
        {
            int centerRow = _height / 2;
            int centerCol = _width / 2;

            return new Pos(centerRow, centerCol);
        }
    }
}
