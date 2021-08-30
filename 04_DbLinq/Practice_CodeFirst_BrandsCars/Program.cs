using Microsoft.EntityFrameworkCore;
using Practice_CodeFirst_BrandsCars.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Practice_CodeFirst_BrandsCars
{
    /*
        Add Database
        Close Connection in Server Explorer !!!
        Manage Nuget Packages for Solution => NOT prerelease
            Microsoft.EntityFrameworkCore.SqlServer
            Microsoft.EntityFrameworkCore.Tools
            Microsoft.EntityFrameworkCore.Proxies 
    */
    static class Extensions
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

    class Program
    {
        static void Main(string[] args)
        {
            CarDbContext ctx = new CarDbContext();
            ctx.Brand.Select(x => x.Name).ToConsole("BRANDS");
            ctx.Cars.Select(x => $"{x.Model} from {x.Brand.Name}").ToConsole("CARS");
            // above is BAD in old EF, because String.Format() cannot be used in LINQ-SQL translation

            var q1 = from car in ctx.Cars
                    group car by car.BrandId into grp
                    select new { BrandId = grp.Key, AvgPrice = grp.Average(x => x.BasePrice) };
            q1.ToConsole("Average Prices");

            var q2 = from car in ctx.Cars.Include(car => car.Brand)
                     group car by new { car.Brand.Id, car.Brand.Name } into grp
                     select new { BrandId = grp.Key.Name, AvgPrice = grp.Average(x => x.BasePrice) };
            q2.ToConsole("Average Prices Again");

            var q3 = from car in ctx.Cars.Include(car => car.Brand)
                     group car by car.Brand into grp
                     select new { BrandId = grp.Key.Name, AvgPrice = grp.Average(x => x.BasePrice) };
            q3.ToConsole("Average Prices - This Doesnt Work!");

        }
    }
}
