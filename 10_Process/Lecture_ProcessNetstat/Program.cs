using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProcessNetstat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("PROCESSES:\n\t");
            Console.WriteLine(String.Join("\n\t", 
                Process.GetProcesses().Select(x => x.ProcessName).OrderBy(x=>x.ToUpper())));

            Console.Write("Process Name? ");
            string processName = Console.ReadLine();

            var processes = Process.GetProcessesByName(processName);
            var pids = processes.Select(process => process.Id.ToString());

            // Process.Start("netstat.exe", "-no");
            Process netStatProcess = new Process();
            netStatProcess.StartInfo.FileName = "netstat.exe";
            netStatProcess.StartInfo.Arguments = "-no";
            netStatProcess.StartInfo.UseShellExecute = false;
            netStatProcess.StartInfo.RedirectStandardOutput = true;
            netStatProcess.StartInfo.RedirectStandardError = true;
            netStatProcess.StartInfo.CreateNoWindow = true;
            netStatProcess.Start();

            // netStatProcess.StandardError.ReadLine(); // Blocking
            // netStatProcess.StandardOutput.ReadLine(); // Blocking
            // netStatProcess.BeginOutputReadLine(); // Start async loop
            // netStatProcess.BeginErrorReadLine(); // Start async loop
            // StringBuilder sb = new StringBuilder();
            // netStatProcess.ErrorDataReceived += (sender, e) => sb.Append(e.Data); // Non-Blocking
            // netStatProcess.OutputDataReceived += (sender, e) => sb.Append(e.Data); // Non-Blocking

            // netStatProcess.WaitForExit(); // possible DEADLOCK - output buffer full!
            string results = netStatProcess.StandardOutput.ReadToEnd(); // Blocking
            netStatProcess.WaitForExit(); // Blocking
            // Console.WriteLine(results);

            string[] lines = results.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var networkConnections =
                from x in lines
                let cols = x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                where cols.Count() == 5 && pids.Contains(cols[4])
                select new { proto = cols[0], local = cols[1], remote = cols[2] };

            Console.WriteLine("Open network connections: ");
            foreach (var networkConnection in networkConnections)
                Console.WriteLine(networkConnection);

            Console.ReadLine();
        }
    }
}