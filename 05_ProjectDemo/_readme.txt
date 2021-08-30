1. CarShop.Data
	Take code from previous lesson, rename project and folder
	Change project type to Class Library
	Remove Program.cs
	Change namespaces of all classes to CarShop.Data
2. CarShop.Program
	Add reference to .Data
	Write code until "SNIP 1"
3. Add DLL projects: CarShop.Repository, CarShop.Logic, CarShop.Tests
	Remove Class1.cs
4. CarShop.Repository
	Add reference to .Data
	Write IRepository, ICarRepository, Repository, CarRepository
4. CarShop.Logic
	Add reference to .Data + .Repo
	Write ICarLogic, CarLogic
5. CarShop.Tests
	Nothing now, LATER
6. CarShop.Program
	Add reference to .Logic + .Repo
	Write code until "SNIP 2"
	Add NUGET reference to ConsoleMenu-simple (can use any other nuget packages too, e.g. EasyConsole)
	Write rest of code
