namespace FakeItEasy.Tests.VisualBasic
{
    using NUnit.Framework;

    [TestFixture]
    public class VisualBasicRuleBuilderTests
    {
        [Test]
        public void AssertWasCalled_should_set_IsAssertion_to_true_of_recorded_rule()
        {
            //var config = this.CreateBuilder(new RecordedCallRule(A.Fake<MethodInfoManager>()));
            //config.AssertWasCalled(x => true);

            //Assert.That(((RecordedCallRule)config.RuleBeingBuilt).IsAssertion, Is.True);
        }

        [Test]
        public void AssertWasCalled_should_set_applicator_to_empty_action()
        {
            //// Arrange
            //var config = this.CreateBuilder(new RecordedCallRule(A.Fake<MethodInfoManager>()));
            //config.AssertWasCalled(x => true);

            //// Act

            //// Assert
            //Assert.That(config.RuleBeingBuilt.Applicator, Is.Not.Null);
        }

        [Test]
        public void AssertWasCalled_should_set_repeat_predicate_to_the_recorded_rule()
        {
            //Func<int, bool> repeatPredicate = x => true;
            //var config = this.CreateBuilder(new RecordedCallRule(A.Fake<MethodInfoManager>()));
            //config.AssertWasCalled(repeatPredicate);

            //Assert.That(((RecordedCallRule)config.RuleBeingBuilt).RepeatPredicate, Is.SameAs(repeatPredicate));
        }

        [Test]
        public void AssertWasCalled_should_throw_when_built_rule_is_not_a_RecordedCallRule()
        {
            //var ex = Assert.Throws<InvalidOperationException>(() =>
            //    this.builder.AssertWasCalled(x => true));

            //Assert.That(ex.Message, Is.EqualTo("Only RecordedCallRules can be used for assertions."));
        }

        [Test]
        public void AssertWasCalled_should_be_null_guarded()
        {
            //NullGuardedConstraint.Assert(() =>
            //    this.builder.AssertWasCalled(x => true));
        }

        //
        [Test]
        public void WhenArgumentsMatches_from_VB_should_be_null_guarded()
        {
            //var builtRule = A.Fake<BuildableCallRule>();

            //var config = this.CreateBuilder(builtRule) as FakeItEasy.VisualBasic.IVisualBasicConfigurationWithArgumentValidation;

            //NullGuardedConstraint.Assert(() =>
            //    config.WhenArgumentsMatch(x => true));
        }

        [Test]
        public void WhenArgumentsMatches_from_VB_should_return_configuration_object()
        {
            //var builtRule = A.Fake<BuildableCallRule>();

            //var config = this.CreateBuilder(builtRule) as FakeItEasy.VisualBasic.IVisualBasicConfigurationWithArgumentValidation;

            //var returned = config.WhenArgumentsMatch(x => true);

            //Assert.That(returned, Is.SameAs(config));
        }

        [Test]
        public void WhenArgumentsMatches_from_VB_should_set_predicate_to_built_rule()
        {
            //var builtRule = A.Fake<BuildableCallRule>();

            //var config = this.CreateBuilder(builtRule) as FakeItEasy.VisualBasic.IVisualBasicConfigurationWithArgumentValidation;

            //Func<ArgumentCollection, bool> predicate = x => true;

            //config.WhenArgumentsMatch(predicate);

            //A.CallTo(() => builtRule.UsePredicateToValidateArguments(predicate)).MustHaveHappened(Repeated.Once);
        }
    }
}
