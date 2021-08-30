using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Linq;

namespace Lecture_XmlSerializer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    class ExcludeFromXmlAttribute : Attribute
    {
        public string Reason { get; set; }
    }
    class Person
    {
        [DisplayName("Személynév")]
        public string Name { get; set; }

        [DisplayName("E-Mail cím")]
        public string Email { get; set; }

        [DisplayName("Életkor")]
        public int Age { get; set; }

        [DisplayName("Lakcím")]
        [ExcludeFromXml(Reason = "Top Secret")]
        public string Address { get; set; }

        [DisplayName("Születési dátum")]
        public DateTime BirthDate { get; set; }
    }

    class XmlBuilder
    {
        string GetPrettyName(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<DisplayNameAttribute>();
            return attr == null ? property.Name : attr.DisplayName;
        }
        bool IsAllowed(PropertyInfo property)
        {
            return property.GetCustomAttribute<ExcludeFromXmlAttribute>() == null;
        }

        // typeof(xxx) vs instance.GetType().GetProperty("Email")
        // property.PropertyType vs property.GetType()

        public XElement ToXml(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            Type type = instance.GetType();
            XElement node = new XElement("instance");
            node.Add(new XAttribute("typeName", type.FullName));
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (IsAllowed(property))
                {
                    XElement dataNode = new XElement("data");
                    dataNode.Add(new XAttribute("name", property.Name));
                    dataNode.Add(new XAttribute("prettyName", GetPrettyName(property)));
                    dataNode.Value = property.GetValue(instance).ToString();
                    node.Add(dataNode);
                }
            }
            return node;
        }
    }

    class NameComparer : IComparer<object>
    {
        public int Compare(dynamic x, dynamic y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    // string name1 = x.GetType().GetProperty("Name")?.GetValue(x)?.ToString();
    // string name2 = y.GetType().GetProperty("Name")?.GetValue(y)?.ToString();
    // return name1.CompareTo(name2);
    // To read: DynamicObject.GetDynamicMemberNames , TrySetMember, TryGetMember

    class Program
    {
        static void Main(string[] args)
        {
            Person person = new Person() { Name = "Béla", 
                Age = 42, 
                Address = "Bélavár 42", 
                BirthDate = DateTime.Now.AddDays(-12345), 
                Email = "bela@bela.hu" };
            var product = new { Name = "Something", 
                Price = 12345, Quantity = 42 };
            XmlBuilder builder = new XmlBuilder();
            XElement personXml = builder.ToXml(person);
            XElement productXml = builder.ToXml(product);
            Console.WriteLine(personXml);
            Console.WriteLine(productXml);
            Console.ReadLine();

            List<object> objects = new List<object>() { product, person };
            objects.Sort(new NameComparer());
            foreach (dynamic item in objects)
            {
                Console.WriteLine(item.Name);
            }
            Console.ReadLine();
            //Console.WriteLine(item.GetType().GetProperty("Name")?.GetValue(item)?.ToString());
        }
    }

}
