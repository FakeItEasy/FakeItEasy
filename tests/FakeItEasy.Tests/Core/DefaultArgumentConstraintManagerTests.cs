namespace FakeItEasy.Tests.Core
{
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class DefaultArgumentConstraintManagerTests
    {
        private readonly DefaultArgumentConstraintManager<string> constraintManager;
        private IArgumentConstraint createdConstraint;

        public DefaultArgumentConstraintManagerTests()
        {
            this.createdConstraint = null;
            this.constraintManager = new DefaultArgumentConstraintManager<string>(x => this.createdConstraint = x);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Should_add_constraint_that_is_validated_by_the_specified_predicate(bool predicateIsValid, bool expectedResult)
        {
            // Arrange

            // Act
            this.constraintManager.Matches(x => predicateIsValid, x => x.Write("foo"));

            // Assert
            this.createdConstraint.IsValid("foo").Should().Be(expectedResult);
        }

        [Fact]
        public void Should_create_constraint_that_passes_argument_to_delegate()
        {
            // Arrange
            string argumentPassedToDelegate = null;

            // Act
            this.constraintManager.Matches(
                x =>
                {
                    argumentPassedToDelegate = x;
                    return true;
                },
                x => x.Write("foo"));

            // Assert
            this.createdConstraint.IsValid("argument");
            argumentPassedToDelegate.Should().Be("argument");
        }

        [Fact]
        public void Should_create_constraint_that_passes_writer_to_writer_delegate()
        {
            // Arrange
            IOutputWriter passedInWriter = null;

            var writerFromOutside = A.Dummy<IOutputWriter>();

            // Act
            this.constraintManager.Matches(x => true, x => passedInWriter = x);

            // Assert
            this.createdConstraint.WriteDescription(writerFromOutside);
            passedInWriter.Should().Be(writerFromOutside);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void Should_add_constraint_that_is_validated_inversed_by_the_specified_predicate_when_prefixing_with_not(bool predicateIsValid, bool expectedResult)
        {
            // Arrange

            // Act
            this.constraintManager.Not.Matches(x => predicateIsValid, x => x.Write("foo"));

            // Assert
            this.createdConstraint.IsValid("foo").Should().Be(expectedResult);
        }

        [Fact]
        public void Should_create_constraint_that_passes_argument_to_delegate_when_prefixing_with_not()
        {
            // Arrange
            string argumentPassedToDelegate = null;

            // Act
            this.constraintManager.Not.Matches(
                x =>
                {
                    argumentPassedToDelegate = x;
                    return true;
                },
                x => x.Write("foo"));

            // Assert
            this.createdConstraint.IsValid("argument");
            argumentPassedToDelegate.Should().Be("argument");
        }

        [Fact]
        public void Should_create_constraint_that_passes_writer_to_writer_delegate_when_prefixing_with_not()
        {
            // Arrange
            IOutputWriter passedInWriter = null;

            var writerFromOutside = A.Dummy<IOutputWriter>();

            // Act
            this.constraintManager.Not.Matches(x => true, x => passedInWriter = x);

            // Assert
            this.createdConstraint.WriteDescription(writerFromOutside);
            passedInWriter.Should().Be(writerFromOutside);
        }

        [Fact]
        public void Should_create_constraint_writes_the_word_not_to_writer_when_prefixing_with_not()
        {
            // Arrange
            var writer = A.Fake<IOutputWriter>();

            // Act
            this.constraintManager.Not.Matches(x => true, x => { });

            // Assert
            this.createdConstraint.WriteDescription(writer);
            A.CallTo(() => writer.Write("not ")).MustHaveHappened();
        }

        [Fact]
        public void Should_create_constraint_that_writes_beginning_and_end_of_argument_constraint()
        {
            // Arrange
            var writer = new StringBuilderOutputWriter();

            // Act
            this.constraintManager.Matches(x => true, x => x.Write("foo"));

            // Assert
            this.createdConstraint.WriteDescription(writer);
            writer.Builder.ToString().Should().Be("<foo>");
        }
    }
}
