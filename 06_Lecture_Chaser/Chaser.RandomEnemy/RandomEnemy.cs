using Chaser.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chaser.Items
{
    public class RandomEnemy : IGameItem
    {
        static Random R = new Random();
        public char ItemChar
        {
            get { return '+'; }
        }
        public MyPoint ProcessMovement(char[,] map, int your_x, int your_y)
        {
            return new MyPoint(R.Next(-1, 2), R.Next(-1, 2));
        }
    }

}
