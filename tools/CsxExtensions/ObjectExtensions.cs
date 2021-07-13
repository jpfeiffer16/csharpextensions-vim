using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CsxExtensions
{
    public static class ObjectExtensions
    {
        private static readonly List<object> _seenRefs = new List<object>();

        /// <summary>
        /// Dumps the value of an object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="indentLevel">The indent level.</param>
        /// <typeparam name="T"></typeparam>
        public static void Dump<T>(this T source, int indentLevel = 0)
        {
            _seenRefs.Clear();
            DumpValue(source, indentLevel);
            Console.WriteLine();
        }

        [Obsolete("Cannot call method directly")]
        public static void DumpValue<T>(T source, int indentLevel)
        {
            // if (_seenRefs.Find(seenRef => seenRef.Equals(source)) != null)
            // {
            //     Console.WriteLine("<Circular Reference>");
            // }

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

            if (!isPrimitive && _seenRefs.Contains(source))
            {
                Console.WriteLine("<Circular Reference>");
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
                if (type.GetInterfaces().Any(i => i.Name.StartsWith("IDictionary")))
                {
                    var dictionarySource = source as IDictionary;
                    var typeArgs = type.GetGenericArguments();
                    foreach (DictionaryEntry item in dictionarySource)
                    {
                        Indent(indentLevel + 1);
                        Console.Write("Key: ");
                        GetGenericDumpMethod(typeArgs[0])
                            .Invoke(null, new object[] { item.Key, indentLevel + 1 });
                        Console.WriteLine();
                        Indent(indentLevel + 1);
                        Console.Write("Value: ");
                        GetGenericDumpMethod(typeArgs[1])
                            .Invoke(null, new object[] { item.Value, indentLevel + 1 });
                    }
                }

                else if (type.GetInterfaces().Any(i => i.Name.StartsWith("IEnumerable")))
                {
                    var enumerableSource = source as IEnumerable<object>;
                    foreach (var item in enumerableSource)
                    {
                        Indent(indentLevel + 1);
                        GetGenericDumpMethod(item.GetType())
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

            _seenRefs.Add(source);
        }

        private static MethodInfo GetGenericDumpMethod(params Type[] genericTypes) =>
            typeof(CsxExtensions.ObjectExtensions).GetMethod("DumpValue").MakeGenericMethod(genericTypes);

        private static void Indent(int indentLevel) =>
            Console.Write(string.Join(string.Empty, Enumerable.Range(0, indentLevel).Select(n => "  ")));

        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType) return type.Name;

            var typeName = type.Name;
            int backTickIndex = 0;
            if ((backTickIndex = typeName.IndexOf("`")) > 0)
                typeName = typeName.Remove(backTickIndex);

            typeName += "<";
            typeName += string.Join(", ", type.GetGenericArguments().Select(ga => GetTypeName(ga)));
            typeName += ">";

            return typeName;
        }
    }
}
