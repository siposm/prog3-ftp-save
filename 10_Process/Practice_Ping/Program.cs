using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Practice_Ping
{
    class Program
    {
        static Dictionary<string, string> processOutputs = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            List<string> domainList = new List<string>() { "telex.hu", "cnn.com", "amazon.de" };
            List<Process> processList = new List<Process>();
            foreach (string domain in domainList)
            {
                Process p = new Process()
                {
                    StartInfo = new ProcessStartInfo("ping", domain)
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                p.EnableRaisingEvents = true;
                p.Exited += P_Exited;
                processList.Add(p);
                p.Start();
                Console.WriteLine(">>> Running for: " + domain);
                System.Threading.Thread.Sleep(1000);
            }
            
            // foreach (Process p in processList) p.WaitForExit();
            while (processList.Any(p => !p.HasExited))
            {
                Console.WriteLine();
                foreach (Process p in processList)
                {
                    Console.WriteLine($"Status for {p.StartInfo.Arguments}: {p.HasExited}");
                }
                System.Threading.Thread.Sleep(500);
            }

            Console.WriteLine("All processes terminated!");
            foreach(var item in processOutputs)
            {
                Console.WriteLine($"*** {item.Key}\n{item.Value}");
            }

            Console.ReadLine();
            // Todo: Process string output 
        }

        private static void P_Exited(object sender, EventArgs e)
        {
            Process p = sender as Process;
            processOutputs.Add(p.StartInfo.Arguments, p.StandardOutput.ReadToEnd());
                 // Deadlock???
        }

    }
}
