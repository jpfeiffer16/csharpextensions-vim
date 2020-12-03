using System;
using System.IO;
using System.IO.Abstractions;
using System.Xml;

namespace ResharperDiagnosticsConverter
{
    /// <inheritdoc />
    public class ResharperReportVisitor : IResharperReportVisitor
    {
        private readonly XmlDocument _xmlDoc;
        private readonly IOffsetToLineColumnConverter _offsetToLineColumnConvert;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResharperReportVisitor"/> class.
        /// </summary>
        /// <param name="reportFile">The report file.</param>
        /// <param name="offsetToLineColumnConverter">The offset to line column converter.</param>
        /// <param name="fileSystem">The file system.</param>
        public ResharperReportVisitor(
            string reportFile,
            IOffsetToLineColumnConverter offsetToLineColumnConverter,
            IFileSystem fileSystem)
        {
            if (fileSystem is null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            _xmlDoc = new XmlDocument();
            using var loadStream = fileSystem.File.OpenRead(
                    reportFile ?? throw new ArgumentNullException(nameof(reportFile)));
            _xmlDoc.Load(loadStream);
            _offsetToLineColumnConvert = offsetToLineColumnConverter
                ?? throw new ArgumentNullException(nameof(offsetToLineColumnConverter));
        }

        /// <inheritdoc />
        public void VisitIssues(Action<string, int, int, int, int, string> issueVisitor)
        {
            var issues = _xmlDoc.DocumentElement.SelectNodes("//Report/Issues/Project/Issue");
            foreach (XmlNode issue in issues)
            {
                var file = issue.Attributes.GetNamedItem("File").Value;
                var offsetParts = issue.Attributes.GetNamedItem("Offset").Value.Split('-');
                var offsetStart = int.Parse(offsetParts[0]);
                var offsetEnd = int.Parse(offsetParts[1]);
                var message = issue.Attributes.GetNamedItem("Message").Value;
                try
                {
                    var lineColumnStartPosition = _offsetToLineColumnConvert.GetLineColumn(file, offsetStart);
                    var lineColumnEndPosition = _offsetToLineColumnConvert.GetLineColumn(file, offsetEnd);
                    // Remove one from the end column to make it exclusive.
                    issueVisitor(
                        file,
                        lineColumnStartPosition.Line,
                        lineColumnStartPosition.Column,
                        lineColumnEndPosition.Line,
                        lineColumnEndPosition.Column - 1,
                        message);
                }
                // Note we are just swallowing file errors here.
                catch (DirectoryNotFoundException) { }
                catch (FileNotFoundException) { }
            }
        }
    }
}
