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
        public Player(string name, int demage, int maxHp = 10) : base(name, demage, maxHp)
        {
        }

        public Pos Move()
        {
            ConsoleKeyInfo key = Console.ReadKey();

            int row = _pos.row;
            int col = _pos.col;

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

        public override void SetStartPos(Map map)
        {
            Random rnd = new Random();

            int rndRow, rndCol;

            do
            {
                rndRow = 1;
                rndCol = rnd.Next(1, map.Col - 1);

            } while (map.GetTile(rndRow, rndCol) != Map.C_FLOOR);

            _pos.row = rndRow;
            _pos.col = rndCol;
            map.SetTile(rndRow, rndCol, Map.C_PLAYER);
        }
    }
}
