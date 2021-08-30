using Chaser.Common;
using System;

namespace Chaser.Items
{
    public class FollowerEnemy : IGameItem
    {
        public char ItemChar
        {
            get { return 'x'; }
        }
        public MyPoint ProcessMovement(char[,] map, int your_x, int your_y)
        {
            int len_x = map.GetLength(0);
            int len_y = map.GetLength(1);
            int dx = 0, dy = 0;
            for (int x = 0; x < len_x; x++)
            {
                for (int y = 0; y < len_y; y++)
                {
                    if (map[x, y] == '*')
                    {
                        dx = x - your_x; dy = y - your_y;
                        if (dx < 0) return MoveDirection.Left;
                        if (dx > 0) return MoveDirection.Right;
                        if (dy < 0) return MoveDirection.Up;
                        if (dy > 0) return MoveDirection.Down;
                    }
                }
            }
            return MoveDirection.None;
        }
    }
}
