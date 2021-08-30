using CarShop.Data;
using CarShop.Logic;
using CarShop.Repository;
using ConsoleTools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarShop.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            CarDbContext ctx = new CarDbContext();
            Console.WriteLine(ctx.Cars.Count());
            Console.ReadLine();

            AveragesUsingLinq(ctx);
            // NOT testable, NOT layered...
            // SNIP 1

            // Should extract the creations into a factory class!!!
            // Must use same ctx for ALL repositories
            CarRepository realRepository = new CarRepository(ctx);  
            CarLogic logic = new CarLogic(realRepository);
            // In tests: CarLogic logic = new CarLogic(MockedRepository.Object); !!
            AveragesUsingLogic(logic);

            // SNIP 2
            var menu = new ConsoleMenu().
                Add("Using Linq", () => AveragesUsingLinq(ctx)).
                Add("Using Logic", () => AveragesUsingLogic(logic)).
                Add("Close", ConsoleMenu.Close);
            menu.Show();

            // SNIP 3
            BrandRepository brandRepository = new BrandRepository(ctx); // Same ctx!
            logic = new CarLogic(realRepository, brandRepository);

            foreach (var item in logic.GetBrandAveragesJoin()) Console.WriteLine(item);
            Console.ReadLine();

            int num = logic.AddBrand("Suzuki");
            Console.WriteLine("Suzuki added as " + num);
            foreach (var brand in logic.GetAllBrands())
            {
                Console.WriteLine($"Brand #{brand.Id}: {brand.Name}");
            }
            Console.ReadLine();
        }

        private static void AveragesUsingLogic(CarLogic logic)
        {
            foreach (var item in logic.GetBrandAverages()) Console.WriteLine(item);
            Console.ReadLine();
        }

        static void AveragesUsingLinq(CarDbContext ctx)
        {
            var q = from car in ctx.Cars.Include(car => car.Brand)
                    group car by new { car.Brand.Id, car.Brand.Name } into grp
                    select new { BrandId = grp.Key.Name, AvgPrice = grp.Average(x => x.BasePrice) };
            foreach (var item in q) Console.WriteLine(item);
            Console.ReadLine();
        }

    }
}
