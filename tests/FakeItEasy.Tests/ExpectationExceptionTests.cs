namespace FakeItEasy.Tests
{
    public class ExpectationExceptionTests
        : ExceptionContractTests<ExpectationException>
    {
        protected override ExpectationException CreateException()
        {
            return new ExpectationException("foo");
        }
    }
}
