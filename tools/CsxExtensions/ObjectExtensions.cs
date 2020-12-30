using System;
using System.Collections.Generic;
using System.Linq;

namespace CsxExtensions
{
    public static class ObjectExtensions
    {
        // public static void Dump(this object source)
        // {
        //     DumpValue<object>(source, 0);
        // }

        public static void Dump<T>(this T source)
        {
            DumpValue<T>(source, 0);
        }


        public static void DumpValue<T>(object source, int indentLevel)
        {
            // Check property type (Primitive or Complex)
            // PRIMITIVE: 
            // Print the primitive value
            //
            // COMPLEX: 
            // Loop through properties and recursively call DumpValue() on them

            // var type = source.GetType();
            var type = typeof(T);
            var isPrimitive = !(type.GetProperties().Count() > 0);
            if (type.Name == "String")
                isPrimitive = true;


            Console.Write("(");
            Console.Write(type.Name);

            if (source is null)
            {
                Console.Write(") ");
                Console.Write("null");
                return;
            }

            if (isPrimitive) Console.Write(") ");
            else Console.WriteLine(")");


            if (isPrimitive)
            {
                // Type is primitive
                Console.Write(source);
            }
            else
            {
                // Type is complex
                if (type.GetInterfaces().Any(i => i.Name.StartsWith("IEnumerable")))
                {
                    // type.GetGenericArguments().MakeGenericType()
                    // var test = Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetGenericArguments()));
                    var enumerableSource = source as IEnumerable<object>;
                    foreach (var item in enumerableSource)
                    {
                        Indent(indentLevel + 1);
                        var genericMethod = typeof(CsxExtensions.ObjectExtensions).GetMethod("DumpValue").MakeGenericMethod(type.GetGenericArguments());
                        genericMethod.Invoke(null, new object[] { item, indentLevel + 2 });
                        // DumpValue<T>(item, indentLevel + 1);
                        // DumpValue.Invoke
                    }
                }
                else
                {
                    foreach (var property in type.GetProperties())
                    {
                        Indent(indentLevel);
                        var value = property.GetValue(source);
                        Console.Write("{0} : ", property.Name);
                        var propertyType = property.PropertyType;
                        var genericMethod = typeof(CsxExtensions.ObjectExtensions).GetMethod("DumpValue").MakeGenericMethod(propertyType);
                        genericMethod.Invoke(null, new object[] { value, indentLevel + 1 });
                        // DumpValue<T>(value, indentLevel + 1);
                        Console.WriteLine();
                    }
                }
            }
        }

        private static void Indent(int indentLevel)
        {
            Console.Write(string.Join(string.Empty, Enumerable.Range(0, indentLevel).Select(n => "  ")));
        }
    }
}
