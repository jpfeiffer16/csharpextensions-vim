using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using NSubstitute;
using NUnit.Framework;

namespace ResharperDiagnosticsConverter.Tests.ResharperReportVisitorTests
{
    public class VisitIssues : Behavior, IDisposable
    {
        private string _reportFile;
        private IOffsetToLineColumnConverter _offsetToLineColumnConverter;
        private MemoryStream _testXmlStream;
        private IFileSystem _fileSystem;
        private Action<string, int, int, int, int, string> _visitorFunc;
        private ResharperReportVisitor _resharperReportVisitor;
        private readonly List<(string file, int line, int column, int endLine, int endColumn, string message)> _visitCalls = new List<(string, int, int, int, int, string)>();

        protected override void Given()
        {
            _reportFile = "test-report-file";
            _offsetToLineColumnConverter = Substitute.For<IOffsetToLineColumnConverter>();
            _offsetToLineColumnConverter.GetLineColumn("test-file-1", 5)
                .Returns(new LineColumnPosition(1, 5));
            _offsetToLineColumnConverter.GetLineColumn("test-file-1", 10)
                .Returns(new LineColumnPosition(1, 10));
            _offsetToLineColumnConverter.GetLineColumn("test-file-2", 15)
                .Returns(new LineColumnPosition(2, 5));
            _offsetToLineColumnConverter.GetLineColumn("test-file-2", 20)
                .Returns(new LineColumnPosition(2, 10));
            var file = Substitute.For<IFile>();
            _testXmlStream = new MemoryStream(Encoding.UTF8.GetBytes(
@"<Report>
    <Issues>
        <Project>
            <Issue File=""test-file-1"" Offset=""5-10"" Message=""test-message-1"" />
        </Project>
        <Project>
            <Issue File=""test-file-2"" Offset=""15-20"" Message=""test-message-2"" />
        </Project>
    </Issues>
</Report>"));
            file.OpenRead(Arg.Is(_reportFile)).Returns(
                _testXmlStream);
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.File.Returns(file);

            _visitorFunc = (file, startColum, startLine, endLine, endColumn, message) => {
                _visitCalls.Add((file, startColum, startLine, endLine, endColumn, message));
            };

            _resharperReportVisitor = new ResharperReportVisitor(_reportFile, _offsetToLineColumnConverter, _fileSystem);
        }

        protected override void When()
        {
            _resharperReportVisitor.VisitIssues(_visitorFunc);
        }

        public void Dispose()
        {
            _testXmlStream.Dispose();
        }

        [Then]
        public void VisitCallsCountIsCorrect()
        {
            Assert.That(_visitCalls.Count, Is.EqualTo(2));
        }

        #region First issue

        [Then]
        public void FirstIssueFileIsCorrect()
        {
            Assert.That(_visitCalls[0].file, Is.EqualTo("test-file-1"));
        }

        [Then]
        public void FirstIssueLineIsCorrect()
        {
            Assert.That(_visitCalls[0].line, Is.EqualTo(1));
        }

        [Then]
        public void FirstIssueColumnIsCorrect()
        {
            Assert.That(_visitCalls[0].column, Is.EqualTo(5));
        }

        [Then]
        public void FirstIssueEndLineIsCorrect()
        {
            Assert.That(_visitCalls[0].endLine, Is.EqualTo(1));
        }

        [Then]
        public void FirstIssueEndColumnIsCorrect()
        {
            // 9 instead of 10 because we make the end exclusive.
            Assert.That(_visitCalls[0].endColumn, Is.EqualTo(9));
        }

        [Then]
        public void FirstIssueMessagenIsCorrect()
        {
            Assert.That(_visitCalls[0].message, Is.EqualTo("test-message-1"));
        }

        #endregion

        #region Second issue

        [Then]
        public void SecondIssueFileIsCorrect()
        {
            Assert.That(_visitCalls[1].file, Is.EqualTo("test-file-2"));
        }

        [Then]
        public void SecondIssueLineIsCorrect()
        {
            Assert.That(_visitCalls[1].line, Is.EqualTo(2));
        }

        [Then]
        public void SecondIssueColumnIsCorrect()
        {
            Assert.That(_visitCalls[1].column, Is.EqualTo(5));
        }

        [Then]
        public void SecondIssueEndLineIsCorrect()
        {
            Assert.That(_visitCalls[1].endLine, Is.EqualTo(2));
        }

        [Then]
        public void SecondIssueEndColumnIsCorrect()
        {
            // 9 instead of 10 because we make the end exclusive.
            Assert.That(_visitCalls[1].endColumn, Is.EqualTo(9));
        }

        [Then]
        public void SecondIssueMessagenIsCorrect()
        {
            Assert.That(_visitCalls[1].message, Is.EqualTo("test-message-2"));
        }

        #endregion
    }
}
