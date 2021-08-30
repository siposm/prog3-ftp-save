using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Practice_Downloader
{
    class Measurement
    {
        public string Address { get; set; }
        public int Bytes { get; set; }
        public int Miliseconds { get; set; }
        public double Speed { get { return (double)Bytes / Miliseconds; } }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Measurement[] T = new Measurement[] {
                new Measurement() { Address = "http://bing.com" },
                new Measurement() { Address = "https://google.com" },
                new Measurement() { Address = "https://users.nik.uni-obuda.hu" },
                new Measurement() { Address = "https://mail.ru" },
            };

            Task[] ts = new Task[T.Length];
            for (int i = 0; i < T.Length; i++)
            {
                // OUTER VARIABLE TRAP with T[i]
                int localIdx = i;
                ts[i] = new Task(() => ExecuteTest(T[localIdx], localIdx));
                ts[i].Start();
            }

            while (ts.Any(task => !task.IsCompleted))
            {
                Console.WriteLine();
                for (int i = 0; i < T.Length; i++)
                {
                    Console.WriteLine($"Waiting for {T[i].Address}; IsCompleted: {ts[i].IsCompleted}");
                    // ts[i].Wait();
                }
                Task.Delay(1000).Wait();
            }

            Console.WriteLine();
            Console.WriteLine();
            for (int i = 0; i < T.Length; i++)
            {
                Console.WriteLine($"{T[i].Address}: {T[i].Bytes} Bytes, {T[i].Miliseconds}ms, {T[i].Speed} kB/s");
            }

            Console.ReadLine();
        }

        static void ExecuteTest(Measurement m, int idx)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            for (int i = 0; i < 10; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                m.Bytes += wc.DownloadString(m.Address).Length;
                sw.Stop();
                m.Miliseconds += (int)sw.ElapsedMilliseconds;
                Task.Delay(100).Wait();
                Console.Write($"|{idx}|");
            }
        }
    }


}
