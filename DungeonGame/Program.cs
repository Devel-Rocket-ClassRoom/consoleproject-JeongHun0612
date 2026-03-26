using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DungeonGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameManager gameManager = GameManager.Instance;
            gameManager.StartGame();
        }
    }
}
