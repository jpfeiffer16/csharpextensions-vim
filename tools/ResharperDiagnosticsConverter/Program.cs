using System;
using System.IO;
using System.Xml;

namespace ResharperDiagnosticsConverter
{
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

            if (command == "quickfix")
            {
                var doc = new XmlDocument();
                doc.Load(reportFile);
                var issues = doc.DocumentElement.SelectNodes("//Report/Issues/Project/Issue");
                foreach (XmlNode issue in issues)
                {
                    var file = issue.Attributes.GetNamedItem("File").Value.Replace("\\", "/");
                    var line = issue.Attributes.GetNamedItem("Line").Value;
                    var message = issue.Attributes.GetNamedItem("Message").Value;

                    Console.WriteLine($"{file}:{line}:{message}");
                }
            }
            else if (command == "highlight")
            {
                var doc = new XmlDocument();
                doc.Load(reportFile);
                var issues = doc.DocumentElement.SelectNodes("//Report/Issues/Project/Issue");
                var converter = new OffsetToLineColumnConverter();
                foreach (XmlNode issue in issues)
                {
                    var file = issue.Attributes.GetNamedItem("File").Value.Replace("\\", "/");
                    var offsetParts = issue.Attributes.GetNamedItem("Offset").Value.Split('-');
                    var offsetStart = int.Parse(offsetParts[0]);
                    var offsetEnd = int.Parse(offsetParts[1]);
                    var message = issue.Attributes.GetNamedItem("Message").Value;

                    try
                    {
                        var (line, column) = converter.GetLineColumn(file, offsetStart);
                        var (endLine, endColumn) = converter.GetLineColumn(file, offsetEnd);
                        // Remove one from the end column to make it exclusive.
                        endColumn--;

                        Console.WriteLine($"{file}:{line}:{column}:{endLine}:{endColumn}:{message}");
                    }
                    // Note we are just swallowing file errors here.
                    catch (DirectoryNotFoundException) { }
                    catch (FileNotFoundException) { }
                }
           }
            else
            {
                Console.Error.WriteLine($"Unknown command: {command}");
                Environment.Exit(1);
            }
        }
    }
}
