namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class StartConfigurationTests
    {
        private readonly FakeManager fakeObject;
        private readonly ExpressionCallRule rule;
        private readonly ExpressionCallRule.Factory ruleFactory;
        private readonly IConfigurationFactory configurationFactory;
        private readonly ICallExpressionParser expressionParser;
        private readonly IInterceptionAsserter interceptionAsserter;
        private LambdaExpression argumentToRuleFactory;

        public StartConfigurationTests()
        {
            this.fakeObject = A.Fake<FakeManager>();
            this.rule = ExpressionHelper.CreateRule<IFoo>(x => x.Bar());
            this.ruleFactory = x =>
            {
                this.argumentToRuleFactory = x;
                return this.rule;
            };

            this.configurationFactory = A.Fake<IConfigurationFactory>();

            this.expressionParser = A.Fake<ICallExpressionParser>();

            this.interceptionAsserter = A.Fake<IInterceptionAsserter>();
        }

        [Fact]
        public void CallsTo_for_void_calls_should_return_configuration_from_configuration_factory()
        {
            // Arrange
            var returnedConfiguration = A.Fake<IVoidArgumentValidationConfiguration>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration(this.fakeObject, this.rule)).Returns(returnedConfiguration);

            var configuration = this.CreateConfiguration<IFoo>();

            Expression<Action<IFoo>> callSpecification = x => x.Bar();

            // Act
            var result = configuration.CallsTo(callSpecification);

            // Assert
            this.argumentToRuleFactory.Should().BeSameAs(callSpecification);
            result.Should().BeSameAs(returnedConfiguration);
        }

        [Fact]
        public void CallsTo_for_void_calls_should_add_rule_to_fake_object()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Act
            configuration.CallsTo(x => x.Bar());

            // Assert
            A.CallTo(() => this.fakeObject.AddRuleFirst(this.rule)).MustHaveHappened();
        }

        [Fact]
        public void CallsTo_for_void_calls_should_be_null_guarded()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Assert
            Expression<Action> call = () =>
                configuration.CallsTo(x => x.Bar());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void CallsTo_for_void_calls_should_set_applicator_to_do_nothing_as_default()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Act
            configuration.CallsTo(x => x.Bar());

            // Assert
            var exception = Record.Exception(() =>
                this.rule.Applicator(A.Fake<IInterceptedFakeObjectCall>()));
            exception.Should().BeNull("because the rule's applicator should do nothing");
        }

        [Fact]
        public void CallsTo_for_function_calls_should_return_configuration_from_configuration_factory()
        {
            // Arrange
            var returnedConfiguration = A.Fake<IAnyCallConfigurationWithReturnTypeSpecified<int>>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(this.fakeObject, this.rule)).Returns(returnedConfiguration);

            var configuration = this.CreateConfiguration<IFoo>();

            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();

            // Act
            var result = configuration.CallsTo(callSpecification);

            // Assert
            this.argumentToRuleFactory.Should().BeSameAs(callSpecification);
            result.Should().BeSameAs(returnedConfiguration);
        }

        [Fact]
        public void CallsTo_for_function_calls_should_add_rule_to_fake_object()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Act
            configuration.CallsTo(x => x.Baz());

            // Assert
            A.CallTo(() => this.fakeObject.AddRuleFirst(this.rule)).MustHaveHappened();
        }

        [Fact]
        public void CallsTo_for_function_calls_should_be_null_guarded()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Assert
            Expression<Action> call = () =>
                configuration.CallsTo(x => x.Baz());
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void AnyCall_should_add_rule_to_fake_object()
        {
            // Arrange
            var configuration = this.CreateConfiguration<int>();

            // Act
            configuration.AnyCall();

            // Assert
            A.CallTo(() => this.fakeObject.AddRuleFirst(
                A<IFakeObjectCallRule>.That.IsInstanceOf(typeof(AnyCallCallRule)))).MustHaveHappened();
        }

        [Fact]
        public void AnyCall_should_return_configuration_from_factory()
        {
            // Arrange
            var returnedConfig = A.Fake<IAnyCallConfigurationWithNoReturnTypeSpecified>();

            A.CallTo(() => this.configurationFactory.CreateAnyCallConfiguration(this.fakeObject, A<AnyCallCallRule>.That.Not.IsNull())).Returns(returnedConfig);

            var configuration = this.CreateConfiguration<int>();

            // Act
            var result = configuration.AnyCall();

            // Assert
            result.Should().BeSameAs(returnedConfig);
        }

        [Fact]
        public void Should_call_interception_asserter_when_configuring_function_call()
        {
            // Arrange
            var call = ExpressionHelper.CreateExpression<IFoo>(x => x.Baz());
            var configuration = this.CreateConfiguration<IFoo>();

            var parsedExpression = A.Dummy<ParsedCallExpression>();

            A.CallTo(() => this.fakeObject.Object).Returns("fake");
            A.CallTo(() => this.expressionParser.Parse(call)).Returns(parsedExpression);

            // Act
            configuration.CallsTo(call);

            // Assert
            A.CallTo(() => this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsedExpression.CalledMethod,
                "fake")).MustHaveHappened();
        }

        [Fact]
        public void Should_call_interception_asserter_when_configuring_void_call()
        {
            // Arrange
            var call = ExpressionHelper.CreateExpression<IFoo>(x => x.Bar());
            var configuration = this.CreateConfiguration<IFoo>();

            var parsedExpression = A.Dummy<ParsedCallExpression>();

            A.CallTo(() => this.fakeObject.Object).Returns("fake");
            A.CallTo(() => this.expressionParser.Parse(call)).Returns(parsedExpression);

            // Act
            configuration.CallsTo(call);

            // Assert
            A.CallTo(() => this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsedExpression.CalledMethod,
                "fake")).MustHaveHappened();
        }

        private StartConfiguration<T> CreateConfiguration<T>()
        {
            return new StartConfiguration<T>(this.fakeObject, this.ruleFactory, this.configurationFactory, this.expressionParser, this.interceptionAsserter);
        }
    }
}
