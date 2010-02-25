namespace FakeItEasy.Tests.Configuration
{
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class FakeConfigurationExceptionTests
        : ExceptionContractTests<FakeConfigurationException>
    {
        protected override FakeConfigurationException CreateException()
        {
            return new FakeConfigurationException();
        }
    }
}
