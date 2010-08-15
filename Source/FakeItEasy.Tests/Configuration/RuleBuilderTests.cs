namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Configuration;
    using NUnit.Framework;
    
    [TestFixture]
    public class RuleBuilderTests
    {
        private RuleBuilder builder;
        private BuildableCallRule ruleProducedByFactory;
        private FakeObject fakeObject;
        private FakeAsserter asserter;

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        protected virtual void OnSetUp()
        {
            this.ruleProducedByFactory = A.Fake<BuildableCallRule>();
            this.fakeObject = new FakeObject();
            this.asserter = A.Fake<FakeAsserter>();

            this.builder = this.CreateBuilder();
        }

        private RuleBuilder CreateBuilder()
        {
            return new RuleBuilder(this.ruleProducedByFactory, this.fakeObject, x => this.asserter);
        }

        private RuleBuilder CreateBuilder(BuildableCallRule ruleBeingBuilt)
        {
            return new RuleBuilder(ruleBeingBuilt, this.fakeObject, x => this.asserter);
        }

     
        [Test] public void 
        Returns_called_with_value_sets_applicator_to_a_function_that_applies_that_value_to_interceptor()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IInterceptedFakeObjectCall>();

            returnConfig.Returns(10);

            this.ruleProducedByFactory.Applicator(call);

            A.CallTo(() => call.SetReturnValue(10)).MustHaveHappened(Repeated.Once);
        }

        [Test] public void 
        Returns_called_with_value_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Returns(10);

            Assert.That(result, Is.EqualTo(this.builder));
        }

        [Test]
        public void Returns_called_with_delegate_sets_return_value_produced_by_delegate_each_time()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IInterceptedFakeObjectCall>();
            int i = 1;
            
            returnConfig.Returns(() => i++);

            this.ruleProducedByFactory.Applicator(call);
            this.ruleProducedByFactory.Applicator(call);

            A.CallTo(() => call.SetReturnValue(1)).MustHaveHappened();
            A.CallTo(() => call.SetReturnValue(2)).MustHaveHappened();
        }

        [Test]
        public void Returns_with_delegate_throws_when_delegate_is_null()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            
            Assert.Throws<ArgumentNullException>(() =>
                returnConfig.Returns((Func<int>)null));
        }

        [Test]
        public void Returns_called_with_delegate_returns_parent_cofiguration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Returns(() => 10);

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

            returnConfig.Returns(x => x.Arguments.Get<int>("bar"));

            this.ruleProducedByFactory.Applicator(call);

            A.CallTo(() => call.SetReturnValue(2)).MustHaveHappened();
        }

        [Test]
        public void Returns_with_call_function_should_return_delegate()
        {
            var config = this.CreateTestableReturnConfiguration();

            var returned = config.Returns(x => x.Arguments.Get<int>(0));

            Assert.That(returned, Is.EqualTo(config.ParentConfiguration));
        }

        [Test]
        public void Returns_with_call_function_should_be_properly_guarded()
        {
            var config = this.CreateTestableReturnConfiguration();

            NullGuardedConstraint.Assert(() => config.Returns(x => x.Arguments.Get<int>(0)));
        }

        [Test]
        public void Throws_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            var exception = new FormatException();

            this.builder.Throws(exception);

            Assert.Throws<FormatException>(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IInterceptedFakeObjectCall>()));
        }

        [Test]
        public void Throws_returns_configuration()
        {
            var result = this.builder.Throws(new Exception());

            Assert.That(result, Is.EqualTo(this.builder));
        }

        [Test]
        public void Throws_called_from_return_value_configuration_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            
            var exception = new FormatException();

            returnConfig.Throws(exception);
            
            var thrown = Assert.Throws<FormatException>(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IInterceptedFakeObjectCall>()));
            Assert.That(thrown, Is.SameAs(exception));
        }

        [Test]
        public void Throws_called_from_return_value_configuration_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Throws(new Exception()) as RuleBuilder;

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

            A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened(Repeated.Once);
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
           
            A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened(Repeated.Once);
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
            var result =this.builder.AssignsOutAndRefParameters(1, "foo");

            Assert.That(result, Is.SameAs(this.builder));
        }

        [Test]
        public void Assert_with_void_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            A.CallTo(() => this.ruleProducedByFactory.ToString()).Returns("call description");

            // Act
            this.builder.MustHaveHappened(Repeated.Times(99).Exactly);

            // Assert
            var repeatMatcher = A<Func<int, bool>>.That.Matches(x => x.Invoke(99) == true);

            A.CallTo(() => this.asserter.AssertWasCalled(A<Func<IFakeObjectCall, bool>>.Ignored, "call description", repeatMatcher, "exactly #99 times")).MustHaveHappened();
        }

        [Test]
        public void Assert_with_void_call_should_remove_built_rule_from_fake_object()
        {
            // Arrange
            this.fakeObject.AddRuleFirst(this.ruleProducedByFactory);

            // Act
            this.builder.MustHaveHappened(Repeated.Once);

            // Assert
            Assert.That(this.fakeObject.Rules, Is.Empty);
        }

        [Test]
        public void Assert_with_function_call_should_assert_on_assertions_produced_by_factory()
        {
            // Arrange
            A.CallTo(() => this.ruleProducedByFactory.ToString()).Returns("call description");

            // Act
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };
            returnConfig.MustHaveHappened(Repeated.Times(99).Exactly);

            // Assert
            var repeatMatcher = A<Func<int, bool>>.That.Matches(x => x.Invoke(99) == true);

            A.CallTo(() => this.asserter.AssertWasCalled(A<Func<IFakeObjectCall, bool>>.Ignored, "call description", repeatMatcher, "exactly #99 times")).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void Assert_with_function_call_should_remove_built_rule_from_fake_object()
        {
            // Arrange
            this.fakeObject.AddRuleFirst(this.ruleProducedByFactory);

            // Act
            var returnConfig = new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };
            returnConfig.MustHaveHappened(Repeated.Once);

            // Assert
            Assert.That(this.fakeObject.Rules, Is.Empty);
        }

        private RuleBuilder.ReturnValueConfiguration<int> CreateTestableReturnConfiguration()
        {
            return new RuleBuilder.ReturnValueConfiguration<int>() { ParentConfiguration = this.builder };
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
    }
}
