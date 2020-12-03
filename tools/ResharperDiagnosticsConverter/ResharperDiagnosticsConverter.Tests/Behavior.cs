using System;
using NUnit.Framework;

namespace ResharperDiagnosticsConverter.Tests
{
    public abstract class Behavior
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Given();
            When();
        }

        protected abstract void Given();

        protected abstract void When();

        protected Exception ExpectException(Action cb)
        {
            Exception resultException = null;

            try
            {
                cb();
            }
            catch(Exception e)
            {
                resultException = e;
            }

            return resultException;
        }
    }
}
