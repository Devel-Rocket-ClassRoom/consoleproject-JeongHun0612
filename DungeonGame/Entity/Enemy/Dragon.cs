using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Dragon : Enemy
    {
        public Dragon(string name, int demage, int maxHp, int moveTurn) : base(name, 'D', demage, maxHp, moveTurn)
        {
        }

        public override void OnAction(Room room, Player player)
        {
            Pos nextPos = GetNextPos(player.Pos);

            if (!nextPos.IsValid() || !room.IsInBound(nextPos))
                return;

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

        private Pos GetNextPos(Pos playerPos)
        {
            int row = _pos.Row;
            int col = _pos.Col;

            Pos nextPos;

            int diffRow = Math.Abs(playerPos.Row - row);
            int diffCol = Math.Abs(playerPos.Col - col);

            if (diffRow > diffCol)
            {
                if (playerPos.Row > row)
                    nextPos = new Pos(row + 1, col);
                else
                    nextPos = new Pos(row - 1, col);
            }
            else
            {
                if (playerPos.Col > col)
                    nextPos = new Pos(row, col + 1);
                else
                    nextPos = new Pos(row, col - 1);
            }

            return nextPos;
        }
    }
}
