using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using NSubstitute;
using NUnit.Framework;

namespace ResharperDiagnosticsConverter.Tests.ResharperReportVisitorTests
{
    public class VisitIssuesWithErrors : Behavior, IDisposable
    {
        private string _reportFile;
        private IOffsetToLineColumnConverter _offsetToLineColumnConverter;
        private MemoryStream _testXmlStream;
        private IFileSystem _fileSystem;
        private Action<string, int, int, int, int, string> _visitorFunc;
        private ResharperReportVisitor _resharperReportVisitor;
        private Exception _directoryNotFoundException;
        private Exception _fileNotFoundException;

        protected override void Given()
        {
            _reportFile = "test-report-file";
            _offsetToLineColumnConverter = Substitute.For<IOffsetToLineColumnConverter>();
            _offsetToLineColumnConverter.GetLineColumn("test-file-1", 5)
                .Returns(
                    x => throw new DirectoryNotFoundException(),
                    x => throw new FileNotFoundException());

            var file = Substitute.For<IFile>();
            _testXmlStream = new MemoryStream(Encoding.UTF8.GetBytes(
@"<Report>
    <Issues>
        <Project>
            <Issue File=""test-file-1"" Offset=""5-10"" Message=""test-message-1"" />
        </Project>
    </Issues>
</Report>"));
            file.OpenRead(Arg.Is(_reportFile)).Returns(
                _testXmlStream);
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.File.Returns(file);

            _visitorFunc = (file, startColum, startLine, endLine, endColumn, message) => { };

            _resharperReportVisitor = new ResharperReportVisitor(_reportFile, _offsetToLineColumnConverter, _fileSystem);
        }

        protected override void When()
        {
            _directoryNotFoundException = ExpectException(() =>
                _resharperReportVisitor.VisitIssues(_visitorFunc));
            _fileNotFoundException = ExpectException(() =>
                _resharperReportVisitor.VisitIssues(_visitorFunc));
        }

        public void Dispose()
        {
            _testXmlStream.Dispose();
        }

        [Then]
        public void DirectoryNotFoundExceptionIsNull()
        {
            Assert.That(_directoryNotFoundException, Is.Null);
        }

        [Then]
        public void FileNotFoundExceptionIsNull()
        {
            Assert.That(_fileNotFoundException, Is.Null);
        }
    }
}
