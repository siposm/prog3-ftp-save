using System;
using System.Collections.Generic;

// HANDOUT - this is given to students before lesson
// add reference for this project in all IGameItem implementations
namespace Chaser.Common
{
    // DLL BASE INTERFACE
    public interface IGameItem
    {
        char ItemChar { get; }
        MyPoint ProcessMovement(char[,] map, int your_x, int your_y);
    }

    // EVERYTHING ELSE
    public struct MyPoint
    {
        public int X;
        public int Y;
        public MyPoint(int newx, int newy)
        {
            X = newx; Y = newy;
        }
    }
    public static class MoveDirection
    {
        public static MyPoint Left { get { return new MyPoint(-1, 0); } }
        public static MyPoint Right { get { return new MyPoint(1, 0); } }
        public static MyPoint Up { get { return new MyPoint(0, -1); } }
        public static MyPoint Down { get { return new MyPoint(0, 1); } }
        public static MyPoint None { get { return new MyPoint(0, 0); } }
    }
    public class ItemWithPos
    {
        public IGameItem Item { get; set; }
        public MyPoint Position { get; set; }
    }
    public class Game
    {
        // List of game items
        List<ItemWithPos> items = new List<ItemWithPos>();

        // Map representation as 2D array
        char[,] map;
        // Generate map every time this is read
        public char[,] Map
        {
            get
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        map[x, y] = '-';
                    }
                }
                foreach (var akt in items)
                {
                    map[akt.Position.X, akt.Position.Y] = akt.Item.ItemChar;
                }
                return map;
            }
        }

        public Game(int max_x, int max_y)
        {
            map = new char[max_x, max_y];
        }

        public void AddPlayer(IGameItem item)
        {
            ItemWithPos newItem = new ItemWithPos();
            newItem.Item = item;
            newItem.Position = new MyPoint();
            items.Add(newItem);
        }

        public void OneTick()
        {
            foreach (ItemWithPos akt in items)
            {
                char[,] current = Map;
                int lenX = current.GetLength(0);
                int lenY = current.GetLength(1);
                // call DLL function here!
                MyPoint vect = akt.Item.ProcessMovement(current, akt.Position.X, akt.Position.Y);
                MyPoint newPos = new MyPoint(
                    (lenX + vect.X + akt.Position.X) % lenX,
                    (lenY + vect.Y + akt.Position.Y) % lenY);

                if (current[newPos.X, newPos.Y] == '-')
                {
                    akt.Position = new MyPoint(newPos.X, newPos.Y);
                }
            }
        }

        public override string ToString()
        {
            string s = "";
            char[,] map = Map;
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    s += map[x, y] + " ";
                }
                s += "\n";
            }
            return s;
        }
    }

}
