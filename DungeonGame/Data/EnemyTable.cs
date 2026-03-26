using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DungeonGame.Data
{
    public class EnemyData
    {
        public EnemyType Type { get; }
        public string Name { get; }
        public char Symbol { get; }
        public int Demage { get; }
        public int MaxHp { get; }
        public int MoveTurn { get; }

        public EnemyData(EnemyType type, string name, char symbol, int demage, int maxHp, int moveTurn)
        {
            Type = type;
            Name = name;
            Symbol = symbol;
            Demage = demage;
            MaxHp = maxHp;
            MoveTurn = moveTurn;
        }
    }

    internal class EnemyTable
    {
        private List<EnemyData> _rows = new List<EnemyData>();
        public List<EnemyData> Rows => _rows;

        public EnemyTable(List<EnemyData> rows)
        {
            _rows = rows;
        }
    }
}
