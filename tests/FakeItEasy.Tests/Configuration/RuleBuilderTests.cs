namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class RuleBuilderTests
    {
        private readonly RuleBuilder builder;
        private readonly FakeManager fakeManager;
        private readonly IFakeAsserter asserter;
        private readonly BuildableCallRule ruleProducedByFactory;

        public RuleBuilderTests()
        {
            this.asserter = A.Fake<IFakeAsserter>();
            this.ruleProducedByFactory = A.Fake<BuildableCallRule>();

            this.fakeManager = A.Fake<FakeManager>(o => o.CallsBaseMethods());

            this.builder = this.CreateBuilder();
        }

        public static IEnumerable<object[]> BehaviorDefinitionActionsForVoid =>
            TestCases.FromObject<Action<IVoidArgumentValidationConfiguration>>(
                configuration => configuration.CallsBaseMethod(),
                configuration => configuration.DoesNothing(),
                configuration => configuration.Throws<Exception>(),
                configuration => configuration.Invokes(DoNothing),
                configuration => configuration.AssignsOutAndRefParametersLazily(_ => new object[0]));

        public static IEnumerable<object[]> BehaviorDefinitionActionsForNonVoid =>
            TestCases.FromObject<Action<IAnyCallConfigurationWithReturnTypeSpecified<int>>>(
                configuration => configuration.CallsBaseMethod(),
                configuration => configuration.Throws<Exception>(),
                configuration => configuration.Invokes(DoNothing),
                configuration => configuration.ReturnsLazily(_ => 0));

        public static IEnumerable<object[]> CallSpecificationActionsForNonVoid =>
            TestCases.FromObject<Action<IAnyCallConfigurationWithReturnTypeSpecified<int>>>(
                configuration => configuration.WhenArgumentsMatch(args => true),
                configuration => configuration.Where(call => true));

        public static IEnumerable<object[]> CallSpecificationActionsForVoid =>
            TestCases.FromObject<Action<IVoidArgumentValidationConfiguration>>(
                configuration => configuration.WhenArgumentsMatch(args => true));

        [Fact]
        public void Returns_with_call_function_should_be_properly_guarded()
        {
            var config = this.CreateTestableReturnConfiguration();

            Expression<Action> call = () => config.ReturnsLazily(x => x.Arguments.Get<int>(0));
            call.Should().BeNullGuarded();
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
        public void CallsBaseMethod_for_function_calls_sets_CallBaseMethod_to_true_on_the_built_rule()
        {
            var config = this.CreateTestableReturnConfiguration();
            config.CallsBaseMethod();

            this.builder.RuleBeingBuilt.CallBaseMethod.Should().BeTrue();
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

            var returnConfig = new RuleBuilder.ReturnValueConfiguration<bool>(config);

            Func<ArgumentCollection, bool> predicate = x => true;

            returnConfig.WhenArgumentsMatch(predicate);

            A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
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
        public void Assert_with_void_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            var repeatedConstraint = Repeated.Exactly.Times(99);

            // Act
            this.builder.MustHaveHappened(repeatedConstraint);

            // Assert
            A.CallTo(() => this.asserter.AssertWasCalled(
                A<Func<ICompletedFakeObjectCall, bool>>._,
                this.ruleProducedByFactory.WriteDescriptionOfValidCall,
                repeatedConstraint))
                .MustHaveHappened();
        }

        [Fact]
        public void Assert_with_function_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            var repeatedConstraint = Repeated.Exactly.Times(99);

            // Act
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>(this.builder);
            returnConfig.MustHaveHappened(repeatedConstraint);

            // Assert
            A.CallTo(() => this.asserter.AssertWasCalled(
                A<Func<ICompletedFakeObjectCall, bool>>._,
                this.ruleProducedByFactory.WriteDescriptionOfValidCall,
                repeatedConstraint))
                .MustHaveHappened();
        }

        [Fact]
        public void Where_should_apply_where_predicate_to_built_rule()
        {
            // Arrange
            Func<IFakeObjectCall, bool> predicate = x => true;
            Action<IOutputWriter> writer = x => { };

            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>(this.builder);

            // Act
            returnConfig.Where(predicate, writer);

            // Assert
            A.CallTo(() => this.ruleProducedByFactory.ApplyWherePredicate(predicate, writer)).MustHaveHappened();
        }

        [Theory]
        [MemberData(nameof(CallSpecificationActionsForVoid))]
        public void Call_specification_method_for_void_should_not_add_rule_to_manager(Action<IVoidArgumentValidationConfiguration> configurationAction)
        {
            // Arrange
            var initialRules = this.fakeManager.Rules.ToList();

            // Act
            configurationAction(this.builder);

            // Assert
            this.fakeManager.Rules.Should().Equal(initialRules);
        }

        [Theory]
        [MemberData(nameof(CallSpecificationActionsForNonVoid))]
        public void Call_specification_method_for_non_void_should_not_add_rule_to_manager(Action<IAnyCallConfigurationWithReturnTypeSpecified<int>> configurationAction)
        {
            // Arrange
            var initialRules = this.fakeManager.Rules.ToList();
            var returnConfig = this.CreateTestableReturnConfiguration();

            // Act
            configurationAction(returnConfig);

            // Assert
            this.fakeManager.Rules.Should().Equal(initialRules);
        }

        [Theory]
        [MemberData(nameof(BehaviorDefinitionActionsForVoid))]
        public void Behavior_definition_method_for_void_should_add_rule_to_manager(Action<IVoidArgumentValidationConfiguration> configurationAction)
        {
            // Arrange
            var initialRules = this.fakeManager.Rules.ToList();

            // Act
            configurationAction(this.builder);

            // Assert
            this.fakeManager.Rules.Should().Equal(new[] { this.ruleProducedByFactory }.Concat(initialRules));
        }

        [Theory]
        [MemberData(nameof(BehaviorDefinitionActionsForNonVoid))]
        public void Behavior_definition_method_for_non_void_should_add_rule_to_manager(Action<IAnyCallConfigurationWithReturnTypeSpecified<int>> configurationAction)
        {
            // Arrange
            var initialRules = this.fakeManager.Rules.ToList();
            var returnConfig = this.CreateTestableReturnConfiguration();

            // Act
            configurationAction(returnConfig);

            // Assert
            this.fakeManager.Rules.Should().Equal(new[] { this.ruleProducedByFactory }.Concat(initialRules));
        }

        private static void DoNothing()
        {
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
            return new RuleBuilder.ReturnValueConfiguration<int>(this.builder);
        }
    }
}