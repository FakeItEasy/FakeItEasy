namespace FakeItEasy.Tests.Core
{
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultArgumentConstraintManagerTests
    {
        private DefaultArgumentConstraintManager<string> constraintManager;
        private IArgumentConstraint createdConstraint;

        [SetUp]
        public void Setup()
        {
            this.createdConstraint = null;
            this.constraintManager = new DefaultArgumentConstraintManager<string>(x => this.createdConstraint = x);
        }

        [TestCase(true, Result = true)]
        [TestCase(false, Result = false)]
        public bool Should_add_constraint_that_is_validated_by_the_specified_predicate(bool predicateIsValid)
        {
            // Arrange

            // Act
            this.constraintManager.Matches(x => predicateIsValid, x => x.Write("foo"));

            // Assert
            return this.createdConstraint.IsValid("foo");
        }

        [Test]
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
            Assert.That(argumentPassedToDelegate, Is.EqualTo("argument"));
        }

        [Test]
        public void Should_create_constraint_that_passes_writer_to_writer_delegate()
        {
            // Arrange
            IOutputWriter passedInWriter = null;

            var writerFromOutside = A.Dummy<IOutputWriter>();

            // Act
            this.constraintManager.Matches(x => true, x => passedInWriter = x);

            // Assert
            this.createdConstraint.WriteDescription(writerFromOutside);
            Assert.That(passedInWriter, Is.EqualTo(writerFromOutside));
        }

        [TestCase(true, Result = false)]
        [TestCase(false, Result = true)]
        public bool Should_add_constraint_that_is_validated_inversed_by_the_specified_predicate_when_prefixing_with_not(bool predicateIsValid)
        {
            // Arrange

            // Act
            this.constraintManager.Not.Matches(x => predicateIsValid, x => x.Write("foo"));

            // Assert
            return this.createdConstraint.IsValid("foo");
        }

        [Test]
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
            Assert.That(argumentPassedToDelegate, Is.EqualTo("argument"));
        }

        [Test]
        public void Should_create_constraint_that_passes_writer_to_writer_delegate_when_prefixing_with_not()
        {
            // Arrange
            IOutputWriter passedInWriter = null;

            var writerFromOutside = A.Dummy<IOutputWriter>();

            // Act
            this.constraintManager.Not.Matches(x => true, x => passedInWriter = x);

            // Assert
            this.createdConstraint.WriteDescription(writerFromOutside);
            Assert.That(passedInWriter, Is.EqualTo(writerFromOutside));
        }

        [Test]
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

        [Test]
        public void Should_create_constraint_that_writes_beginning_and_end_of_argument_constraint()
        {
            // Arrange
            var writer = new StringBuilderOutputWriter();

            // Act
            this.constraintManager.Matches(x => true, x => x.Write("foo"));

            // Assert
            this.createdConstraint.WriteDescription(writer);
            Assert.That(writer.Builder.ToString(), Is.EqualTo("<foo>"));
        }
    }
}