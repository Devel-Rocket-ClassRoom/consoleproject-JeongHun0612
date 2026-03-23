using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Map
    {
        public static char C_FLOOR = ' ';
        public static char C_WALL = '#';
        public static char C_PLAYER = 'P';
        public static char C_ENEMY = 'M';
        public static char C_Door = '-';
        public static char C_Stair = 'S';

        char[,] _map;
        int _row;
        int _col;

        public int Row => _row;
        public int Col => _col;

        public void CreateMap(int row, int col, int wallCount)
        {
            _row = row;
            _col = col;

            _map = new char[row, col];

            // 외각선은 벽으로 할당
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if (r == 0 || r == row - 1 || c == 0 || c == col - 1)
                    {
                        SetTile(r, c, C_WALL);
                    }
                    else
                    {
                        SetTile(r, c, C_FLOOR);
                    }
                }
            }

            // 랜덤한 벽 생성
            int createWallCount = 0;
            Random rnd = new Random();
            while (createWallCount < wallCount)
            {
                int rndRow = rnd.Next(0, row);
                int rndCol = rnd.Next(0, col);

                if (GetTile(rndRow, rndCol) == C_FLOOR)
                {
                    SetTile(rndRow, rndCol, C_WALL);
                    createWallCount++;
                }
            }
        }

        public void PrintMap()
        {
            Console.SetCursorPosition(0, 0);

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

        public void CreateRandomRoom(int roomCount)
        {
            Random rnd = new Random();

            // 10, 25

            // 첫번째 룸 생성
            Room room = new Room();



            // 랜덤한 4방향 중 2개 선택

            // 플레이가 존재하는 첫번째 방에는 몬스터 생성 X

            // Index 0번째 방이 시작지점

            // Index 마지막 방이 도착지점 (계단 생성)

            // 테스트용 푸시

            //ㄴㅁㅇㅁㄴ
        }

        public char GetTile(int row, int col)
        {
            return _map[row, col];
        }

        public char GetTile(Pos pos)
        {
            return _map[pos.row, pos.col];
        }

        public void SetTile(int row, int col, char symbol)
        {
            _map[row, col] = symbol;
        }

        public void SetTile(Pos pos, char symbol)
        {
            _map[pos.row, pos.col] = symbol;
        }

        public bool IsWalkable(int row, int col)
        {
            return _map[row, col] == C_FLOOR;
        }

        public bool IsWalkable(Pos pos)
        {
            return _map[pos.row, pos.col] == C_FLOOR;
        }
    }
}
