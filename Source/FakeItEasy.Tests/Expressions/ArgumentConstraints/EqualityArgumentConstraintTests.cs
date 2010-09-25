namespace FakeItEasy.Tests.Expressions.ArgumentConstraints
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Expressions;
    using FakeItEasy.Expressions.ArgumentConstraints;
    using NUnit.Framework;

    [TestFixture]
    public class EqualityArgumentConstraintTests
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.constraintField = new EqualityArgumentConstraint(1);
        }

        protected override IEnumerable<object> InvalidValues
        {
            get 
            {
                return new[] { null, new object(), Guid.NewGuid(), "FOO", " foo " }; 
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { 1 }; }
        }

        protected override string ExpectedDescription
        {
            get { return "1"; }
        }

        [Test]
        public override void Constraint_should_provide_correct_description()
        {
            Assert.That(this.constraintField.ToString(), Is.EqualTo("1"));
        }

        [Test]
        public void ToString_should_return_NULL_when_expected_value_is_null()
        {
            var validator = new EqualityArgumentConstraint(null);

            Assert.That(validator.ToString(), Is.EqualTo("<NULL>"));
        }

        [Test]
        public void ToString_should_put_accents_when_expected_value_is_string()
        {
            var validator = new EqualityArgumentConstraint("foo");

            Assert.That(validator.ToString(), Is.EqualTo("\"foo\""));
        }
    }

    [TestFixture]
    public class GenericEqualityArgumentConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        
        protected override IEnumerable<object> InvalidValues
        {
            get 
            {
                yield return "unexpected value";
                yield return 1;
                yield return null;
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get 
            {
                return new[] { "expected value" };
            }
        }

        protected override string ExpectedDescription
        {
            get 
            {
                return "expected value";
            }
        }

        protected override ArgumentConstraint<object> CreateConstraint(FakeItEasy.Expressions.ArgumentConstraintScope<object> scope)
        {
            return new EqualityArgumentConstraint<object>(scope, "expected value");
        }


        [Test]
        public void Should_handle_null_as_expected_value()
        {
            // Arrange
            var constraint = new EqualityArgumentConstraint<object>(GetScopeForTesting<object>(), null);

            // Act
            var result = constraint.IsValid(null);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Should_give_correct_description_when_expected_value_is_null()
        {
            // Arrange
            var constraint = new EqualityArgumentConstraint<object>(GetScopeForTesting<object>(), null);

            // Act

            // Assert
            Assert.That(constraint.FullDescription, Is.EqualTo("NULL"));
        }
    }
}