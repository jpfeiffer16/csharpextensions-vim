using System;
using System.IO.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace ResharperDiagnosticsConverter.Tests.OffsetToLineColumnConverterTests
{
    public class GetLineColumnWithErrors : Behavior
    {
        private const string TestFileContent =
@"line 1
line 2
line 3
";
        private const string TestFilename = "test-file";

        private OffsetToLineColumnConverter _offsetToLineColumnConverter;
        private Exception _badOffsetException;

        protected override void Given()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var fileClass = Substitute.For<IFile>();
            fileClass.ReadAllText(TestFilename).Returns(TestFileContent);
            fileSystem.File.Returns(fileClass);
            _offsetToLineColumnConverter = new OffsetToLineColumnConverter(fileSystem);
        }

        protected override void When()
        {
            _badOffsetException = ExpectException (() => _offsetToLineColumnConverter.GetLineColumn(TestFilename, 123));
        }

        [Then]
        public void BadOffsetExceptionIsCorrectType()
        {
            Assert.That(_badOffsetException, Is.TypeOf<Exception>());
        }

        [Then]
        public void BadOffsetExceptionHasCorrectMessage()
        {
            Assert.That(_badOffsetException.Message, Is.EqualTo("line not found"));
        }

    }
}
