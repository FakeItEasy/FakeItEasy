namespace FakeItEasy.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class GenericATests
    {
        [Fact]
        public void That_should_return_root_validations()
        {
            // Arrange

            // Act
            var validations = A<string>.That;

            // Assert
            validations.Should().BeOfType<DefaultArgumentConstraintManager<string>>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("hello world")]
        [InlineData("foo")]
        public void Ignored_should_return_validator_that_passes_any_argument(string argument)
        {
            // Arrange

            // Act
            var isValid = GetIgnoredConstraint<string>().IsValid(argument);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Ignored_should_return_validator_with_correct_description()
        {
            // Arrange
            var result = new StringBuilder();

            // Act
            GetIgnoredConstraint<string>().WriteDescription(new StringBuilderOutputWriter(result));

            // Assert
            result.ToString().Should().Be("<Ignored>");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("hello world")]
        [InlineData("foo")]
        public void Underscore_should_return_validator_that_passes_any_argument(string argument)
        {
            // Arrange

            // Act
            var isValid = GetUnderscoreConstraint<string>().IsValid(argument);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Underscore_should_return_validator_with_correct_description()
        {
            // Arrange
            var result = new StringBuilder();

            // Act
            GetUnderscoreConstraint<string>().WriteDescription(new StringBuilderOutputWriter(result));

            // Assert
            result.ToString().Should().Be("<Ignored>");
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ignored", Justification = "Required for testing.")]
        private static IArgumentConstraint GetIgnoredConstraint<T>()
        {
            var trap = ServiceLocator.Current.Resolve<IArgumentConstraintTrapper>();
            return trap.TrapConstraints(() => { var ignored = A<string>.Ignored; }).Single();
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ignored", Justification = "Required for testing.")]
        private static IArgumentConstraint GetUnderscoreConstraint<T>()
        {
            var trap = ServiceLocator.Current.Resolve<IArgumentConstraintTrapper>();
            return trap.TrapConstraints(() => { var ignored = A<string>._; }).Single();
        }
    }
}
