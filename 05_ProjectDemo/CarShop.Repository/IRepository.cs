using CarShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarShop.Repository
{
    public interface IRepository<T> where T : class
    {
        T GetOne(int id);
        IQueryable<T> GetAll();
        // void Insert(T entity);
        // bool Remove(T entity / int id)
        // !!NOT!! Update(xxxx)
    }
    public interface ICarRepository : IRepository<Car>
    {
        void ChangePrice(int id, int newprice);
        // Must have something changeable in every repository
        // Super-Generic "change anything" methods: not recommended
        // Methods that require/return string[] or object[] data: FORBIDDEN!
    }

}
