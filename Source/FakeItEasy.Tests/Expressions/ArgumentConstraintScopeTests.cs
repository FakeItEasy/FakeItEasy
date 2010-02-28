namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class ArgumentConstraintScopeTests
    {
        private ArgumentConstraintScope<T> CreateValidations<T>()
        {
            return new TestableValidations<T>();
        }

        private class TestableValidations<T>
            : ArgumentConstraintScope<T>
        {
            public bool IsValidReturnValue = true;
            
            internal override bool IsValid(T argument)
            {
                return this.IsValidReturnValue;
            }

            internal override bool ResultOfChildValidatorIsValid(bool result)
            {
                return result;
            }
        }

        [TestCase(10, Result = true)]
        [TestCase(9, Result = false)]
        public bool Matches_should_return_validator_that_delegates_to_predicate(int value)
        {
            // Arrange
            Func<int, bool> predicate = x => x == 10;
            var validations = this.CreateValidations<int>();

            // Act
            
            // Assert
            return validations.Matches(predicate).IsValid(value);
        }

        [Test]
        public void Matches_should_pass_this_into_constructor()
        {
            // Arrange
            var validations = this.CreateValidations<int>();

            // Act
            var validator = validations.Matches(x => true);

            // Assert
            Assert.That(validator.Scope, Is.SameAs(validations));
        }

        [Test]
        public void Matches_should_return_validator_that_has_correct_description()
        {
            // Arrange
            var validations = this.CreateValidations<int>();

            // Act
            var validator = validations.Matches(x => true);

            // Assert
            Assert.That(validator.ToString(), Is.EqualTo("<Predicate>"));
        }

        [Test]
        public void IsInstanceOf_should_pass_this_into_constructor()
        {
            // Arrange
            var validations = this.CreateValidations<object>();

            // Act
            var validator = validations.IsInstanceOf<string>();

            // Assert
            Assert.That(validator.Scope, Is.SameAs(validations));
        }

        [Test]
        public void Not_should_return_not_validator_with_parent_set_to_current_validator()
        {
            // Arrange
            var validations = this.CreateValidations<string>();

            // Act
            var not = validations.Not as NotArgumentValidatorScope<string>;

            // Assert
            Assert.That(not.ParentValidations, Is.SameAs(validations));
        }

        [Test]
        public void ToString_should_return_empty_string()
        {
            // Arrange
            var validations = this.CreateValidations<int>();

            // Act
            var description = validations.ToString();

            // Assert
            Assert.That(description, Is.Empty);
        }
    }
}
