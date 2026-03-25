using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public enum TileType
    {
        Empty,
        Floor,
        Wall,
        Door,
        Stair,

        Player,
        Enemy
    }

    internal class Tile
    {
        private TileType _type;
        private Pos _pos;

        public TileType Type => _type;
        public Pos Pos => _pos;
        public int Row => Pos.Row;
        public int Col => Pos.Col;

        public Tile(TileType type, Pos pos)
        {
            _type = type;
            _pos = pos;
        }

        public void SetType(TileType type)
        {
            _type = type;
        }

        public char GetSymbol()
        {
            switch (_type)
            {
                case TileType.Floor:
                    return ' ';
                case TileType.Wall:
                    return '#';
                case TileType.Door:
                    return 'O';
                case TileType.Stair:
                    return 'S';
                case TileType.Player:
                    return 'P';
                case TileType.Enemy:
                    return 'M';
            }

            return ' ';
        }

        public bool IsWalkable()
        {
            return _type == TileType.Floor;
        }
    }
}
