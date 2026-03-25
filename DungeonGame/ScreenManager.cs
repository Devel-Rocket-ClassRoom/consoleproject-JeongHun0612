using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class ScreenManager
    {
        char[,] _map;

        public void Initialize()
        {
            _map = new char[100, 100];
        }

        public void Render()
        {
            int row = _map.GetLength(0);
            int col = _map.GetLength(1);

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    Console.Write(_map[r, c]);
                }
                Console.WriteLine();
            }
        }
    }
}
