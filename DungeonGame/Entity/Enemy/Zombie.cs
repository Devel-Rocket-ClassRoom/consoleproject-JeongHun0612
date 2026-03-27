using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Zombie : Enemy
    {
        private MoveAxis _axis;
        private int _dir;

        public Zombie(string name, int demage, int maxHp, int moveTurn) : base(name, 'Z', demage, maxHp, moveTurn)
        {
            Random random = new Random();
            _axis = random.Next(0, 2) == 0 ? MoveAxis.Vertical : MoveAxis.Horizontal;
            _dir = random.Next(0, 2) == 0 ? -1 : 1;
        }

        public override void OnAction(Room room, Player player)
        {
            Pos nextPos = GetNextPos();

            if (!nextPos.IsValid() || !room.IsInBound(nextPos))
                return;

            if (!room.GetTile(nextPos.Row, nextPos.Col).IsWalkable() || room.HasEnemyAt(nextPos, this))
            {
                _dir *= -1;
                nextPos = GetNextPos();
            }

            if (nextPos.IsEqual(player.Pos))
            {
                player.TakeDemage(_demage);
                return;
            }

            if (room.CanMoveTo(nextPos, this))
            {
                MoveTo(nextPos);
            }
        }

        private Pos GetNextPos()
        {
            if (_axis == MoveAxis.Vertical)
                return new Pos(Pos.Row + _dir, Pos.Col);
            else
                return new Pos(Pos.Row, Pos.Col + _dir);
        }
    }
}
