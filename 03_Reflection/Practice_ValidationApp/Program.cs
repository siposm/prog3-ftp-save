using System;
using Validation.Classes;

namespace Practice_ValidationApp
{
    class Person
    {
        [MaxLength(20)]
        public string Name { get; set; }

        [Range(50, 300)]
        public int Height { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Person validPerson = new Person()
            {
                Name = "Alex Fergusson",
                Height = 190
            };

            Person invalidPerson1 = new Person()
            {
                Name = "012345678901234567891",
                Height = 190
            };

            Person invalidPerson2 = new Person()
            {
                Name = "Alex Fergusson",
                Height = 22
            };

            Validator validator = new Validator();
            Console.WriteLine("1st person: " + validator.Validate(validPerson));
            Console.WriteLine("2nd person: " + validator.Validate(invalidPerson1));
            Console.WriteLine("3rd person: " + validator.Validate(invalidPerson2));
            Console.ReadLine();
        }
    }
}