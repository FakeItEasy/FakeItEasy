namespace FakeItEasy.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class DummyExceptionContractTests : ExceptionContractTests<DummyException>
    {
        protected override DummyException CreateException()
        {
            return new DummyException("test");
        }
    }
}
