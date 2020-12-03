using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using NSubstitute;
using NUnit.Framework;

namespace ResharperDiagnosticsConverter.Tests.ResharperReportVisitorTests
{
    public class ConstructorWithErrors : Behavior, IDisposable
    {
        private string _reportFile;
        private IOffsetToLineColumnConverter _offsetToLineColumnConverter;
        private IFileSystem _fileSystem;
        private Exception _nullReportFileException;
        private Exception _nullConverterException;
        private Exception _nullFileSystemException;
        private MemoryStream _testXmlStream;

        protected override void Given()
        {
            _reportFile = "test-report-file";
            _offsetToLineColumnConverter = Substitute.For<IOffsetToLineColumnConverter>();
            var file = Substitute.For<IFile>();
            _testXmlStream = new MemoryStream(Encoding.UTF8.GetBytes("<text></text>"));
            file.OpenRead(Arg.Is(_reportFile)).Returns(
                _testXmlStream);
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.File.Returns(file);
        }

        protected override void When()
        {
            _nullReportFileException = ExpectException(() =>
                new ResharperReportVisitor(null, _offsetToLineColumnConverter, _fileSystem));
            _nullConverterException = ExpectException(() =>
                new ResharperReportVisitor(_reportFile, null, _fileSystem));
            _nullFileSystemException = ExpectException(() =>
                new ResharperReportVisitor(_reportFile, _offsetToLineColumnConverter, null));
        }

        public void Dispose()
        {
            _testXmlStream.Dispose();
        }

        [Then]
        public void NullReportFileExceptionIsCorrectType()
        {
            Assert.That(_nullReportFileException, Is.TypeOf<ArgumentNullException>());
        }

        [Then]
        public void NullReportFileExceptionHasCorrectMessage()
        {
            Assert.That(_nullReportFileException.Message, Does.Contain("reportFile"));
        }

        [Then]
        public void NullConverterExceptionIsCorrectType()
        {
            Assert.That(_nullConverterException, Is.TypeOf<ArgumentNullException>());
        }

        [Then]
        public void NullConverterExceptionHasCorrectMessage()
        {
            Assert.That(_nullConverterException.Message, Does.Contain("offsetToLineColumnConverter"));
        }

        [Then]
        public void NullFileSystemExceptionIsCorrectType()
        {
            Assert.That(_nullFileSystemException, Is.TypeOf<ArgumentNullException>());
        }

        [Then]
        public void NullFileSystemExceptionHasCorrectMessage()
        {
            Assert.That(_nullFileSystemException.Message, Does.Contain("fileSystem"));
        }
    }
}
