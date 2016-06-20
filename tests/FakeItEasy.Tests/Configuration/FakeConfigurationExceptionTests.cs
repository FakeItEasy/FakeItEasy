namespace FakeItEasy.Tests.Configuration
{
    using FakeItEasy.Configuration;

    public class FakeConfigurationExceptionTests
        : ExceptionContractTests<FakeConfigurationException>
    {
        protected override FakeConfigurationException CreateException()
        {
            return new FakeConfigurationException();
        }
    }
}
