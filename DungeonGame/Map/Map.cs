using DungeonGame.Data;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Map
    {
        private const int MIN_ROOMCOUNT = 5;

        private const int MIN_ROOM_WIDTH = 20;
        private const int MAX_ROOM_WIDTH = 25;
        private const int MIN_ROOM_HEIGHT = 10;
        private const int MAX_ROOM_HEIGHT = 15;

        private List<Room> _rooms = new List<Room>();
        private Dictionary<RoomGridPos, Room> _roomByPos = new Dictionary<RoomGridPos, Room>();

        public Room CurrentRoom { get; private set; }

        public void GenerateRandomRoom(MapData mapData)
        {
            Random random = new Random();

            // 최소 방 갯수는 MIN_ROOMCOUNT개 이상
            int roomCount = Math.Max(mapData.RoomCount, MIN_ROOMCOUNT);

            // 룸 정보 초기화
            _rooms.Clear();
            _roomByPos.Clear();

            // 시작 방 생성
            Room startRoom = CreateRoom(0, new RoomGridPos(0, 0), random);
            AddRoom(startRoom);

            // 나머지 방 생성
            int nextId = 0;
            while (nextId < roomCount && _rooms.Count < roomCount)
            {
                Room current = _rooms[nextId];

                for (int i = 0; i < 2; i++)
                {
                    if (_rooms.Count >= roomCount)
                        break;

                    Direction? unConnectedDir = current.GetUnconnectedRandomDirection(random);
                    if (unConnectedDir == null)
                        break;

                    Direction dir = unConnectedDir.Value;
                    RoomGridPos targetPos = current.GridPos.Move(dir);

                    if (!_roomByPos.TryGetValue(targetPos, out Room targetRoom))
                    {
                        int roomId = _rooms.Count;
                        targetRoom = CreateRoom(roomId, targetPos, random);
                        AddRoom(targetRoom);
                    }

                    // 방 연결
                    TryLinkRoom(current, targetRoom, dir);
                }

                nextId++;
            }

            CurrentRoom = startRoom;
        }

        public void ChangeRoom(int targetRoomId)
        {
            if (targetRoomId > _rooms.Count - 1)
            {
                Console.WriteLine($"{targetRoomId}가 Rooms의 Count 보다 값이 큽니다.");
                return;
            }

            CurrentRoom = _rooms[targetRoomId];
        }

        private Room CreateRoom(int id, RoomGridPos gridPos, Random random)
        {
            int width = random.Next(MIN_ROOM_WIDTH, MAX_ROOM_WIDTH);
            int height = random.Next(MIN_ROOM_HEIGHT, MAX_ROOM_HEIGHT);

            return new Room(id, width, height, gridPos);
        }

        private void AddRoom(Room room)
        {
            _rooms.Add(room);
            _roomByPos.Add(room.GridPos, room);
        }

        private bool TryLinkRoom(Room from, Room to, Direction fromDir)
        {
            Direction toDir = Opposite(fromDir);

            // 이미 해당 위치에 문이 존재하면
            if (from.HasDoor(fromDir) || to.HasDoor(toDir))
                return false;

            // 이미 문이 4방향에 다 존재한다면
            if (from.DoorCount >= 4 || to.DoorCount >= 4)
                return false;

            // 문 생성
            from.AddDoor(fromDir, to.RoomId, to.GetTargetSpawnPos(toDir));
            to.AddDoor(toDir, from.RoomId, from.GetTargetSpawnPos(fromDir));

            return true;
        }

        private Direction Opposite(Direction dir)
        {
            switch (dir)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Up:
                    return Direction.Down;
                default:
                    return Direction.Up;
            }
        }
    }
}
