namespace FakeItEasy.Tests.Configuration
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class RecordingRuleBuilderTests
    {
        private RecordingRuleBuilder builder;
        private RuleBuilder wrappedBuilder;
        private RecordedCallRule rule;

        [SetUp]
        public void Setup()
        {
            this.wrappedBuilder = A.Fake<RuleBuilder>(x => x.WithArgumentsForConstructor(() =>
                new RuleBuilder(A.Fake<BuildableCallRule>(), A.Fake<FakeManager>(), c => A.Fake<IFakeAsserter>())));

            this.rule = A.Fake<RecordedCallRule>();

            this.builder = new RecordingRuleBuilder(this.rule, this.wrappedBuilder);
        }

        [Test]
        public void IsAssertion_should_not_be_set_when_MustHaveHappened_has_not_been_called()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(this.rule.IsAssertion, Is.False);
        }

        [Test]
        public void MustHaveHappened_should_set_IsAssertion_to_true_of_recorded_rule()
        {
            // Arrange

            // Act
            this.builder.MustHaveHappened();

            // Assert
            Assert.That(this.rule.IsAssertion, Is.True);
        }

        [Test]
        public void MustHaveHappened_should_set_applicator_to_empty_action()
        {
            // Arrange

            // Act
            this.builder.MustHaveHappened();

            // Assert
            Assert.That(this.rule.Applicator, Is.Not.Null);
        }

        [Test]
        public void MustHaveHappened_should_set_repeat_predicate_to_the_recorded_rule()
        {
            // Arrange
            var repeatPredicate = Repeated.AtLeast.Once;

            // Act
            this.builder.MustHaveHappened(repeatPredicate);

            // Assert
            Assert.That(this.rule.RepeatConstraint, Is.SameAs(repeatPredicate));
        }

        [Test]
        public void MustHaveHappened_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.builder.MustHaveHappened(Repeated.AtLeast.Once));
        }

        [Test]
        public void WhenArgumentsMatches_from_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.builder.WhenArgumentsMatch(x => true));
        }

        [Test]
        public void WhenArgumentsMatches_from_should_return_configuration_object()
        {
            // Arrange

            // Act
            var returned = this.builder.WhenArgumentsMatch(x => true);

            // Assert
            Assert.That(returned, Is.SameAs(this.builder));
        }

        [Test]
        public void WhenArgumentsMatches_from_VB_should_set_predicate_to_built_rule()
        {
            // Arrange
            Func<ArgumentCollection, bool> predicate = x => true;

            // Act
            this.builder.WhenArgumentsMatch(predicate);

            // Assert
            A.CallTo(() => this.rule.UsePredicateToValidateArguments(predicate)).MustHaveHappened();
        }

        [Test]
        public void DoesNothing_should_delegate_to_wrapped_builder()
        {
            // Arrange
            var config = A.Fake<IAfterCallSpecifiedConfiguration>();
            A.CallTo(() => this.wrappedBuilder.DoesNothing()).Returns(config);

            // Act
            var returned = this.builder.DoesNothing();

            // Assert
            Assert.That(returned, Is.SameAs(config));
        }

        [Test]
        public void Throws_should_delegate_to_wrapped_builder()
        {
            // Arrange
            var exceptionFactory = A.Dummy<Func<IFakeObjectCall, Exception>>();

            var config = A.Fake<IAfterCallSpecifiedConfiguration>();
            A.CallTo(() => this.wrappedBuilder.Throws(exceptionFactory)).Returns(config);

            // Act
            var returned = this.builder.Throws(exceptionFactory);

            // Assert
            Assert.That(returned, Is.SameAs(config));
        }

        [Test]
        public void Invokes_should_delegate_to_wrapped_builder()
        {
            // Arrange
            Action<IFakeObjectCall> action = x => { };

            var config = A.Fake<IVoidConfiguration>();
            A.CallTo(() => this.wrappedBuilder.Invokes(action)).Returns(config);

            // Act
            var returned = this.builder.Invokes(action);

            // Assert
            Assert.That(returned, Is.SameAs(config));
        }

        [Test]
        public void CallsBaseMethod_should_delegate_to_wrapped_builder()
        {
            // Arrange
            var config = A.Fake<IAfterCallSpecifiedConfiguration>();
            A.CallTo(() => this.wrappedBuilder.CallsBaseMethod()).Returns(config);

            // Act
            var returned = this.builder.CallsBaseMethod();

            // Assert
            Assert.That(returned, Is.SameAs(config));
        }

        [Test]
        public void AssignsOutAndRefParametersLazily_delegates_to_wrapped_builder()
        {
            // Arrange
            Func<IFakeObjectCall, object[]> valueProducer = x => new object[] { "foo", "bar" };

            var config = A.Fake<IAfterCallSpecifiedConfiguration>();
            A.CallTo(() => this.wrappedBuilder.AssignsOutAndRefParametersLazily(valueProducer)).Returns(config);

            // Act
            var returned = this.builder.AssignsOutAndRefParametersLazily(valueProducer);

            // Assert
            Assert.That(returned, Is.SameAs(config));
        }
    }
}
