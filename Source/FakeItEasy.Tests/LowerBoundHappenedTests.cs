namespace FakeItEasy.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class LowerBoundHappenedTests
    {
        [TestCase(1, 1, Result = true)]
        [TestCase(1, 0, Result = false)]
        [TestCase(2, 3, Result = true)]
        public bool Matches_should_return_correct_value(int specifiedRepeat, int numberOfCallsActuallyMade)
        {
            // Arrange
            var happened = new LowerBoundRepeated(specifiedRepeat);

            // Act

            // Assert
            return happened.Matches(numberOfCallsActuallyMade);
        }

        [TestCase(0, Result = "never")]
        [TestCase(1, Result = "once")]
        [TestCase(2, Result = "twice")]
        [TestCase(3, Result = "#3 times")]
        [TestCase(4, Result = "#4 times")]
        public string ToString_should_provide_correct_description(int repeat)
        {
            // Arrange
            var happened = new LowerBoundRepeated(repeat);

            // Act

            // Assert
            return happened.ToString();
        }

        [TestCase(1, Result = "once or less")]
        [TestCase(2, Result = "twice or less")]
        [TestCase(3, Result = "#3 times or less")]
        [TestCase(4, Result = "#4 times or less")]
        public string ToString_should_provide_correct_description_when_or_less_is_specified(int repeat)
        {
            // Arrange
            var happened = new LowerBoundRepeated(repeat);

            // Act

            // Assert
            return happened.OrLess.ToString();
        }

        [TestCase(1, Result = "exactly once")]
        [TestCase(2, Result = "exactly twice")]
        [TestCase(3, Result = "exactly #3 times")]
        [TestCase(4, Result = "exactly #4 times")]
        public string ToString_should_provide_correct_description_when_exactly_is_specified(int repeat)
        {
            // Arrange
            var happened = new LowerBoundRepeated(repeat);

            // Act

            // Assert
            return happened.Exactly.ToString();
        }

        [TestCase(1, 1, Result = true)]
        [TestCase(1, 0, Result = false)]
        [TestCase(2, 3, Result = false)]
        public bool Exactly_should_return_Happened_that_matches_on_exact_values(int specifiedRepeat, int numberOfCallsActuallyMade)
        {
            // Arrange
            var happened = new LowerBoundRepeated(specifiedRepeat).Exactly;

            // Act

            // Assert
            return happened.Matches(numberOfCallsActuallyMade);
        }

        [TestCase(1, 1, Result = true)]
        [TestCase(1, 0, Result = true)]
        [TestCase(2, 3, Result = false)]
        public bool OrLess_should_return_Happened_that_matches_on_same_value_or_less(int specifiedRepeat, int numberOfCallsActuallyMade)
        {
            // Arrange
            var happened = new LowerBoundRepeated(specifiedRepeat).OrLess;

            // Act

            // Assert
            return happened.Matches(numberOfCallsActuallyMade);
        }
    }
}
