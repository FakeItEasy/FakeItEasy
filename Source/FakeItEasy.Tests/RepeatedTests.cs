namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class RepeatedTests
    {
        [TestCase(1, Result = true)]
        [TestCase(2, Result = true)]
        [TestCase(0, Result = false)]
        public bool Once_should_return_instance_that_matches_one_or_above(int repeat)
        {
            // Arrange
            var once = Repeated.Once;

            // Act
            
            // Assert
            return once.Matches(repeat);
        }

        [TestCase(1, Result = false)]
        [TestCase(2, Result = true)]
        [TestCase(0, Result = false)]
        [TestCase(3, Result = true)]
        public bool Twice_should_return_instance_that_matches_one_or_above(int repeat)
        {
            // Arrange
            var twice = Repeated.Twice;

            // Act
            
            // Assert
            return twice.Matches(repeat);
        }

        [TestCase(1, Result = false)]
        [TestCase(2, Result = false)]
        [TestCase(0, Result = true)]
        [TestCase(3, Result = false)]
        public bool Never_should_return_instance_that_matches_one_or_above(int repeat)
        {
            // Arrange
            var never = Repeated.Never;

            // Act

            // Assert
            return never.Matches(repeat);
        }

        [TestCase(1, 2, Result = true)]
        [TestCase(2, 2, Result = true)]
        [TestCase(3, 2, Result = false)]
        [TestCase(1, 0, Result = false)]
        [TestCase(0, 4, Result = true)]
        public bool Never_should_return_instance_that_matches_one_or_above(int expectedRepeat, int repeat)
        {
            // Arrange
            var times = Repeated.Times(expectedRepeat);

            // Act

            // Assert
            return times.Matches(repeat);
        }

        [TestCase(1, 1, Result = true)]
        [TestCase(1, 2, Result = false)]
        public bool ConvertFromExpression_should_return_instance_that_delegates_to_expression(int expected, int actual)
        {
            // Arrange
            Expression<Func<int, bool>> repeatPredicate = repeat => repeat == expected;

            // Act
            var happened = Repeated.Like(repeatPredicate);

            // Assert
            return happened.Matches(actual);
        }

        [Test]
        public void Like_should_return_instance_that_has_correct_description()
        {
            // Arrange
            Expression<Func<int, bool>> repeatPredicate = repeat => repeat == 1;

            // Act
            var happened = Repeated.Like(repeatPredicate);

            // Assert
            Assert.That(happened.ToString(), Is.EqualTo("the number of times specified by the predicate 'repeat => (repeat == 1)'"));
        }
    }
}
