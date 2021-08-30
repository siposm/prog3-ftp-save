using System;
using System.Linq;

namespace Zh.Utils
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CovidTestMethodAttribute : Attribute 
    {
    }
    public class CovidTester
    {
        static Random rnd = new Random();

        [CovidTestMethod]
        public bool PcrTester(string input)
        {
            int num = 3 * (input.ToUpper().Count(x => x == 'E') + input.ToUpper().Count(x => x == 'T'));
            return rnd.Next(num) == 0;
        }

        [CovidTestMethod]
        public bool AntibodyTester(string input)
        {
            int num = 5 * (input.ToUpper().Count(x => x == 'A') + input.ToUpper().Count(x => x == 'S'));
            return rnd.Next(num) == 0;
        }
    }
}
