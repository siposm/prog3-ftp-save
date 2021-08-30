using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Practice_ProducerConsumer
{
    class WorkitemProcessor
    {
        static Random rnd = new Random();
        Queue<char> characters = new Queue<char>(); // List<T>, Dictionary<T,U>
        // ConcurrentQueue, ConcurrentDictionary, ConcurrentBag
        CancellationTokenSource CTS = new CancellationTokenSource(); // add in step 5

        // thread synchronization! => thread safety => Critical sections! =>  lock {} Monitor.xxx
        object consoleLock = new object(); // add in step 2
        object queueLock = new object(); // add in step 3

        void MyOut(ref int x, ref int y, ConsoleColor color, char c)
        {
            lock (consoleLock) // Monitor.Enter(consoleLock); // add lock in step 2
            {
                Console.CursorTop = y;
                Console.CursorLeft = x;
                Console.ForegroundColor = color;
                Console.Write(c);
            } // Monitor.Exit(consoleLock);

            x++;
            if (x >= Console.WindowWidth)
            {
                x = 0;
                y += 2;
            }
        }
        char ProduceOne()
        {
            char workItem = (char)rnd.Next('A', 'Z' + 1);
            lock (queueLock) // add lock in step 3
            {
                characters.Enqueue(workItem);
                Monitor.Pulse(queueLock); // add in step 4
            }
            return workItem;
        }
        char ConsumeOne()
        {
            char workItem = '_';
            lock (queueLock) // add lock in step 3
            {
                if (characters.Count == 0) Monitor.Wait(queueLock); // add in step 4
                // if (characters.Count > 0) // Starvation - remove in step 4
                {
                    workItem = characters.Dequeue();
                }
            }
            return workItem;
        }
        void ProduceJob()
        {
            int x = 0, y = 0;
            while (!CTS.IsCancellationRequested) // change in step 5 - initially with while (true)
            {
                //CTS.Token.ThrowIfCancellationRequested();
                char c = ProduceOne();
                MyOut(ref x, ref y, ConsoleColor.Red, c);
                Thread.Sleep(rnd.Next(20, 80));
            }
        }
        void ConsumeJob()
        {
            int x = 0, y = 1;
            while (!CTS.IsCancellationRequested) // change in step 5 - initially with while (true)
            {
                char c = ConsumeOne();
                MyOut(ref x, ref y, ConsoleColor.Green, c);
                Task.Delay(rnd.Next(10, 60)).Wait();
            }
        }

        public void Cancel() // add in step 5 - SHOULD use IDisposable.Dispose() 
        {
            CTS.Cancel();
        }

        public void Start()
        {
            // Thread only: .Abort() .Priority ... IsBackground
            // Task only: Result, Parameters, Exceptions, Continuation
            // new Task() , Task.Run() , Task.Factory.StartNew()
            // Task scheduler = ThreadPool
            new Task(ProduceJob, TaskCreationOptions.LongRunning).Start();
            new Task(ConsumeJob, TaskCreationOptions.LongRunning).Start();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // new WorkitemProcessor().ConsumeJob();
            WorkitemProcessor workitems = new WorkitemProcessor();
            workitems.Start();
            Console.ReadLine();
            workitems.Cancel(); // add in step 5
        }
    }

}
