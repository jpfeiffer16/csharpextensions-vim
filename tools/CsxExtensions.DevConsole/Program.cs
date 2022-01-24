using System;
using System.Collections.Generic;
using CsxExtensions;

namespace CsxExtensions.DevConsole
{
    struct Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        public class Test
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Test Next { get; set; }
            public static Test New(int id, string name, Test next = null)
            {
                return new Test
                {
                    Id = id,
                    Name = name,
                    Next = next
                };
            }
        }

        static void Main(string[] args)
        {
            new List<Test>
            {
                Test.New(123, "Test", Test.New(124, "Test", Test.New(125, "Test",
                    Test.New(126, "Test", Test.New(127, "Test",
                        Test.New(128, "Test", Test.New(129, "Test",
                            Test.New(130, "Test", Test.New(131, "Test")))))))))
            }.Dump();

            new List<Test>
            {
                Test.New(123, "Test"),
                Test.New(123, "Test"),
                Test.New(123, "Test"),
                Test.New(123, "Test"),
                Test.New(123, "Test"),
                Test.New(123, "Test"),
                Test.New(123, "Test"),
                Test.New(123, "Test"),
                Test.New(123, "Test"),
            }.Dump();

            var dictionary = new Dictionary<int, Test>
            {
                { 0, Test.New(123, "Test") },
                { 1, Test.New(123, "Test") },
                { 2, Test.New(123, "Test") },
                { 3, Test.New(123, "Test") },
                { 4, Test.New(123, "Test") },
                { 5, Test.New(123, "Test") },
                { 6, Test.New(123, "Test") },
            };
            dictionary.Dump();

            // try
            // {
            //     throw new InvalidOperationException("Duhh, this is a test.");
            // }
            // catch (Exception e)
            // {
            //     e.Dump();
            // }

            var test = new List<Test>
            {
                new Test
                {
                    Id = 1,
                    Name = "Test1"
                },
                new Test
                {
                    Id = 2,
                    Name = "Test2"
                }
            };

            test.Dump();
        }
    }
}
