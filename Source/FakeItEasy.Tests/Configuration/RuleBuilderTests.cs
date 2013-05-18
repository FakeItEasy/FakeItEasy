﻿namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class RuleBuilderTests
    {
        private RuleBuilder builder;
        private FakeManager fakeManager;

        [Fake]
        private IFakeAsserter asserter;

        [Fake]
        private BuildableCallRule ruleProducedByFactory;

        private IFakeObjectCallRuleWithDescription RuleWithDescription
        {
            get { return (IFakeObjectCallRuleWithDescription)this.ruleProducedByFactory; }
        }

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        [Test]
        public void Returns_called_with_value_sets_applicator_to_a_function_that_applies_that_value_to_interceptor()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IInterceptedFakeObjectCall>();

            returnConfig.Returns(10);

            this.ruleProducedByFactory.Applicator(call);

            A.CallTo(() => call.SetReturnValue(10)).MustHaveHappened();
        }

        [Test]
        public void Returns_called_with_value_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Returns(10);

            Assert.That(result, Is.EqualTo(this.builder));
        }

        [Test]
        public void Returns_with_call_function_applies_value_returned_from_function()
        {
            var config = this.CreateTestableReturnConfiguration();

            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IInterceptedFakeObjectCall>();
            A.CallTo(() => call.Arguments).Returns(new ArgumentCollection(
                new object[] { 1, 2 },
                new string[] { "foo", "bar" }));

            returnConfig.ReturnsLazily(x => x.Arguments.Get<int>("bar"));

            this.ruleProducedByFactory.Applicator(call);

            A.CallTo(() => call.SetReturnValue(2)).MustHaveHappened();
        }

        [Test]
        public void Returns_with_call_function_should_return_delegate()
        {
            var config = this.CreateTestableReturnConfiguration();

            var returned = config.ReturnsLazily(x => x.Arguments.Get<int>(0));

            Assert.That(returned, Is.EqualTo(config.ParentConfiguration));
        }

        [Test]
        public void Returns_with_call_function_should_be_properly_guarded()
        {
            var config = this.CreateTestableReturnConfiguration();

            NullGuardedConstraint.Assert(() => config.ReturnsLazily(x => x.Arguments.Get<int>(0)));
        }

        [Test]
        public void Throws_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            var exception = new FormatException();

            this.builder.Throws(x => exception);

            Assert.Throws<FormatException>(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IInterceptedFakeObjectCall>()));
        }

        [Test]
        public void Throws_returns_configuration()
        {
            var result = this.builder.Throws(A.Dummy<Func<IFakeObjectCall, Exception>>());

            Assert.That(result, Is.EqualTo(this.builder));
        }

        [Test]
        public void Throws_called_from_return_value_configuration_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var exception = new FormatException();

            returnConfig.Throws(x => exception);

            var thrown = Assert.Throws<FormatException>(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IInterceptedFakeObjectCall>()));
            Assert.That(thrown, Is.SameAs(exception));
        }

        [Test]
        public void Should_pass_call_to_interceptor_when_throwing_exception()
        {
            // Arrange
            var factory = A.Fake<Func<IFakeObjectCall, Exception>>();
            var call = A.Fake<IInterceptedFakeObjectCall>();

            // Act
            this.builder.Throws(factory);

            // Assert
            Assert.Catch(() =>
               this.ruleProducedByFactory.Applicator(call));

            A.CallTo(() => factory(call)).MustHaveHappened();
        }

        [Test]
        public void Should_pass_call_to_interceptor_when_throwing_exception_specified_in_return_value_configuration()
        {
            // Arrange
            var config = this.CreateTestableReturnConfiguration();
            var factory = A.Fake<Func<IFakeObjectCall, Exception>>();
            var call = A.Fake<IInterceptedFakeObjectCall>();

            // Act
            config.Throws(factory);

            // Assert
            Assert.Catch(() =>
               this.ruleProducedByFactory.Applicator(call));

            A.CallTo(() => factory(call)).MustHaveHappened();
        }

        [Test]
        public void Throws_called_from_return_value_configuration_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Throws(_ => new Exception()) as RuleBuilder;

            Assert.That(result, Is.EqualTo(this.builder));
        }

        [Test]
        public void NumberOfTimes_sets_number_of_times_to_interceptor()
        {
            this.builder.NumberOfTimes(10);

            Assert.That(this.builder.RuleBeingBuilt.NumberOfTimesToCall, Is.EqualTo(10));
        }

        [Test]
        public void NumberOfTimes_throws_when_number_of_times_is_not_a_positive_integer(
            [Values(0, -1, -100, int.MinValue)]int numberOftimes)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                this.builder.NumberOfTimes(numberOftimes));
        }

        [Test]
        public void DoesNothing_should_set_applicator_that_does_nothing_when_called()
        {
            this.builder.DoesNothing();

            var call = A.Fake<IInterceptedFakeObjectCall>();
            Any.CallTo(call).Throws(new AssertionException("Applicator should do nothing."));

            this.builder.RuleBeingBuilt.Applicator(call);
        }

        [Test]
        public void Does_nothing_should_return_configuration_object()
        {
            var result = this.builder.DoesNothing();

            Assert.That(result, Is.EqualTo(this.builder));
        }

        [Test]
        public void Invokes_should_return_the_configuration_object()
        {
            var result = this.builder.Invokes(x => { });

            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void Invokes_should_add_action_to_list_of_actions()
        {
            Action<IFakeObjectCall> action = x => { };

            this.builder.Invokes(action);

            A.CallTo(() => this.builder.RuleBeingBuilt.Actions.Add(action)).MustHaveHappened();
        }

        [Test]
        public void Invokes_should_be_null_guarded()
        {
            Action<IFakeObjectCall> action = x => { };
            NullGuardedConstraint.Assert(() =>
                this.builder.Invokes(action));
        }

        [Test]
        public void Invokes_on_return_value_configuration_should_return_the_configuration_object()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var result = returnConfig.Invokes(x => { });

            Assert.That(result, Is.SameAs(returnConfig));
        }

        [Test]
        public void Invokes_on_return_value_configuration_should_add_action_to_list_of_actions()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            Action<IFakeObjectCall> action = x => { };

            returnConfig.Invokes(action);

            A.CallTo(() => this.builder.RuleBeingBuilt.Actions.Add(action)).MustHaveHappened();
        }

        [Test]
        public void Invokes_on_return_value_configuration_should_be_null_guarded()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            Action<IFakeObjectCall> action = x => { };
            NullGuardedConstraint.Assert(() =>
                returnConfig.Invokes(action));
        }

        [Test]
        public void CallsBaseMethod_sets_CallBaseMethod_to_true_on_the_built_rule()
        {
            this.builder.CallsBaseMethod();

            Assert.That(this.builder.RuleBeingBuilt.CallBaseMethod, Is.True);
        }

        [Test]
        public void CallBaseMethod_returns_configuration_object()
        {
            var result = this.builder.CallsBaseMethod();

            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void CallBaseMethod_sets_the_applicator_to_a_null_action()
        {
            this.builder.CallsBaseMethod();

            Assert.That(this.builder.RuleBeingBuilt.Applicator, Is.Not.Null);
        }

        [Test]
        public void CallsBaseMethod_for_function_calls_sets_CallBaseMethod_to_true_on_the_built_rule()
        {
            var config = this.CreateTestableReturnConfiguration();
            config.CallsBaseMethod();

            Assert.That(this.builder.RuleBeingBuilt.CallBaseMethod, Is.True);
        }

        [Test]
        public void CallBaseMethod_for_function_calls_returns_configuration_object()
        {
            var config = this.CreateTestableReturnConfiguration();
            var result = config.CallsBaseMethod();

            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void CallBaseMethod_for_function_calls_sets_the_applicator_to_a_null_action()
        {
            var config = this.CreateTestableReturnConfiguration();
            config.CallsBaseMethod();

            Assert.That(this.builder.RuleBeingBuilt.Applicator, Is.Not.Null);
        }

        [Test]
        public void WhenArgumentsMatches_should_call_UsePredicateToValidateArguments_on_built_rule()
        {
            Func<ArgumentCollection, bool> predicate = x => true;

            var builtRule = A.Fake<BuildableCallRule>();

            var config = this.CreateBuilder(builtRule);
            config.WhenArgumentsMatch(predicate);

            A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
        }

        [Test]
        public void WhenArgumentsMatches_should_return_self()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = this.CreateBuilder(builtRule);

            Assert.That(config.WhenArgumentsMatch(x => true), Is.SameAs(config));
        }

        [Test]
        public void WhenArgumentsMatches_should_be_null_guarded()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = this.CreateBuilder(builtRule);

            NullGuardedConstraint.Assert(() =>
                config.WhenArgumentsMatch(x => true));
        }

        [Test]
        public void WhenArgumentsMatches_with_function_call_should_call_UsePredicateToValidateArguments_on_built_rule()
        {
            var builtRule = A.Fake<BuildableCallRule>();
            var config = this.CreateBuilder(builtRule);

            var returnConfig = new RuleBuilder.ReturnValueConfiguration<bool>() { ParentConfiguration = config };

            Func<ArgumentCollection, bool> predicate = x => true;

            returnConfig.WhenArgumentsMatch(predicate);

            A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
        }

        [Test]
        public void WhenArgumentsMatches_with_function_call_should_return_config_should_return_self()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            Assert.That(returnConfig.WhenArgumentsMatch(x => true), Is.SameAs(returnConfig));
        }

        [Test]
        public void WhenArgumentsMatches_with_function_call_should_be_null_guarded()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            NullGuardedConstraint.Assert(() =>
                returnConfig.WhenArgumentsMatch(x => true));
        }

        [Test]
        public void AssignsOutAndRefParameters_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.builder.AssignsOutAndRefParameters());
        }

        [Test]
        public void AssignsOutAndRefParameters_should_set_values_to_rule()
        {
            this.builder.AssignsOutAndRefParameters(1, "foo");

            Assert.That(this.ruleProducedByFactory.OutAndRefParametersValues, Is.EquivalentTo(new object[] { 1, "foo" }));
        }

        [Test]
        public void AssignsOutAndRefParameters_returns_self()
        {
            var result = this.builder.AssignsOutAndRefParameters(1, "foo");

            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void Assert_with_void_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            A.CallTo(() => this.ruleProducedByFactory.DescriptionOfValidCall).Returns("call description");

            // Act
            this.builder.MustHaveHappened(Repeated.Exactly.Times(99));

            // Assert
            A.CallTo(() => this.asserter.AssertWasCalled(A<Func<IFakeObjectCall, bool>>._, "call description", A<Func<int, bool>>.That.Matches(x => x.Invoke(99)), "exactly 99 times")).MustHaveHappened();
        }

        [Test]
        public void Assert_with_void_call_should_remove_built_rule_from_fake_object()
        {
            // Arrange
            this.fakeManager.AddRuleFirst(this.ruleProducedByFactory);

            // Act
            this.builder.MustHaveHappened();

            // Assert
            Assert.That(this.fakeManager.Rules, Is.Empty);
        }

        [Test]
        public void Assert_with_function_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            A.CallTo(() => this.ruleProducedByFactory.DescriptionOfValidCall).Returns("call description");

            // Act
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };
            returnConfig.MustHaveHappened(Repeated.Exactly.Times(99));

            // Assert
            A.CallTo(() => this.asserter.AssertWasCalled(A<Func<IFakeObjectCall, bool>>._, "call description", A<Func<int, bool>>.That.Matches(x => x.Invoke(99)), "exactly 99 times")).MustHaveHappened();
        }

        [Test]
        public void Assert_with_function_call_should_remove_built_rule_from_fake_object()
        {
            // Arrange
            this.fakeManager.AddRuleFirst(this.ruleProducedByFactory);

            // Act
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };
            returnConfig.MustHaveHappened();

            // Assert
            Assert.That(this.fakeManager.Rules, Is.Empty);
        }

        [Test]
        public void Where_should_apply_where_predicate_to_built_rule()
        {
            // Arrange
            Func<IFakeObjectCall, bool> predicate = x => true;
            Action<IOutputWriter> writer = x => { };

            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };

            // Act
            returnConfig.Where(predicate, writer);

            // Assert
            A.CallTo(() => this.ruleProducedByFactory.ApplyWherePredicate(predicate, writer)).MustHaveHappened();
        }

        [Test]
        public void Where_should_return_the_configuration_object()
        {
            // Arrange
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };

            // Act

            // Assert
            Assert.That(returnConfig.Where(x => true, x => { }), Is.SameAs(returnConfig));
        }

        protected virtual void OnSetUp()
        {
            Fake.InitializeFixture(this);

            this.fakeManager = new FakeManager();

            this.builder = this.CreateBuilder();
        }

        private static Expression<Func<TFake, TMember>> CreateExpression<TFake, TMember>(Expression<Func<TFake, TMember>> expression) where TFake : class
        {
            return expression;
        }

        private static Expression<Action<TFake>> CreateExpression<TFake>(Expression<Action<TFake>> expression)
            where TFake : class
        {
            return expression;
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
            return new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };
        }
    }
}
