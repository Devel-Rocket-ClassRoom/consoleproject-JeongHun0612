using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Player : Entity
    {
        public bool HasKey { get; set; }

        private RenderManager _renderManager;

        public Player(string name, int demage, int maxHp) : base(name, 'P', demage, maxHp)
        {
        }

        public void Reset()
        {
            _hp = _maxHp;
            HasKey = false;
        }

        public void Action(Room currentRoom)
        {
            if (_renderManager == null)
                _renderManager = GameManager.Instance.RenderManager;

            Pos nextPos = Move();

            if (!nextPos.IsValid() || !currentRoom.IsInBound(nextPos))
            {
                _renderManager.DrawText(PanelType.Log, 0, 0, $"해당 위치로는 이동할 수 없습니다.");
                return;
            }

            // 다음 위치에 몬스터가 존재하면 공격
            foreach (var enemy in currentRoom.Enemies)
            {
                if (nextPos.IsEqual(enemy.Pos))
                {
                    enemy.TakeDemage(_demage);

                    if (enemy.IsDead)
                    {
                        _renderManager.ClearPanel(PanelType.Status);
                        _renderManager.DrawText(PanelType.Log, 0, 0, $"{enemy.Name}을(를) 제거 하였습니다.");
                        currentRoom.RemoveEnemy(enemy);
                    }

                    return;
                }
            }

            //Tile tile = currentRoom.GetTile(nextPos.Row, nextPos.Col);
            //if (tile.IsWalkable())
            //{
            //    // 플레이어 위치 이동
            //    _player.MoveTo(nextPos);

            //    _renderManager.ClearPanel(PanelType.Log);
            //    _renderManager.DrawText(PanelType.Log, 0, 0, $"{_player.Name} 이동 - [{_player.Pos.Row}, {_player.Pos.Col}]");
            //}
            //else if (tile.Type == TileType.Door)
            //{
            //    Door door = _map.CurrentRoom.GetDoor(nextPos);
            //    _map.ChangeRoom(door.TargetRoomId);

            //    _player.MoveTo(door.TargetSpawnPos);

            //    _renderManager.ClearPanel(PanelType.Status);
            //    _renderManager.ClearPanel(PanelType.Map);
            //}
        }

        public Pos Move()
        {
            Console.SetCursorPosition(0, 0);

            ConsoleKeyInfo key = Console.ReadKey();

            int row = _pos.Row;
            int col = _pos.Col;

            switch (key.Key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    return new Pos(row - 1, col);
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    return new Pos(row + 1, col);
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    return new Pos(row, col - 1);
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    return new Pos(row, col + 1);
                default:
                    return new Pos();
            }
        }

        public override void SetStartPos(Room room)
        {
            // 전달받은 룸의 중앙 위치
            int halfHeight = room.Height / 2;
            int halfWidth = room.Width / 2;

            _pos = new Pos(halfHeight, halfWidth);
        }
    }
}
