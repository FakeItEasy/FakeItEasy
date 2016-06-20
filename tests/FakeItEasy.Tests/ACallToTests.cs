namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FluentAssertions;
    using Xunit;

    public class ACallToTests
        : ConfigurableServiceLocatorTestBase
    {
        private readonly IFakeConfigurationManager configurationManager;

        public ACallToTests()
        {
            this.configurationManager = A.Fake<IFakeConfigurationManager>(x => x.Wrapping(ServiceLocator.Current.Resolve<IFakeConfigurationManager>()));
            this.StubResolve(this.configurationManager);
        }

        [Fact]
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
            result.Should().BeSameAs(configuration);
        }

        [Fact]
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
            result.Should().BeSameAs(configuration);
        }
    }
}
