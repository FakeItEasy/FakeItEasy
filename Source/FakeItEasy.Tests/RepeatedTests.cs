namespace FakeItEasy.Tests
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class RepeatedTests
    {
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

        [TestCase(1, Result = true)]
        [TestCase(2, Result = false)]
        public bool Exactly_once_should_only_match_one(int actualRepeat)
        {
            // Arrange

            // Act
            var repeated = Repeated.Exactly.Once;

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(2, Result = true)]
        [TestCase(0, Result = false)]
        public bool Exactly_twice_should_only_match_two(int actualRepeat)
        {
            // Arrange

            // Act
            var repeated = Repeated.Exactly.Twice;

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(0, 0, Result = true)]
        [TestCase(0, 1, Result = false)]
        public bool Exactly_number_of_times_should_match_as_expected(int actualRepeat, int expectedNumberOfTimes)
        {
            // Arrange

            // Act
            var repeated = Repeated.Exactly.Times(expectedNumberOfTimes);

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(1, Result = true)]
        [TestCase(0, Result = false)]
        [TestCase(2, Result = true)]
        public bool At_least_once_should_match_one_or_higher(int actualRepeat)
        {
            // Arrange

            // Act
            var repeated = Repeated.AtLeast.Once;

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(1, Result = false)]
        [TestCase(0, Result = false)]
        [TestCase(2, Result = true)]
        [TestCase(3, Result = true)]
        public bool At_least_twice_should_only_match_two_or_higher(int actualRepeat)
        {
            // Arrange

            // Act
            var repeated = Repeated.AtLeast.Twice;

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(0, 0, Result = true)]
        [TestCase(1, 0, Result = true)]
        [TestCase(1, 1, Result = true)]
        [TestCase(0, 1, Result = false)]
        [TestCase(2, 1, Result = true)]
        public bool At_least_number_of_times_should_match_as_expected(int actualRepeat, int expectedNumberOfTimes)
        {
            // Arrange

            // Act
            var repeated = Repeated.AtLeast.Times(expectedNumberOfTimes);

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(0, Result = true)]
        [TestCase(1, Result = true)]
        [TestCase(2, Result = false)]
        public bool No_more_than_once_should_match_zero_and_one_only(int actualRepeat)
        {
            // Arrange

            // Act
            var repeated = Repeated.NoMoreThan.Once;

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(0, Result = true)]
        [TestCase(1, Result = true)]
        [TestCase(2, Result = true)]
        [TestCase(3, Result = false)]
        public bool No_more_than_twice_should_match_zero_one_and_two_only(int actualRepeat)
        {
            // Arrange

            // Act
            var repeated = Repeated.NoMoreThan.Twice;

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(0, 0, Result = true)]
        [TestCase(1, 0, Result = false)]
        [TestCase(1, 1, Result = true)]
        [TestCase(0, 1, Result = true)]
        [TestCase(2, 1, Result = false)]
        public bool No_more_than_times_should_match_as_expected(int actualRepeat, int expectedNumberOfTimes)
        {
            // Arrange

            // Act
            var repeated = Repeated.NoMoreThan.Times(expectedNumberOfTimes);

            // Assert
            return repeated.Matches(actualRepeat);
        }

        [TestCase(0, Result = true)]
        public bool Never_should_match_zero_only(int actualRepeat)
        {
            // Arrange

            // Act

            // Assert
            return Repeated.Never.Matches(actualRepeat);
        }
    }
}
