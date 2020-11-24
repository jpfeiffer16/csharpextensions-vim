using System;
using System.Xml;

namespace ResharperDiagnosticsConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Must provide a file name for the resharper report.");
                Environment.Exit(1);
            }

            var reportFile = args[0];

            var doc = new XmlDocument();
            doc.Load(reportFile);
            var issues = doc.DocumentElement.SelectNodes("//Report/Issues/Project/Issue");
            Console.WriteLine(issues.Count);
            foreach (XmlNode issue in issues)
            {
                var file = issue.Attributes.GetNamedItem("File").Value.Replace("\\", "/");
                var line = issue.Attributes.GetNamedItem("Line").Value;
                var message = issue.Attributes.GetNamedItem("Message").Value;

                Console.WriteLine($"{file}:{line}:{message}");
            }
        }
    }
}
