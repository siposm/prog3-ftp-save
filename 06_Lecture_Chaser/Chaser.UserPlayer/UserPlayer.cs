using System;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using Chaser.Common;

namespace Chaser.Items
{
    public class User : IGameItem
    {
        Dictionary<ConsoleKey, MyPoint> Movements;

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        public User()
        {
            Movements = new Dictionary<ConsoleKey, MyPoint>();
            Movements.Add(ConsoleKey.W, MoveDirection.Up);
            Movements.Add(ConsoleKey.S, MoveDirection.Down);
            Movements.Add(ConsoleKey.A, MoveDirection.Left);
            Movements.Add(ConsoleKey.D, MoveDirection.Right);
        }

        public char ItemChar
        {
            get { return '*'; }
        }

        public MyPoint ProcessMovement(char[,] map, int your_x, int your_y)
        {
            foreach (var akt in Movements)
            {
                if (GetAsyncKeyState((int)akt.Key) != 0)
                    return akt.Value;
            }
            return MoveDirection.None;
        }
    }

}
