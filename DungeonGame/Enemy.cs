using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Enemy : Entity
    {
        private int _currentTurn = 0;
        private int _moveTurn = 0;

        public Enemy(string name, int demage, int maxHp = 10) : base(name, demage, maxHp)
        {
        }

        public Pos Move(Pos playerPos)
        {
            int row = _pos.row;
            int col = _pos.col;

            Pos nextPos;

            _currentTurn++;
            if (_currentTurn < _moveTurn)
            {
                nextPos = new Pos(row, col);
            }
            else
            {
                int diffRow = Math.Abs(playerPos.row - row);
                int diffCol = Math.Abs(playerPos.col - col);

                if (diffRow > diffCol)
                {
                    if (playerPos.row > row)
                        nextPos = new Pos(row + 1, col);
                    else
                        nextPos = new Pos(row - 1, col);
                }
                else
                {
                    if (playerPos.col > col)
                        nextPos = new Pos(row, col + 1);
                    else
                        nextPos = new Pos(row, col - 1);
                }

                _currentTurn = 0;
            }

            Random rnd = new Random();
            _moveTurn = rnd.Next(2, 4);
            return nextPos;
        }

        public override void SetStartPos(Map map)
        {
            Random rnd = new Random();

            int rndRow, rndCol;

            do
            {
                rndRow = rnd.Next(2, map.Row - 1);
                rndCol = rnd.Next(1, map.Col - 1);

            } while (map.GetTile(rndRow, rndCol) != Map.C_FLOOR);

            _pos.row = rndRow;
            _pos.col = rndCol;
            map.SetTile(rndRow, rndCol, Map.C_ENEMY);
        }
    }
}

