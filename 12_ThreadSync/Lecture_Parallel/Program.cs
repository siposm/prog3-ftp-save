using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lecture_Parallel
{
    // Parallel.For() , Parallel.ForEach(), Parallel.Invoke(), PLINQ
    class Program
    {
        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            double lim = Math.Sqrt(number);
            for (int i = 2; i <= lim; i++) // fixed order => no parallel!
            {
                if (number % i == 0) return false;
            }
            return true;
        }
        static bool Decide(List<int> numbers)
        {
            /*
            foreach (int akt in numbers)
            {
                if (IsPrime(akt)) return true;
            }
            return false;
            */
            // for (int i=0; i<num; i++) {xxxx }
            // Parallel.For(0, num, i => { xxxxx });
            Parallel.ForEach(numbers, item =>
            {
                Console.WriteLine(item);
            });
            Console.ReadLine();

            bool primeFound = false;
            int steps = 0;
            ParallelLoopResult result = Parallel.ForEach(numbers, (item, state) =>
            {
                // steps++;
                /*
                 * int number=0;
                 * number+=1;               number+=2;
                 */
                // lock(xxxx) { steps++; }
                Interlocked.Increment(ref steps);

                Console.WriteLine("CHECKING " + item);
                if (IsPrime(item))
                {
                    primeFound = true;
                    Console.WriteLine("FOUND " + item);
                    state.Break();
                }
            });

            Console.WriteLine("LOWEST BREAK: " + result.LowestBreakIteration);
            Console.WriteLine("STEPS: " + steps);
            return primeFound;
        }
        static void Main(string[] args)
        {
            List<int> numbers = Enumerable.Range(1000000, 400).ToList();
            Console.WriteLine("PRIME INSIDE: " + Decide(numbers));
            Console.ReadLine();

            var primes = from item in numbers.AsParallel()
                         where IsPrime(item)
                         select item;
            foreach (int item in primes) Console.WriteLine("PRIME: " + item);
            Console.ReadLine();
        }
    }

}
