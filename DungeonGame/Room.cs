using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Room
    {
        private char[,] _map;

        private int _sizeY;
        private int _sizeX;

        private int _row;
        private int _col;

        private List<Enemy> _enemies = new List<Enemy>();

        public void SettingEnemy()
        {

        }
    }
}
