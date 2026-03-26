using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DungeonGame
{
    public enum EnemyType
    {
        Slime,
        Zombie,
        Goblin,

        //Dragon,
    }

    public enum MoveAxis
    {
        Vertical,
        Horizontal
    }

    internal abstract class Enemy : Entity
    {
        private int _currentTurn = 0;
        private int _moveTurn = 0;

        public Enemy(string name, char symbol, int demage, int maxHp, int moveTurn) : base(name, symbol, demage, maxHp)
        {
            _moveTurn = moveTurn;
        }

        public void Action(Room room, Player player)
        {
            _currentTurn++;
            if (_currentTurn < _moveTurn)
                return;

            OnAction(room, player);

            _currentTurn = 0;
        }

        public abstract void OnAction(Room room, Player player);

        public override void SetStartPos(Room room)
        {
            Random rnd = new Random();

            int rndRow, rndCol;

            do
            {
                rndRow = rnd.Next(2, room.Height - 2);
                rndCol = rnd.Next(2, room.Width - 2);

            } while (room.GetTile(rndRow, rndCol).Type != TileType.Floor);

            _pos.Row = rndRow;
            _pos.Col = rndCol;
        }

        private void Temp(Pos playerPos)
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
        }
    }
}

