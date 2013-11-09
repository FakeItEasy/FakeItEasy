namespace FakeItEasy.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ExpectationExceptionTests
        : ExceptionContractTests<ExpectationException>
    {
        protected override ExpectationException CreateException()
        {
            return new ExpectationException("foo");
        }
    }
}
