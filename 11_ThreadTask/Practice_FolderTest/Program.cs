using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Practice_FolderTest
{
    class Measurement
    {
        public string DirectoryName { get; set; }
        public int NumberOfDirs { get; set; }
        public int NumberOfFiles { get; set; }
        public float SizeOfFiles { get; set; }
    }

    class Program
    {
        // static CancellationTokenSource cts = new CancellationTokenSource();

        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task<Measurement>> tasks = new List<Task<Measurement>>();

            //string[] dirs = { @"c:\anon\bmf", @"c:\", @"c:\windows" };
            string[] dirs = { @"c:\anon\bmf", @"c:\anon\_KEPEK" };
            for (int i = 0; i < dirs.Length; i++)
            {
                int tmpIndex = i;
                tasks.Add(Task.Run(() => ProcessDirectory(dirs[tmpIndex], cts.Token), cts.Token));
            }
            Task.WhenAll(tasks).ContinueWith(prevTask =>
            {
                Console.WriteLine("ALL Tasks Terminated");
                if (prevTask.IsCompletedSuccessfully) foreach (var item in prevTask.Result)
                {
                    Console.WriteLine($"FINISHED | {item.DirectoryName}: {item.NumberOfDirs} dirs, {item.NumberOfFiles} files,  {item.SizeOfFiles} MB");
                }
                Console.WriteLine("Is Cancelled: "+cts.IsCancellationRequested);
            });
            // , TaskContinuationOptions.OnlyOnRanToCompletion
            // , TaskContinuationOptions.OnlyOnCanceled
            // , TaskContinuationOptions.OnlyOnFaulted

            Console.ReadLine();
            cts.Cancel();
            Console.WriteLine("EXIT!");
            Console.ReadLine();
        }


        static Measurement ProcessDirectory(string path, CancellationToken ct)
        {
            //System.IO.Directory.EnumerateFileSystemEntries(path, "*.*", System.IO.SearchOption.AllDirectories)

            Queue<string> Q = new Queue<string>();
            Measurement m = new Measurement() { DirectoryName = path };
            Q.Enqueue(path);
            while (Q.Count > 0)
            {
                // if (ct.IsCancellationRequested) break;
                ct.ThrowIfCancellationRequested();
                string dir = Q.Dequeue();
                try
                {
                    foreach (var d in Directory.GetDirectories(dir))
                    {
                        m.NumberOfDirs++;
                        Q.Enqueue(d);
                    }
                    foreach (var f in Directory.GetFiles(dir))
                    {
                        m.NumberOfFiles++;
                        m.SizeOfFiles += new FileInfo(f).Length / (1024f * 1024f);
                    }
                }
                catch (Exception) { } // IOException / UnauthorizedAccessException ???
                Console.WriteLine($"{path}: {m.NumberOfDirs} dirs / {m.SizeOfFiles} MB");
            }
            return m;
        }
    }

}
