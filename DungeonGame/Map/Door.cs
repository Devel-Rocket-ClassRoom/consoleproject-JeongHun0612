using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Door
    {
        private Pos _pos;
        private Direction _dir;
        private int _targetRoomId;
        private Pos _targetSpawnPos;

        public Pos Pos => _pos;
        public Direction Dir => _dir;
        public int TargetRoomId => _targetRoomId;
        public Pos TargetSpawnPos => _targetSpawnPos;

        public Door(Pos pos, Direction dir, int targetRoomId, Pos targetSpawnPos)
        {
            _pos = pos;
            _dir = dir;
            _targetRoomId = targetRoomId;
            _targetSpawnPos = targetSpawnPos;
        }
    }
}
