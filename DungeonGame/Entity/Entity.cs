using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal abstract class Entity
    {
        protected string _name;
        protected char _symbol;
        protected Pos _pos;
        protected int _hp;
        protected int _maxHp;
        protected int _demage;

        public string Name => _name;
        public char Symbol => _symbol;
        public Pos Pos => _pos;
        public int Hp => _hp;
        public int MaxHp => _maxHp;
        public int Demage => _demage;
        public bool IsDead => _hp <= 0;

        public Entity(string name, char symbol, int demage, int maxHp)
        {
            _name = name;
            _symbol = symbol;
            _pos = new Pos();
            _demage = demage;
            _maxHp = maxHp;
            _hp = maxHp;
        }

        public virtual void MoveTo(Pos pos)
        {
            _pos = pos;
        }

        public virtual void TakeDemage(int demage)
        {
            _hp -= demage;
        }

        public abstract void SetStartPos(Room room);
    }
}
