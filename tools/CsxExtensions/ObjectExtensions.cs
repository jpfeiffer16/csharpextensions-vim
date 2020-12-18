using System;
using System.Collections.Generic;
using System.Linq;

namespace CsxExtensions
{
    public static class ObjectExtensions
    {
        public static void Dump(this object source)
        {
            var type = source.GetType();
            var properties = type.GetProperties();
            Console.WriteLine("NAME: {0}", type.Name);
            if (type.Name.StartsWith("Dictionary"))
            {
                Console.WriteLine("Dictionary");
            }
            else if (type.GetInterfaces().Any(i => i.Name == "IEnumerable"))
            {
                Console.WriteLine("IEnumerable");
            }
            else if (properties.Count() > 0)
            {
                foreach (var property in properties)
                {
                    Console.Write(property.Name);
                    Console.Write(" : ");
                    Console.Write(type.GetProperty(property.Name).GetValue(source));
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine(source);
            }
        }
    }
}
