using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CsxExtensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Dumps the value of an object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="indentLevel">The indent level.</param>
        /// <typeparam name="T"></typeparam>
        public static void Dump<T>(this T source, int indentLevel = 0)
        {
            var type = typeof(T);
            var isPrimitive = !(type.GetProperties().Count() > 0);
            if (type.Name == "String")
                isPrimitive = true;

            Console.Write("(");
            Console.Write(GetTypeName(type));

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
                            .Invoke(null, new object[] { item, indentLevel + 1 });
                    }
                }
                else
                {
                    foreach (var property in type.GetProperties())
                    {
                        Indent(indentLevel + 1);
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

        private static MethodInfo GetGenericDumpMethod(params Type[] genericTypes) =>
            typeof(CsxExtensions.ObjectExtensions).GetMethod("Dump").MakeGenericMethod(genericTypes);

        private static void Indent(int indentLevel) =>
            Console.Write(string.Join(string.Empty, Enumerable.Range(0, indentLevel).Select(n => "  ")));

        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType) return type.Name;

            var typeName = type.Name;
            int backTickIndex = 0;
            if ((backTickIndex = typeName.IndexOf("`")) > 0)
                typeName = typeName.Remove(backTickIndex);

            foreach (var genericType in type.GetGenericArguments())
            {
                typeName += $"<{GetTypeName(genericType)}>";
            }

            return typeName;
        }
    }
}
