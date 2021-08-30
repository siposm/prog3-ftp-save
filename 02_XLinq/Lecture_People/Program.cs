using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Lecture_People
{
    static class MyExtensions
    {
        public static void ToConsole<T>(this IEnumerable<T> input, string str)
        {
            Console.WriteLine("*** BEGIN " + str);
            foreach (T item in input)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine("*** END " + str);
            Console.ReadLine();
        }

    }

    class Person
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Dept { get; set; }
        public string Rank { get; set; }
        public string Phone { get; set; }
        public string Room { get; set; }

        public static Person Parse(XElement node)
        {
            return new Person()
            {
                Name = node.Element("name")?.Value,
                Email = node.Element("email")?.Value,
                Dept = node.Element("dept")?.Value,
                Rank = node.Element("rank")?.Value,
                Phone = node.Element("phone")?.Value,
                Room = node.Element("room")?.Value
            };
        }

        public static IEnumerable<Person> Load(string url) // IEnumerable<X> vs List<X>
        {
            XDocument XDoc = XDocument.Load(url);
            return XDoc.Descendants("person").Select(node => Person.Parse(node));
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            IEnumerable<Person> people = Person.Load("http://users.nik.uni-obuda.hu/prog3/_data/people.xml");
            people.Select(person => person.Name).ToConsole("ALL WORKERS");

            // 1. number of AII workers
            string dept = "Alkalmazott Informatikai Intézet";
            int num = people.Where(person => person.Dept == dept).Count();
            int num2 = people.Count(person => person.Dept == dept);
            Console.WriteLine("Q1");
            Console.WriteLine(num);
            Console.WriteLine(num2);
            Console.ReadLine();

            // 2. paginated list
            int current = 0; int pagesize = 15;
            while (current < num)
            {
                var q2 = people.Where(person => person.Dept == dept).Skip(current).Take(pagesize).Select(person => person.Name);
                q2.ToConsole("Q2 / page");
                current += pagesize;
            }

            // 3. people with the longest/shortest name
            var q3 = from person in people
                     let minlen = people.Min(x => x.Name.Length)
                     let maxlen = people.Max(x => x.Name.Length)
                     where person.Name.Length == minlen || person.Name.Length == maxlen
                     select new { person.Name, person.Name.Length };
            q3.ToConsole("Q3");

            // 4. number of people per department
            var q4 = from person in people
                     group person by person.Dept into g
                     select new { Dept = g.Key, Cnt = g.Count() };
            q4.ToConsole("Q4");

            // 5. biggest dept
            // ElementAt, First, Last, Single, ...OrDefault
            var oneDept = q4.OrderByDescending(rec => rec.Cnt).FirstOrDefault();
            var oneDept_alter = q4.Aggregate((i, j) => i.Cnt > j.Cnt ? i : j);
            Console.WriteLine(oneDept.ToString());
            Console.WriteLine(oneDept_alter.ToString());
            Console.ReadLine();
        }
    }
}
