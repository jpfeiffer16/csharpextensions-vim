using System.IO.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace ResharperDiagnosticsConverter.Tests.OffsetToLineColumnConverterTests
{
    public class GetLineColumn : Behavior
    {
        private const string TestFileContent =
@"line 1
line 2
line 3
";
        private const string TestFilename = "test-file";

        private OffsetToLineColumnConverter _offsetToLineColumnConverter;
        private LineColumnPosition _result;

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
            _result = _offsetToLineColumnConverter.GetLineColumn(TestFilename, 12);
        }

        [Then]
        public void ResultLineIsCorrect()
        {
            Assert.That(_result.Line, Is.EqualTo(2));
        }

        [Then]
        public void ResultColumnIsCorrect()
        {
            Assert.That(_result.Column, Is.EqualTo(6));
        }
    }
}
