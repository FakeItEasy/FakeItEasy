namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class RootValidationsTests
    {
        private RootValidations<int> CreateValidations()
        {
            return new RootValidations<int>();
        }

        [Test]
        public void IsValid_should_return_true()
        {
            // Arrange
            var validations = this.CreateValidations();

            // Act

            // Assert
            Assert.That(validations.IsValid(10), Is.True);
        }

        [Test]
        public void ToString_should_return_empty_string()
        {
            // Arrange
            var validations = this.CreateValidations();

            // Act
            var description = validations.ToString();

            // Assert
            Assert.That(description, Is.Empty);
        }

        [TestCase(true, Result = true)]
        [TestCase(false, Result = false)]
        public bool ResultOfChildValidator_should_return_true_when_child_is_valid(bool validatorIsValid)
        {
            // Arrange
            var validations = this.CreateValidations();

            // Act

            // Assert
            return validations.ResultOfChildConstraintIsValid(validatorIsValid);
        }
    }
}
