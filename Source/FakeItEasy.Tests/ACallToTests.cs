namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class ACallToTests
        : ConfigurableServiceLocatorTestBase
    {
        private IFakeConfigurationManager configurationManager;

        [Test]
        public void CallTo_with_void_call_should_return_configuration_from_configuration_manager()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Action> callSpecification = () => foo.Bar();

            var configuration = A.Fake<IVoidArgumentValidationConfiguration>();
            A.CallTo(() => this.configurationManager.CallTo(callSpecification)).Returns(configuration);

            // Act
            var result = A.CallTo(callSpecification);

            // Assert
            Assert.That(result, Is.SameAs(configuration));
        }

        [Test]
        public void CallTo_with_function_call_should_return_configuration_from_configuration_manager()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Func<int>> callSpecification = () => foo.Baz();

            var configuration = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();
            A.CallTo(() => this.configurationManager.CallTo(callSpecification)).Returns(configuration);

            // Act
            var result = A.CallTo(callSpecification);

            // Assert
            Assert.That(result, Is.SameAs(configuration));
        }

        protected override void OnSetup()
        {
            this.configurationManager = A.Fake<IFakeConfigurationManager>(x => x.Wrapping(ServiceLocator.Current.Resolve<IFakeConfigurationManager>()));
            this.StubResolve<IFakeConfigurationManager>(this.configurationManager);
        }
    }
}