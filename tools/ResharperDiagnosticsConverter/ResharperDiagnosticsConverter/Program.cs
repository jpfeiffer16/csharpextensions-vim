using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace ResharperDiagnosticsConverter
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Must provide a comand and a file name for the resharper report.");
                Environment.Exit(1);
            }

            var command = args[0];
            var reportFile = args[1];

            FileSystem fileSystem = new FileSystem();
            var visitor = new ResharperReportVisitor(
                reportFile,
                new OffsetToLineColumnConverter(fileSystem),
                fileSystem);
            if (command == "quickfix")
            {
                visitor.VisitIssues((file, line, column, endLine, endColumn, message) =>
                    Console.WriteLine($"{file}:{line}:{column}:{message}"));
            }
            else if (command == "highlight")
            {
                visitor.VisitIssues((file, line, column, endLine, endColumn, message) =>
                    Console.WriteLine(
                        $"{file}|{line}|{column}|{endLine}|{endColumn}|{message}"));
            }
            else
            {
                Console.Error.WriteLine($"Unknown command: {command}");
                Environment.Exit(1);
            }
        }
    }
}
