namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public static IEnumerable<object[]> CallSpecificationActions =>
            TestCases.FromObject<Action<IFoo>>(
                foo => A.CallTo(() => foo.Bar()),
                foo => A.CallTo(() => foo.Baz()),
                foo => A.CallToSet(() => foo.SomeProperty),
                foo => A.CallTo(foo));

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

        [Theory]
        [MemberData(nameof(CallSpecificationActions))]
        public void CallTo_should_not_add_rule_to_manager(Action<IFoo> action)
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var manager = Fake.GetFakeManager(foo);
            var initialRules = manager.Rules.ToList();

            // Act
            action(foo);

            // Assert
            manager.Rules.Should().Equal(initialRules);
        }
    }
}
