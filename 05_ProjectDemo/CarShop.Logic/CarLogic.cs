﻿using CarShop.Data;
using CarShop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarShop.Logic
{
    public class AveragesResult
    {
        public string BrandName { get; set; }
        public double AveragePrice { get; set; }
        public override string ToString()
        {
            return $"BRAND = {BrandName}, AVG = {AveragePrice}";
        }
        public override bool Equals(object obj)
        {
            if (obj is AveragesResult)
            {
                AveragesResult other = obj as AveragesResult;
                return this.BrandName == other.BrandName &&
                    this.AveragePrice == other.AveragePrice;

                // maybe: replace == with "close enough" check

                // maybe: reflection, SLOW
                /*
                var properties = this.GetType().GetProperties();
                var thisValues = properties.Select(x => x.GetValue(this));
                var otherValues = properties.Select(x => x.GetValue(obj));
                return thisValues.SequenceEqual(otherValues);
                */
            }
            else return false;
        }
        public override int GetHashCode()
        {
            return 0; // BrandName.GetHashCode() + AveragePrice.GetHashCode();
        }

    }
    // Should add DTO instead of Entities => SKIP for this semester
    // This is a SERIOUS security hole!
    // var car = logic.GetOneCar(40);
    // car.Model = "NEW NAME"; // Shouldn't be able to do this.
    // logic.ChangeCarPrice(40, car.car_baseprice); // saves new model name too!!!

    public interface ICarLogic
    // Avoid: GOD OBJECT, too many responsibilities!
    {
        Car GetOneCar(int id);
        void ChangeCarPrice(int id, int newprice);
        IList<Car> GetAllCars();
        IList<AveragesResult> GetBrandAverages();
    }
    public class CarLogic : ICarLogic
    {
        ICarRepository carRepo;

        public CarLogic(ICarRepository repo)
        // Dependency Injection => THE ONLY RIGHT WAY
        {
            this.carRepo = repo;
        }

        public void ChangeCarPrice(int id, int newprice)
        {
            carRepo.ChangePrice(id, newprice);
        }
        public IList<Car> GetAllCars()
        {
            return carRepo.GetAll().ToList();
        }
        public IList<AveragesResult> GetBrandAverages()
        {
            var q = from car in carRepo.GetAll()
                    group car by new { car.Brand.Id, car.Brand.Name } into grp
                    select new AveragesResult()
                    {
                        BrandName = grp.Key.Name,
                        AveragePrice = grp.Average(car => car.BasePrice) ?? 0
                    };
            return q.ToList();
        }
        public Car GetOneCar(int id)
        {
            return carRepo.GetOne(id);
        }
    }

}
