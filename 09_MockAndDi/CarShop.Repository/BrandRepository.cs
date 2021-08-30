using CarShop.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarShop.Repository
{
    public interface IBrandRepository : IRepository<Brand>
    {
        public int Add(string name);
        // Should have all CRUD in all Repositories!
    }
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(DbContext ctx) : base(ctx)
        {
        }
        public int Add(string name)
        {
            Brand brand = new Brand();
            brand.Name = name;
            ctx.Set<Brand>().Add(brand);
            ctx.SaveChanges();
            return brand.Id; // NON zero!
        }
        public override Brand GetOne(int id)
        {
            return GetAll().SingleOrDefault(x => x.Id == id);
        }
    }
}
