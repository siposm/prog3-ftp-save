using System;

using Chaser.Common; // add reference to chaser.* projects from the solution
using Chaser.Items;
using System.IO;
using System.Reflection;

namespace Chaser.ZProgram
{
    class Program
    {
        // Implement this only after showing how it works with the "Add Reference" way
        static void LoadDLL(Game g)
        {
            string[] plugins = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");
            foreach (string akt in plugins)
            {
                Assembly assembly = Assembly.LoadFile(akt);
                foreach (Type aktType in assembly.GetTypes())
                {
                    if (aktType.GetInterface(nameof(IGameItem)) != null)
                    {
                        Console.WriteLine(">>> Loading plugin: " + aktType.ToString());
                        object instance = Activator.CreateInstance(aktType);
                        //aktType.InvokeMember("InitPlugin", BindingFlags.Default | BindingFlags.InvokeMethod, null, instance, new object[] { core });
                        g.AddPlayer(instance as IGameItem);
                    }
                }
            }
            Console.WriteLine("Press ENTER to start");
            Console.ReadLine();
        }
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Game G = new Game(10, 10);
            // First, add the DLL references to the project, then use these
            // G.AddPlayer(new RandomEnemy());
            // G.AddPlayer(new User());
            // G.AddPlayer(new FollowerEnemy());

            // Second, comment out the three .AddItem() lines, and use dynamic DLL loading
            LoadDLL(G);

            while (true)
            {
                System.Threading.Thread.Sleep(300);
                G.OneTick();
                Console.Clear();
                Console.WriteLine(G.ToString());
            }
        }
    }

}
