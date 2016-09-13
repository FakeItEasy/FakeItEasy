namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests;
    using FluentAssertions;
    using Xunit;

    public class FakeConfigurationManagerTests
    {
        private IConfigurationFactory configurationFactory;
        private FakeConfigurationManager configurationManager;
        private ExpressionCallRule.Factory ruleFactory;
        private ExpressionCallRule ruleReturnedFromFactory;
        private CallExpressionParser callExpressionParser;
        private IInterceptionAsserter interceptionAsserter;

        public FakeConfigurationManagerTests()
        {
            this.OnSetup();
        }

        public static IEnumerable<object[]> CallSpecificationActions =>
            TestCases.FromObject<Action<FakeConfigurationManagerTests, IFoo>>(
                (@this, foo) => @this.configurationManager.CallTo(() => foo.Bar()),
                (@this, foo) => @this.configurationManager.CallTo(() => foo.Baz()),
                (@this, foo) => @this.configurationManager.CallToSet(() => foo.SomeProperty),
                (@this, foo) => @this.configurationManager.CallTo(foo));

        // Callto
        [Fact]
        public void CallTo_with_void_call_should_call_configuration_factory_with_call_rule_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Bar());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration(A<FakeManager>._, this.ruleReturnedFromFactory)).MustHaveHappened();
        }

        [Fact]
        public void CallTo_with_void_call_should_be_null_guarded()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act

            // Assert
            Expression<Action> call = () =>
                this.configurationManager.CallTo(() => foo.Bar());
            call.Should().BeNullGuarded();
        }

        // CallTo with function calls
        [Fact]
        public void CallTo_with_function_call_should_call_configuration_factory_with_call_rule_from_factory()
        {
            // Arrange
            var foo = A.Fake<IFoo>();

            // Act
            this.configurationManager.CallTo(() => foo.Baz());

            // Assert
            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(A<FakeManager>._, this.ruleReturnedFromFactory)).MustHaveHappened();
        }

        [Fact]
        public void CallTo_with_function_call_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<Action> call = () =>
                this.configurationManager.CallTo(() => string.Empty.Length);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Should_call_interception_asserter_when_configuring_function_call()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Func<int>> call = () => foo.Baz();

            var parsedCall = this.callExpressionParser.Parse(call);

            // Act
            this.configurationManager.CallTo(call);

            // Assert
            A.CallTo(() => this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsedCall.CalledMethod,
                parsedCall.CallTarget)).MustHaveHappened();
        }

        [Fact]
        public void Should_call_interception_asserter_when_configuring_void_call()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Action> call = () => foo.Bar();

            var parsedCall = this.callExpressionParser.Parse(call);

            // Act
            this.configurationManager.CallTo(call);

            // Assert
            A.CallTo(() => this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsedCall.CalledMethod,
                parsedCall.CallTarget)).MustHaveHappened();
        }

        [Fact]
        public void Should_call_configuration_factory_with_manager_from_fake()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var manager = Fake.GetFakeManager(fake);

            // Act
            this.configurationManager.CallTo(fake);

            // Assert
            A.CallTo(() => this.configurationFactory.CreateAnyCallConfiguration(
                manager, A<AnyCallCallRule>.That.Not.IsNull())).MustHaveHappened();
        }

        [Fact]
        public void Should_return_configuration_when_configuring_any_call()
        {
            // Arrange
            var fake = A.Fake<IFoo>();
            var expectedConfiguration = A.Dummy<IAnyCallConfigurationWithNoReturnTypeSpecified>();
            A.CallTo(() => this.configurationFactory.CreateAnyCallConfiguration(
                A<FakeManager>._, A<AnyCallCallRule>._))
                .Returns(expectedConfiguration);

            // Act
            var result = this.configurationManager.CallTo(fake);

            // Assert
            result.Should().BeSameAs(expectedConfiguration);
        }

        [Theory]
        [MemberData(nameof(CallSpecificationActions))]
        public void CallTo_should_not_add_rule_to_manager(Action<FakeConfigurationManagerTests, IFoo> action)
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var manager = Fake.GetFakeManager(foo);
            var initialRules = manager.Rules.ToList();

            // Act
            action(this, foo);

            // Assert
            manager.Rules.Should().Equal(initialRules);
        }

        private void OnSetup()
        {
            this.configurationFactory = A.Fake<IConfigurationFactory>();
            this.callExpressionParser = new CallExpressionParser();
            this.interceptionAsserter = A.Fake<IInterceptionAsserter>();

            Expression<Action<IFoo>> dummyExpression = x => x.Bar();
            var parsedDummyExpression = this.callExpressionParser.Parse(dummyExpression);
            this.ruleReturnedFromFactory = ServiceLocator.Current.Resolve<ExpressionCallRule.Factory>().Invoke(parsedDummyExpression);
            this.ruleFactory = x => this.ruleReturnedFromFactory;

            this.configurationManager = this.CreateManager();
        }

        private FakeConfigurationManager CreateManager()
        {
            return new FakeConfigurationManager(
                this.configurationFactory,
                this.ruleFactory,
                this.callExpressionParser,
                this.interceptionAsserter);
        }
    }
}
