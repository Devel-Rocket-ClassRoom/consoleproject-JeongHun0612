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

        private const int BOSS_ROOM_WIDTH = 25;
        private const int BOSS_ROOM_HEIGHT = 15;

        private List<Room> _rooms = new List<Room>();
        private Dictionary<RoomGridPos, Room> _roomByPos = new Dictionary<RoomGridPos, Room>();

        public Room CurrentRoom { get; private set; }

        public void ResetMap()
        {
            // 룸 정보 초기화
            _rooms.Clear();
            _roomByPos.Clear();
        }

        public void GenerateNormalFloor(MapData mapData)
        {
            Random random = new Random();

            // 최소 방 갯수는 MIN_ROOMCOUNT개 이상
            int roomCount = Math.Max(mapData.RoomCount, MIN_ROOMCOUNT);

            Room startRoom = CreateNormalRoom(0, RoomType.Start, new RoomGridPos(0, 0), random, mapData);
            AddRoom(startRoom);

            int nextId = 0;
            while (nextId < _rooms.Count && _rooms.Count < roomCount)
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
                        targetRoom = CreateNormalRoom(roomId, RoomType.Normal, targetPos, random, mapData);
                        AddRoom(targetRoom);
                    }

                    TryLinkRoom(current, targetRoom, dir);
                }

                nextId++;
            }

            AssignSpecialRooms(mapData, random);
            CurrentRoom = startRoom;
        }

        public void GenerateBossFloor(MapData mapData)
        {
            Random random = new Random();

            Room startRoom = CreateNormalRoom(0, RoomType.Start, new RoomGridPos(0, 0), random, mapData);
            AddRoom(startRoom);

            Room bossRoom = CreateBossRoom(1, new RoomGridPos(-1, 0), mapData);
            AddRoom(bossRoom);

            TryLinkRoom(startRoom, bossRoom, Direction.Up);

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

        public int GetTotalEnemiesCount()
        {
            return _rooms.Sum(room => room.Enemies.Count);
        }

        private Room CreateNormalRoom(int id, RoomType type, RoomGridPos gridPos, Random random, MapData mapData)
        {
            int width = random.Next(MIN_ROOM_WIDTH, MAX_ROOM_WIDTH);
            int height = random.Next(MIN_ROOM_HEIGHT, MAX_ROOM_HEIGHT);

            return new Room(id, type, width, height, gridPos, mapData);
        }

        private Room CreateBossRoom(int id, RoomGridPos gridPos, MapData mapData)
        {
            return new Room(id, RoomType.Boss, BOSS_ROOM_WIDTH, BOSS_ROOM_HEIGHT, gridPos, mapData);
        }

        private void AssignSpecialRooms(MapData mapData, Random random)
        {
            if (_rooms.Count == 0)
                return;

            Room startRoom = _rooms[0];

            // 계단방은 가장 마지막에 생성된 방으로 할당
            Room stairsRoom = _rooms[^1];
            stairsRoom.SetRoomType(RoomType.Stair, mapData);

            // 시작방과 계단방을 제외한 랜덤한 방을 열쇠방으로 할당
            List<Room> keyCandidates = _rooms
                .Where(r => r != startRoom && r != stairsRoom)
                .ToList();

            if (keyCandidates.Count > 0)
            {
                Room keyRoom = keyCandidates[random.Next(keyCandidates.Count)];
                keyRoom.SetRoomType(RoomType.Key, mapData);
            }
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
