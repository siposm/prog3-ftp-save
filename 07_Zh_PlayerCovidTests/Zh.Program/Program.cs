using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Xml.Linq;
using Zh.Db;
using Zh.Utils;

namespace Zh.Program
{
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
        static void FillDatabase(PlayerContext ctx)
        {
            TeamGenerator team = new TeamGenerator();
            CovidTester tester = new CovidTester();
            var teamXml = team.GetTeam(10);
            Console.WriteLine(teamXml);
            Console.ReadLine();
            foreach (var node in teamXml.Descendants("player"))
            {
                Player player = new Player(node);
                ctx.Players.Add(player);
            }
            ctx.SaveChanges();

            for (int i = 0; i < 10; i++)
            {
                var results = TestExecutor.ExecuteTests(tester, ctx.Players);
                foreach (var item in results)
                {
                    CovidTest test = new CovidTest();
                    test.Date = DateTime.Now.Date.AddDays(i*5);
                    test.IsPositive = item.Value;
                    test.PlayerId = item.Key;
                    ctx.CovidTests.Add(test);
                }
                ctx.SaveChanges();
            }
        }

        private static void QueryDatabase(PlayerContext ctx)
        {
            Console.WriteLine("PLAYERS: " + ctx.Players.Count());
            Console.WriteLine("TESTS: " + ctx.CovidTests.Count());
            Console.ReadLine();

            var q1 = from test in ctx.CovidTests
                     group test by test.IsPositive into grp
                     select new { IsPositive = grp.Key, Number = grp.Count() };
            q1.ToConsole("Q1");

            var q2 = from test in ctx.CovidTests
                     where test.IsPositive
                     group test by new { test.Player.Position } into grp
                     select new { Position = grp.Key.Position, Number = grp.Count() };
            q2.ToConsole("Q2");

            var q3 = from test in ctx.CovidTests
                     join player in ctx.Players on test.PlayerId equals player.Id
                     select new { testId=test.Id, test.Date, test.IsPositive, player.Code };
            q3.ToConsole("Q3");
        }

        static void Main(string[] args)
        {
            PlayerContext ctx = new PlayerContext();
            FillDatabase(ctx);
            QueryDatabase(ctx);
        }

    }
}
