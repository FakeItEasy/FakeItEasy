namespace FakeItEasy.Tests
{
    public class DummyExceptionContractTests : ExceptionContractTests<DummyException>
    {
        protected override DummyException CreateException()
        {
            return new DummyException("test");
        }
    }
}
