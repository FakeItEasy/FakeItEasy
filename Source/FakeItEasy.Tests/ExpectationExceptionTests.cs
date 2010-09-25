using NUnit.Framework;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ExpectationExceptionTests
        : ExceptionContractTests<ExpectationException>
    {
        protected override ExpectationException CreateException()
        {
            return new ExpectationException("fo");
        }
    }
}
