namespace FakeItEasy.Tests.Configuration
{
    using FakeItEasy.Configuration;
    using FakeItEasy.Creation;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultInterceptionAsserterTests
    {
        private IProxyGenerator proxyGenerator;
        private DefaultInterceptionAsserter interceptionAsserter;

        [SetUp]
        public void Setup()
        {
            this.proxyGenerator = A.Fake<IProxyGenerator>();

            this.interceptionAsserter = new DefaultInterceptionAsserter(this.proxyGenerator);
        }

        [Test]
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

  The current proxy generator can not intercept the specified method for the following reason:
    - reason

";
            Assert.That(
                () =>
                {
                    this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(method, instance);
                },
                Throws.Exception.InstanceOf<FakeConfigurationException>().With.Message.EqualTo(expectedMessage));
        }
    }
}