using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Player : Entity
    {
        public Player(string name, int demage, int maxHp) : base(name, 'P', demage, maxHp)
        {
        }

        public void Reset()
        {
            _hp = _maxHp;
        }

        public Pos Move()
        {
            Console.SetCursorPosition(0, 0);

            ConsoleKeyInfo key = Console.ReadKey();

            int row = _pos.Row;
            int col = _pos.Col;

            switch (key.Key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    return new Pos(row - 1, col);
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    return new Pos(row + 1, col);
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    return new Pos(row, col - 1);
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    return new Pos(row, col + 1);
                default:
                    return new Pos();
            }
        }

        public override void SetStartPos(Room room)
        {
            // 전달받은 룸의 중앙 위치
            int halfHeight = room.Height / 2;
            int halfWidth = room.Width / 2;

            _pos = new Pos(halfHeight, halfWidth);
        }
    }
}
