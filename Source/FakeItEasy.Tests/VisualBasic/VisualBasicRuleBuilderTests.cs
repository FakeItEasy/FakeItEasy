namespace FakeItEasy.Tests.VisualBasic
{
    using NUnit.Framework;
    using FakeItEasy.VisualBasic;
    using FakeItEasy.Configuration;
using FakeItEasy.Api;
    using FakeItEasy.Assertion;

    [TestFixture]
    public class VisualBasicRuleBuilderTests
    {
        private VisualBasicRuleBuilder builder;
        private RuleBuilder wrappedBuilder;
        private RecordedCallRule rule;

        [SetUp]
        public void SetUp()
        {
            this.wrappedBuilder = A.Fake<RuleBuilder>(x => x.WithArgumentsForConstructor(() => 
                new RuleBuilder(A.Fake<BuildableCallRule>(), A.Fake<FakeObject>(), c => A.Fake<FakeAsserter>())));

            this.builder = new VisualBasicRuleBuilder();
        }

        [Test]
        public void MustHaveHappened_should_set_IsAssertion_to_true_of_recorded_rule()
        {
            // Arrange
            
            // Act
            this.builder.MustHaveHappened(Repeated.Once);

            // Assert
            Assert.That(this.rule.IsAssertion, Is.True);
        }

        [Test]
        public void MustHaveHappened_should_set_applicator_to_empty_action()
        {
            //// Arrange
            //var config = this.CreateBuilder(new RecordedCallRule(A.Fake<MethodInfoManager>()));
            //config.MustHaveHappened(x => true);

            //// Act

            //// Assert
            //Assert.That(config.RuleBeingBuilt.Applicator, Is.Not.Null);
        }

        [Test]
        public void MustHaveHappened_should_set_repeat_predicate_to_the_recorded_rule()
        {
            //Func<int, bool> repeatPredicate = x => true;
            //var config = this.CreateBuilder(new RecordedCallRule(A.Fake<MethodInfoManager>()));
            //config.MustHaveHappened(repeatPredicate);

            //Assert.That(((RecordedCallRule)config.RuleBeingBuilt).RepeatPredicate, Is.SameAs(repeatPredicate));
        }

        [Test]
        public void MustHaveHappened_should_throw_when_built_rule_is_not_a_RecordedCallRule()
        {
            //var ex = Assert.Throws<InvalidOperationException>(() =>
            //    this.builder.MustHaveHappened(x => true));

            //Assert.That(ex.Message, Is.EqualTo("Only RecordedCallRules can be used for assertions."));
        }

        [Test]
        public void MustHaveHappened_should_be_null_guarded()
        {
            //NullGuardedConstraint.Assert(() =>
            //    this.builder.MustHaveHappened(x => true));
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
