using Lecture_SqlFirst_EmpDept.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Lecture_SqlFirst_EmpDept
{
    class Program
    {
        // Manage Nuget Packages for Solution => NOT prerelease
        //      Microsoft.EntityFrameworkCore.SqlServer
        //      Microsoft.EntityFrameworkCore.Tools
        //      Microsoft.EntityFrameworkCore.Proxies 

        // Add Service-Based Database: EmpDept.mdf
        //      MDF + LDF => Content, Copy Always !!!
        //      MDF + LDF should also go to GIT => comment out in .gitignore !!!
        // Server Explorer => EmpDept => Right click => Properties
        //      "Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = |DataDirectory|\EmpDept.mdf; Integrated Security = True"
        //      Fill with data, if using SQL First

        // ALWAYS Close Connection in Server Explorer !!!

        static void Main(string[] args)
        {
            // Nuget Package Manager Console
            // Scaffold-DbContext "CONNECTION_STRING" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
            // Optional: -DataAnnotations

            // If it fails:
            // dotnet tool install --global dotnet-ef
            // dotnet ef dbcontext scaffold "CONNECTIONSTRING" Microsoft.EntityFrameworkCore.SqlServer -o Models
            // optional: --data-annotations

            EmpDeptContext ED = new EmpDeptContext();
            // ED.Database.Log = Console.WriteLine;
            // Only in EF Core 5: optionsBuilder.LogTo(Console.WriteLine)

            // Q2 Workers + Locations
            // Eager Loading
            /*
            foreach (var item in ED.Emp.Include(emp => emp.DeptnoNavigation))
            {
                Console.WriteLine($"{item.Ename} in {item.DeptnoNavigation.Loc}");
            }
            */

            // Lazy Loading
            // 1) add .UseLazyLoadingProxies() in the DbContext
            // 2a) Must use .ToList() 
            // 2b) or "MultipleActiveResultSets = true" in the connection string
            foreach (var item in ED.Emp)
            {
                Console.WriteLine($"{item.Ename} in {item.DeptnoNavigation.Loc}");
            }
            Console.ReadLine();
            

            // Q3 Workers + Average job incomes (=salary+commission)
            var q3_averages = from worker in ED.Emp
                              group worker by worker.Job into g
                              select new
                              {
                                  Job = g.Key,
                                  //Avg = g.Average(x => x.Sal + x.Comm)
                                  //Avg = g.Average(x => x.Sal + x.Comm.GetValueOrDefault())
                                  Avg = g.Average(x => x.Sal + (x.Comm ?? 0))
                              };
            var q3_workers = from worker in ED.Emp
                             join average in q3_averages on worker.Job equals average.Job
                             select new { worker.Ename, worker.Sal, worker.Job, average.Avg };
            foreach (var item in q3_workers) Console.WriteLine(item);
            // old EF: .ToString() / .ToTraceString()
            // EF core 5: ToQueryString
            // two LINQ queries ==> one SQL query !!!
            Console.WriteLine(q3_workers.ToString());
            Console.ReadLine();

            // Q4 Biggest dept
            var q4_counts = from worker in ED.Emp
                            group worker by worker.Deptno into g
                            orderby g.Count() descending
                            select new { Count = g.Count(), Dept = g.Key };
            var oneDept = q4_counts.FirstOrDefault();
            Console.WriteLine($"{oneDept.Count} workers in {oneDept.Dept}");
            Console.ReadLine();

            var q4_join = from worker in ED.Emp
                          join stat in q4_counts on worker.Deptno equals stat.Dept
                          select new { worker.Ename, worker.DeptnoNavigation.Dname, stat.Count };
            foreach (var item in q4_join) Console.WriteLine(item);
            Console.ReadLine();

            // Q5 double King's salary
            var boss = ED.Emp.Single(x => x.Job == "PRESIDENT");
            Console.WriteLine(boss.Sal);
            boss.Sal = boss.Sal * 2;
            ED.SaveChanges();
            var king = ED.Emp.Single(x => x.Ename == "KING");
            Console.WriteLine(king.Sal);
            Console.ReadLine();

            // System.Data.Entity.Core.Objects.EntityFunctions old, deprecated
            // System.Data.Entity.DbFunctions new

            // Q6 Delete those who joined less than 30 days after the president
            var president_date = (from worker in ED.Emp
                                  where worker.Job == "PRESIDENT"
                                  select worker.Hiredate.Value).FirstOrDefault();
            var q6 = from worker in ED.Emp
                         where worker.Hiredate <= president_date.AddDays(30) &&
                         worker.Hiredate > president_date
                         select worker;
            // Old EF: DbFunctions.AddDays(president_date, 30)
            // DbFunctions => Raw SQL in EF Core
            Console.WriteLine("ALL PEOPLE OLD: " + string.Join(";", ED.Emp.Select(x => x.Ename)));
            Console.WriteLine("PEOPLE TO DELETE: " + string.Join(";", q6.Select(x => x.Ename)));
            Console.ReadLine();
            
            // Removing...
            // Try commenting out the Q2 part => crashes!
            // .OnDelete(DeleteBehavior.SetNull) 
            foreach (var worker in q6) ED.Emp.Remove(worker);
            ED.SaveChanges();
            Console.WriteLine("ALL PEOPLE NEW: " + string.Join(";", ED.Emp.Select(x => x.Ename)));
            Console.ReadLine();
            // CRUD = Create, Read, Update, Delete

        }
    }
}
