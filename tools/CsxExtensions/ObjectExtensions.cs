using System;
using System.Collections.Generic;
using System.Linq;

namespace CsxExtensions
{
    public static class ObjectExtensions
    {
        public static void Dump<T>(this T source, int indentLevel = 0)
        {
            var type = typeof(T);
            var isPrimitive = !(type.GetProperties().Count() > 0);
            if (type.Name == "String")
                isPrimitive = true;

            Console.Write("(");
            Console.Write(type.Name);

            if (source == null)
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
                    var enumerableSource = source as IEnumerable<object>;
                    foreach (var item in enumerableSource)
                    {
                        Indent(indentLevel + 1);
                        GetGenericDumpMethod(type.GetGenericArguments())
                            .Invoke(null, new object[] { item, indentLevel + 2 });
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
                        GetGenericDumpMethod(propertyType)
                            .Invoke(null, new object[] { value, indentLevel + 1 });
                        Console.WriteLine();
                    }
                }
            }
        }

        private static System.Reflection.MethodInfo GetGenericDumpMethod(params Type[] genericTypes) =>
            typeof(CsxExtensions.ObjectExtensions).GetMethod("Dump").MakeGenericMethod(genericTypes);

        private static void Indent(int indentLevel)
        {
            Console.Write(string.Join(string.Empty, Enumerable.Range(0, indentLevel).Select(n => "  ")));
        }
    }
}
