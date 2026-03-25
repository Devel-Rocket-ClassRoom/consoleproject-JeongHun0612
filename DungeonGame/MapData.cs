using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class MapData
    {
        private List<List<Tile>> _tiles;

        public List<List<Tile>> Tiles => _tiles;

        public void SaveMapData(char[,] map)
        {
            int row = map.GetLength(0);
            int col = map.GetLength(1);

            if (_tiles == null)
                _tiles = new List<List<Tile>>();

            for (int r = 0; r < row; r++)
            {
                var rowList = new List<Tile>();

                for (int c = 0; c < col; c++)
                {
                    Tile tile = new Tile(TileType.Floor, new Pos(r, c));
                    rowList.Add(tile);
                }
                _tiles.Add(rowList);
            }
        }
    }
}
