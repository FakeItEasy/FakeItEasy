using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class DummyExceptionContractTests : ExceptionContractTests<DummyException>
    {

        protected override DummyException CreateException()
        {
            return new DummyException("test");
        }
    }
}
