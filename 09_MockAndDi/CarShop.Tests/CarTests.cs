using CarShop.Data;
using CarShop.Logic;
using CarShop.Repository;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarShop.Tests
{
    // NuGet: NUnit, NUnit3TestAdapter, Moq, Microsoft.NET.Test.Sdk
    // Project: Logic, Repository, Data
    // Split up to multiple classes!
    // 5x CRUD: Insert, Remove, Update, GetOne, GetAll
    // 3x Non-Crud
    // NO real database usage! Console.*! File I/O! Network....

    [TestFixture]
    public class LogicTests
    {
        // FAKE vs MOCK
        [Test] // Crud test: Filter
        public void TestGetByBrand()
        {
            // ARRANGE => refactor to method/factory class?
            Mock<ICarRepository> mockedRepo = new Mock<ICarRepository>(MockBehavior.Loose);
            // method exists: mockedRepo.Object.GetAll()!
            // NEVER ACCESS mockedRepo.Object.XXX !!!

            List<Car> cars = new List<Car>()
            {
                new Car() { Model="Suzuki Swift", BrandId = 1 },
                new Car() { Model="BMW 116d", BrandId = 2 },
                new Car() { Model="BMW i8", BrandId = 2 },
                new Car() { Model="Audi A4", BrandId = 3 }
            };
            List<Car> expectedBmws = new List<Car>() { cars[1], cars[2] };
            // Use same-instance lists! If not, ovveride equals+GetHashCode
            /*
            List<Car> expectedBmws = new List<Car>() {
                new Car() { Model="BMW 116d", BrandId = 2 },
                new Car() { Model="BMW i8", BrandId = 2 },
            };
            */

            // statement lambda vs expression lambda
            mockedRepo.Setup(repo => repo.GetAll()).Returns(cars.AsQueryable());
            CarLogic logic = new CarLogic(mockedRepo.Object);

            // ACT
            var result = logic.GetCarsByBrand(2);

            // ASSERT
            Assert.That(result.Count(), Is.EqualTo(expectedBmws.Count));
            // Assert.That(result.Select(x => x.Model), Does.Contain("BMW 116d"));
            // Assert.That(result.Select(x => x.Model), Does.Contain("BMW i8"));
            // Rather use this:
            Assert.That(result, Is.EquivalentTo(expectedBmws));
            // EquivalentTo() vs EqualTo() 
            // Add Car.ToString(), Car.Equals(), Car.GetHashCode() !!!

            mockedRepo.Verify(repo => repo.GetAll(), Times.Once);
            mockedRepo.Verify(repo => repo.GetOne(42), Times.Exactly(0));
            mockedRepo.Verify(repo => repo.GetOne(It.IsAny<int>()), Times.Never);
        }

        [Test] // Crud Test: Add
        public void TestBrandAdd()
        {
            Mock<IBrandRepository> brandRepo = new Mock<IBrandRepository>();
            Mock<ICarRepository> carRepo = new Mock<ICarRepository>();
            brandRepo.Setup(repo => repo.Add(It.IsAny<string>())).Returns(42);
            CarLogic logic = new CarLogic(carRepo.Object, brandRepo.Object);

            int idNumber = logic.AddBrand("Suzuki");
            Assert.That(idNumber, Is.EqualTo(42));
            brandRepo.Verify(repo => repo.Add(It.IsAny<string>()), Times.Once);
            brandRepo.Verify(repo => repo.Add("Suzuki"), Times.Once);
        }

        Mock<ICarRepository> carRepo;
        Mock<IBrandRepository> brandRepo;
        List<AveragesResult> expectedAverages;

        private CarLogic CreateLogicWithMocks()
        {
            carRepo = new Mock<ICarRepository>();
            brandRepo = new Mock<IBrandRepository>();

            Brand bmw = new Brand() { Id = 1, Name = "BMW" };
            Brand audi = new Brand() { Id = 2, Name = "Audi" };
            List<Brand> brands = new List<Brand>() { bmw, audi };
            List<Car> cars = new List<Car>()
            {
                new Car() { Brand = bmw, BrandId=bmw.Id, BasePrice = 6000 },
                new Car() { Brand = bmw, BrandId=bmw.Id, BasePrice = 4000 },
                new Car() { Brand = audi, BrandId=audi.Id, BasePrice = 2500},
            };
            expectedAverages = new List<AveragesResult>()
            {
                new AveragesResult() { BrandName="BMW", AveragePrice=5000 },
                new AveragesResult() { BrandName="Audi", AveragePrice=2500 }
            };

            carRepo.Setup(repo => repo.GetAll()).Returns(cars.AsQueryable());
            brandRepo.Setup(repo => repo.GetAll()).Returns(brands.AsQueryable());
            return new CarLogic(carRepo.Object, brandRepo.Object);
        }

        [Test] // Noncrud test
        public void TestGetAverages()
        {
            var logic = CreateLogicWithMocks();
            var actualAverages = logic.GetBrandAverages();
            
            // MUST HAVE .EQUALS!!!
            Assert.That(actualAverages, Is.EquivalentTo(expectedAverages));
            carRepo.Verify(repo => repo.GetAll(), Times.Exactly(1));
            brandRepo.Verify(repo => repo.GetAll(), Times.Never);
        }

        [Test] // Noncrud test
        public void TestGetAveragesJoin()
        {
            var logic = CreateLogicWithMocks();
            var actualAverages = logic.GetBrandAveragesJoin();

            Assert.That(actualAverages, Is.EquivalentTo(expectedAverages));
            carRepo.Verify(repo => repo.GetAll(), Times.Once);
            brandRepo.Verify(repo => repo.GetAll(), Times.Once);
        }
    }
}
