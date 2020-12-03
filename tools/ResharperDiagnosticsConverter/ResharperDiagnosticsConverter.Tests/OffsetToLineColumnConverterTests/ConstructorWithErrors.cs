using System;
using NUnit.Framework;

namespace ResharperDiagnosticsConverter.Tests.OffsetToLineColumnConverterTests
{
    public class ConstructorWithErrors : Behavior
    {
        private Exception _nullFileSystemException;

        protected override void Given() { }

        protected override void When()
        {
            _nullFileSystemException = ExpectException(() => 
                new OffsetToLineColumnConverter(null));
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
