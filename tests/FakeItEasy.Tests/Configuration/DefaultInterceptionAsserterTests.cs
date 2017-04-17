namespace FakeItEasy.Tests.Configuration
{
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Configuration;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class DefaultInterceptionAsserterTests
    {
        private readonly IProxyGenerator proxyGenerator;
        private readonly DefaultInterceptionAsserter interceptionAsserter;

        public DefaultInterceptionAsserterTests()
        {
            this.proxyGenerator = A.Fake<IProxyGenerator>();

            this.interceptionAsserter = new DefaultInterceptionAsserter(this.proxyGenerator);
        }

        [Fact]
        public void Should_throw_with_message_from_generator()
        {
            // Arrange
            var method = typeof(object).GetMethod("ToString");
            var instance = 1;

            string outValue;
            A.CallTo(() => this.proxyGenerator.MethodCanBeInterceptedOnInstance(method, instance, out outValue))
                .WithAnyArguments()
                .Returns(false)
                .AssignsOutAndRefParameters("reason");

            // Act, Assert
            var expectedMessage =
@"

  The current proxy generator can not intercept the method System.Object.ToString() for the following reason:
    - reason

";
            var exception = Record.Exception(() =>
                this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(method, instance));

            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().WithMessage(expectedMessage);
        }
    }
}
