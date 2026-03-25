using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public enum Direction
    {
        Left, Right, Up, Down
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
        private int _width;
        private int _height;
        private RoomGridPos _gridPos;

        private Tile[,] _tiles;

        private List<Enemy> _enemies = new List<Enemy>();

        public int RoomId => _roomId;
        public int Width => _width;
        public int Height => _height;
        public RoomGridPos GridPos => _gridPos;

        public int DoorCount => _doorDicts.Count;


        public Room(int id, int width, int height, RoomGridPos gridPos)
        {
            _roomId = id;
            _width = Math.Max(width, 10);
            _height = Math.Max(height, 10);
            _gridPos = gridPos;

            _tiles = new Tile[height, width];
            CreateRoom(height, width);
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

        public bool IsInBound(Pos pos)
        {
            return pos.Row < _height && pos.Col < _width;
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

        public void PrintRoom()
        {
            Console.SetCursorPosition(0, 0);

            for (int r = 0; r < _height; r++)
            {
                for (int c = 0; c < _width; c++)
                {
                    Console.Write(_tiles[r, c].GetSymbol());
                }
                Console.WriteLine();
            }
        }

        private void CreateRoom(int row, int col)
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
    }
}
