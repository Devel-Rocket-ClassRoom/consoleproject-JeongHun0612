using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Tile
    {
        private Pos _pos;
        private char _symbol;

        public Pos Pos { get; private set; }
        public int Row => _pos.row;
        public int Col => _pos.col;

        public char Symbol => _symbol;

        public Tile(Pos pos, char symbol)
        {
            _pos = pos;
            _symbol = symbol;
        }
    }
}
