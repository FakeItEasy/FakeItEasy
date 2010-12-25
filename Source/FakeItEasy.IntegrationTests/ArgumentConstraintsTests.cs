namespace FakeItEasy.IntegrationTests
{
    using FakeItEasy.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentConstraintsTests
    {
        [Test]
        public void Not_should_reverse_IsValid()
        {
            // Arrange
            var validator = A<string>.That.Not.IsNull();

            // Act

            // Assert
            Assert.That(validator.IsValid(null), Is.False);
            Assert.That(validator.IsValid("foo"), Is.True);
        }

        [Test]
        public void Constraints_should_be_combinable_by_and()
        {
            // Arrange
            var validator = A<object>.That.Not.IsNull().And.IsInstanceOf<string>().And.Matches(x => ((string)x).StartsWith("foo"));

            // Act
            
            // Assert
            Assert.That(validator.IsValid("foo bar"));
        }

        [Test]
        public void Validators_should_be_combinable_by_or()
        {
            // Arrange
            var validator = A<string>.That.StartsWith("foo").Or(A<string>.That.StartsWith("bar"));

            // Act

            // Assert
            Assert.That(validator.IsValid("foo..."));
            Assert.That(validator.IsValid("bar..."));
        }

        [Test]
        public void Or_should_provide_overload_with_validator_lambda()
        {
            // Arrange
            var validator = A<string>.That.StartsWith("foo").Or(x => StringArgumentConstraints.StartsWith(x, "bar"));
            
            // Act

            // Assert
            Assert.That(validator.IsValid("foo"));
            Assert.That(validator.IsValid("bar"));
        }
    }
}