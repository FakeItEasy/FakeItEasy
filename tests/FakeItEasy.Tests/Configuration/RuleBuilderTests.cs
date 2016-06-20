namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Xunit;

    public class RuleBuilderTests
    {
        private readonly RuleBuilder builder;
        private readonly FakeManager fakeManager;

#pragma warning disable 649
        [Fake]
        private IFakeAsserter asserter;

        [Fake]
        private BuildableCallRule ruleProducedByFactory;
#pragma warning restore 649

        public RuleBuilderTests()
        {
            Fake.InitializeFixture(this);

            this.fakeManager = A.Fake<FakeManager>(o => o.CallsBaseMethods());

            this.builder = this.CreateBuilder();
        }

        [Fact]
        public void Returns_called_with_value_sets_applicator_to_a_function_that_applies_that_value_to_interceptor()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IInterceptedFakeObjectCall>();

            returnConfig.Returns(10);

            this.ruleProducedByFactory.Applicator(call);

            A.CallTo(() => call.SetReturnValue(10)).MustHaveHappened();
        }

        [Fact]
        public void Returns_called_with_value_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Returns(10);

            result.Should().Be(this.builder);
        }

        [Fact]
        public void Returns_with_call_function_applies_value_returned_from_function()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(
                new object[] { 1, 2 },
                new[] { "foo", "bar" }));

            returnConfig.ReturnsLazily(x => x.Arguments.Get<int>("bar"));

            this.ruleProducedByFactory.Applicator(call);

            A.CallTo(() => call.SetReturnValue(2)).MustHaveHappened();
        }

        [Fact]
        public void Returns_with_call_function_should_return_delegate()
        {
            var config = this.CreateTestableReturnConfiguration();

            var returned = config.ReturnsLazily(x => x.Arguments.Get<int>(0));

            returned.Should().Be(config.ParentConfiguration);
        }

        [Fact]
        public void Returns_with_call_function_should_be_properly_guarded()
        {
            var config = this.CreateTestableReturnConfiguration();

            Expression<Action> call = () => config.ReturnsLazily(x => x.Arguments.Get<int>(0));
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Throws_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            this.builder.Throws(x => new FormatException());

            var exception = Record.Exception(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IInterceptedFakeObjectCall>()));

            exception.Should().BeAnExceptionOfType<FormatException>();
        }

        [Fact]
        public void Throws_returns_configuration()
        {
            var result = this.builder.Throws(A.Dummy<Func<IFakeObjectCall, Exception>>());

            result.Should().Be(this.builder);
        }

        [Fact]
        public void Throws_called_from_return_value_configuration_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            returnConfig.Throws(x => new FormatException());

            var exception = Record.Exception(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IInterceptedFakeObjectCall>()));

            exception.Should().BeAnExceptionOfType<FormatException>();
        }

        [Fact]
        public void Should_pass_call_to_interceptor_when_throwing_exception()
        {
            // Arrange
            var factory = A.Fake<Func<IFakeObjectCall, Exception>>();
            var call = A.Fake<IInterceptedFakeObjectCall>();
            this.builder.Throws(factory);

            // Act
            var exception = Record.Exception(() => this.ruleProducedByFactory.Applicator(call));

            // Assert
            exception.Should().BeAnExceptionAssignableTo<Exception>();
            A.CallTo(() => factory(call)).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_call_to_interceptor_when_throwing_exception_specified_in_return_value_configuration()
        {
            // Arrange
            var config = this.CreateTestableReturnConfiguration();
            var factory = A.Fake<Func<IFakeObjectCall, Exception>>();
            var call = A.Fake<IInterceptedFakeObjectCall>();
            config.Throws(factory);

            // Act
            var exception = Record.Exception(() => this.ruleProducedByFactory.Applicator(call));

            // Assert
            exception.Should().BeAnExceptionAssignableTo<Exception>();
            A.CallTo(() => factory(call)).MustHaveHappened();
        }

        [Fact]
        public void Throws_called_from_return_value_configuration_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Throws(_ => new InvalidOperationException()) as RuleBuilder;

            result.Should().Be(this.builder);
        }

        [Fact]
        public void NumberOfTimes_sets_number_of_times_to_interceptor()
        {
            this.builder.NumberOfTimes(10);

            this.builder.RuleBeingBuilt.NumberOfTimesToCall.Should().Be(10);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public void NumberOfTimes_throws_when_number_of_times_is_not_a_positive_integer(int numberOfTimes)
        {
            var exception = Record.Exception(() =>
                this.builder.NumberOfTimes(numberOfTimes));

            exception.Should().BeAnExceptionOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void DoesNothing_should_set_applicator_that_does_nothing_when_called()
        {
            this.builder.DoesNothing();

            var call = A.Fake<IInterceptedFakeObjectCall>();

            A.CallTo(call).Throws(new AssertionFailedException("Applicator should do nothing."));

            var exception = Record.Exception(() => this.builder.RuleBeingBuilt.Applicator(call));

            exception.Should().BeNull();
        }

        [Fact]
        public void Does_nothing_should_return_configuration_object()
        {
            var result = this.builder.DoesNothing();

            result.Should().Be(this.builder);
        }

        [Fact]
        public void Invokes_should_return_the_configuration_object()
        {
            var result = this.builder.Invokes(x => { });

            result.Should().BeSameAs(this.builder);
        }

        [Fact]
        public void Invokes_should_add_action_to_list_of_actions()
        {
            Action<IFakeObjectCall> action = x => { };

            this.builder.Invokes(action);

            A.CallTo(() => this.builder.RuleBeingBuilt.Actions.Add(action)).MustHaveHappened();
        }

        [Fact]
        public void Invokes_should_be_null_guarded()
        {
            Action<IFakeObjectCall> action = x => { };
            Expression<Action> call = () => this.builder.Invokes(action);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void Invokes_on_return_value_configuration_should_return_the_configuration_object()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var result = returnConfig.Invokes(x => { });

            result.Should().BeSameAs(returnConfig);
        }

        [Fact]
        public void Invokes_on_return_value_configuration_should_add_action_to_list_of_actions()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            Action<IFakeObjectCall> action = x => { };

            returnConfig.Invokes(action);

            A.CallTo(() => this.builder.RuleBeingBuilt.Actions.Add(action)).MustHaveHappened();
        }

        [Fact]
        public void Invokes_on_return_value_configuration_should_be_null_guarded()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            Action<IFakeObjectCall> action = x => { };
            Expression<Action> call = () => returnConfig.Invokes(action);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void CallsBaseMethod_sets_CallBaseMethod_to_true_on_the_built_rule()
        {
            this.builder.CallsBaseMethod();

            this.builder.RuleBeingBuilt.CallBaseMethod.Should().BeTrue();
        }

        [Fact]
        public void CallBaseMethod_returns_configuration_object()
        {
            var result = this.builder.CallsBaseMethod();

            result.Should().BeSameAs(this.builder);
        }

        [Fact]
        public void CallBaseMethod_sets_the_applicator_to_a_null_action()
        {
            this.builder.CallsBaseMethod();

            this.builder.RuleBeingBuilt.Applicator.Should().NotBeNull();
        }

        [Fact]
        public void CallsBaseMethod_for_function_calls_sets_CallBaseMethod_to_true_on_the_built_rule()
        {
            var config = this.CreateTestableReturnConfiguration();
            config.CallsBaseMethod();

            this.builder.RuleBeingBuilt.CallBaseMethod.Should().BeTrue();
        }

        [Fact]
        public void CallBaseMethod_for_function_calls_returns_configuration_object()
        {
            var config = this.CreateTestableReturnConfiguration();
            var result = config.CallsBaseMethod();

            result.Should().BeSameAs(this.builder);
        }

        [Fact]
        public void CallBaseMethod_for_function_calls_sets_the_applicator_to_a_null_action()
        {
            var config = this.CreateTestableReturnConfiguration();
            config.CallsBaseMethod();

            this.builder.RuleBeingBuilt.Applicator.Should().NotBeNull();
        }

        [Fact]
        public void WhenArgumentsMatches_should_call_UsePredicateToValidateArguments_on_built_rule()
        {
            Func<ArgumentCollection, bool> predicate = x => true;

            var builtRule = A.Fake<BuildableCallRule>();

            var config = this.CreateBuilder(builtRule);
            config.WhenArgumentsMatch(predicate);

            A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
        }

        [Fact]
        public void WhenArgumentsMatches_should_return_self()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = this.CreateBuilder(builtRule);

            config.WhenArgumentsMatch(x => true).Should().BeSameAs(config);
        }

        [Fact]
        public void WhenArgumentsMatches_should_be_null_guarded()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = this.CreateBuilder(builtRule);

            Expression<Action> call = () => config.WhenArgumentsMatch(x => true);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void WhenArgumentsMatches_with_function_call_should_call_UsePredicateToValidateArguments_on_built_rule()
        {
            var builtRule = A.Fake<BuildableCallRule>();
            var config = this.CreateBuilder(builtRule);

            var returnConfig = new RuleBuilder.ReturnValueConfiguration<bool> { ParentConfiguration = config };

            Func<ArgumentCollection, bool> predicate = x => true;

            returnConfig.WhenArgumentsMatch(predicate);

            A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
        }

        [Fact]
        public void WhenArgumentsMatches_with_function_call_should_return_config_should_return_self()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            returnConfig.WhenArgumentsMatch(x => true).Should().BeSameAs(returnConfig);
        }

        [Fact]
        public void WhenArgumentsMatches_with_function_call_should_be_null_guarded()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            Expression<Action> call = () => returnConfig.WhenArgumentsMatch(x => true);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void AssignsOutAndRefParameters_should_be_null_guarded()
        {
            Expression<Action> call = () => this.builder.AssignsOutAndRefParameters();
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void AssignsOutAndRefParameters_should_set_values_to_rule()
        {
            this.builder.AssignsOutAndRefParameters(1, "foo");

            var valueProducer = this.ruleProducedByFactory.OutAndRefParametersValueProducer;
            valueProducer(null).Should().BeEquivalentTo(1, "foo");
        }

        [Fact]
        public void AssignsOutAndRefParameters_returns_self()
        {
            var result = this.builder.AssignsOutAndRefParameters(1, "foo");

            result.Should().BeSameAs(this.builder);
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_should_be_null_guarded()
        {
            Expression<Action> call = () => this.builder.AssignsOutAndRefParametersLazily(null);
            call.Should().BeNullGuarded();
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_should_set_values_to_rule()
        {
            this.builder.AssignsOutAndRefParametersLazily(call => new object[] { 1, "foo" });

            var valueProducer = this.ruleProducedByFactory.OutAndRefParametersValueProducer;
            valueProducer(null).Should().BeEquivalentTo(1, "foo");
        }

        [Fact]
        public void AssignsOutAndRefParametersLazily_returns_self()
        {
            var result = this.builder.AssignsOutAndRefParametersLazily(call => new object[] { 1, "foo" });

            result.Should().BeSameAs(this.builder);
        }

        [Fact]
        public void Assert_with_void_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            A.CallTo(() => this.ruleProducedByFactory.DescriptionOfValidCall).Returns("call description");

            // Act
            this.builder.MustHaveHappened(Repeated.Exactly.Times(99));

            // Assert
            A.CallTo(() => this.asserter.AssertWasCalled(
                A<Func<IFakeObjectCall, bool>>._,
                "call description",
                A<Func<int, bool>>.That.Matches(x => x.Invoke(99)),
                "exactly 99 times"))
                .MustHaveHappened();
        }

        [Fact]
        public void Assert_with_void_call_should_remove_built_rule_from_fake_object()
        {
            // Arrange
            this.fakeManager.AddRuleFirst(this.ruleProducedByFactory);

            // Act
            this.builder.MustHaveHappened();

            // Assert
            this.fakeManager.Rules.Should().BeEmpty();
        }

        [Fact]
        public void Assert_with_function_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            A.CallTo(() => this.ruleProducedByFactory.DescriptionOfValidCall).Returns("call description");

            // Act
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int> { ParentConfiguration = this.builder };
            returnConfig.MustHaveHappened(Repeated.Exactly.Times(99));

            // Assert
            A.CallTo(() => this.asserter.AssertWasCalled(
                A<Func<IFakeObjectCall, bool>>._,
                "call description",
                A<Func<int, bool>>.That.Matches(x => x.Invoke(99)),
                "exactly 99 times"))
                .MustHaveHappened();
        }

        [Fact]
        public void Assert_with_function_call_should_remove_built_rule_from_fake_object()
        {
            // Arrange
            this.fakeManager.AddRuleFirst(this.ruleProducedByFactory);

            // Act
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int> { ParentConfiguration = this.builder };
            returnConfig.MustHaveHappened();

            // Assert
            this.fakeManager.Rules.Should().BeEmpty();
        }

        [Fact]
        public void Where_should_apply_where_predicate_to_built_rule()
        {
            // Arrange
            Func<IFakeObjectCall, bool> predicate = x => true;
            Action<IOutputWriter> writer = x => { };

            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int> { ParentConfiguration = this.builder };

            // Act
            returnConfig.Where(predicate, writer);

            // Assert
            A.CallTo(() => this.ruleProducedByFactory.ApplyWherePredicate(predicate, writer)).MustHaveHappened();
        }

        [Fact]
        public void Where_should_return_the_configuration_object()
        {
            // Arrange
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int> { ParentConfiguration = this.builder };

            // Act

            // Assert
            returnConfig.Where(x => true, x => { }).Should().BeSameAs(returnConfig);
        }

        private RuleBuilder CreateBuilder()
        {
            return this.CreateBuilder(this.ruleProducedByFactory);
        }

        private RuleBuilder CreateBuilder(BuildableCallRule ruleBeingBuilt)
        {
            return new RuleBuilder(ruleBeingBuilt, this.fakeManager, x => this.asserter);
        }

        private RuleBuilder.ReturnValueConfiguration<int> CreateTestableReturnConfiguration()
        {
            return new RuleBuilder.ReturnValueConfiguration<int> { ParentConfiguration = this.builder };
        }
    }
}
