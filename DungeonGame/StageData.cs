using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class StageData
    {
        // Map 정보
        public int mapRow;
        public int mapCol;
        public int mapWallCount;

        // Enemy 정보
        public int enemyCount;

        public int enemyMinHp;
        public int enemyMaxHp;

        public int enemyMinDemage;
        public int enemyMaxDemage;

        public StageData(int mapRow, int mapCol, int mapWallCount, int enemyCount, int enemyMinHp, int enemyMaxHp, int enemyMinDemage, int enemyMaxDemage)
        {
            this.mapRow = mapRow;
            this.mapCol = mapCol;
            this.mapWallCount = mapWallCount;
            this.enemyCount = enemyCount;
            this.enemyMinHp = enemyMinHp;
            this.enemyMaxHp = enemyMaxHp;
            this.enemyMinDemage = enemyMinDemage;
            this.enemyMaxDemage = enemyMaxDemage;
        }
    }
}
