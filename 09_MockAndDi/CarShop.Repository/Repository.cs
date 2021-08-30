using CarShop.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarShop.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext ctx;
        public Repository(DbContext ctx)
        {
            this.ctx = ctx;
        }
        public IQueryable<T> GetAll()
        {
            return ctx.Set<T>(); // "Set" as a noun, not as a verb!!!
        }

        public abstract T GetOne(int id); // return ctx.Set<T>().Find(id);
    }

    public class CarRepository : Repository<Car>, ICarRepository
    {
        public CarRepository(DbContext ctx) : base(ctx) { }
        public void ChangePrice(int id, int newprice)
        {
            var car = GetOne(id);
            if (car == null) throw new InvalidOperationException("Car not found!");
            car.BasePrice = newprice;
            ctx.SaveChanges(); // Save here or a separated public method ==> Unit of Work pattern ???
        }
        public override Car GetOne(int id)
        {
            return GetAll().SingleOrDefault(x => x.Id == id);
        }
    }

}
