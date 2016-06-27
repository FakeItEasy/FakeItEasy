namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Xunit;

    public class RepeatedTests
    {
        public static IEnumerable<object> DescriptionTestCases()
        {
            return TestCases.FromProperties(
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.AtLeast.Once,
                    ExpectedDescription = "at least once"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.AtLeast.Twice,
                    ExpectedDescription = "at least twice"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.AtLeast.Times(3),
                    ExpectedDescription = "at least 3 times"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.AtLeast.Times(10),
                    ExpectedDescription = "at least 10 times"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.AtLeast.Times(10),
                    ExpectedDescription = "at least 10 times"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.NoMoreThan.Once,
                    ExpectedDescription = "no more than once"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.NoMoreThan.Twice,
                    ExpectedDescription = "no more than twice"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.NoMoreThan.Times(10),
                    ExpectedDescription = "no more than 10 times"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.Exactly.Once,
                    ExpectedDescription = "exactly once"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.Exactly.Twice,
                    ExpectedDescription = "exactly twice"
                },
                new RepeatDescriptionTestCase()
                {
                    Repeat = () => Repeated.Exactly.Times(99),
                    ExpectedDescription = "exactly 99 times"
                });
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(1, 2, false)]
        public void Like_should_return_instance_that_delegates_to_expression(int expected, int actual, bool expectedResult)
        {
            // Arrange
            Expression<Func<int, bool>> repeatPredicate = repeat => repeat == expected;

            // Act
            var happened = Repeated.Like(repeatPredicate);

            // Assert
            happened.Matches(actual).Should().Be(expectedResult);
        }

        [Fact]
        public void Like_should_return_instance_that_has_correct_description()
        {
            // Arrange
            Expression<Func<int, bool>> repeatPredicate = repeat => repeat == 1;

            // Act
            var happened = Repeated.Like(repeatPredicate);

            // Assert
            happened.ToString().Should().Be("the number of times specified by the predicate 'repeat => (repeat == 1)'");
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public void Exactly_once_should_only_match_one(int actualRepeat, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.Exactly.Once;

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(0, false)]
        public void Exactly_twice_should_only_match_two(int actualRepeat, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.Exactly.Twice;

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, false)]
        public void Exactly_number_of_times_should_match_as_expected(int actualRepeat, int expectedNumberOfTimes, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.Exactly.Times(expectedNumberOfTimes);

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(2, true)]
        public void At_least_once_should_match_one_or_higher(int actualRepeat, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.AtLeast.Once;

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(0, false)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public void At_least_twice_should_only_match_two_or_higher(int actualRepeat, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.AtLeast.Twice;

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(1, 0, true)]
        [InlineData(1, 1, true)]
        [InlineData(0, 1, false)]
        [InlineData(2, 1, true)]
        public void At_least_number_of_times_should_match_as_expected(int actualRepeat, int expectedNumberOfTimes, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.AtLeast.Times(expectedNumberOfTimes);

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public void No_more_than_once_should_match_zero_and_one_only(int actualRepeat, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.NoMoreThan.Once;

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        public void No_more_than_twice_should_match_zero_one_and_two_only(int actualRepeat, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.NoMoreThan.Twice;

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(1, 0, false)]
        [InlineData(1, 1, true)]
        [InlineData(0, 1, true)]
        [InlineData(2, 1, false)]
        public void No_more_than_times_should_match_as_expected(int actualRepeat, int expectedNumberOfTimes, bool expectedResult)
        {
            // Arrange

            // Act
            var repeated = Repeated.NoMoreThan.Times(expectedNumberOfTimes);

            // Assert
            repeated.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void Never_should_match_zero_only(int actualRepeat, bool expectedResult)
        {
            // Arrange

            // Act

            // Assert
            Repeated.Never.Matches(actualRepeat).Should().Be(expectedResult);
        }

        [Theory]
        [MemberData(nameof(DescriptionTestCases))]
        public void Should_provide_expected_description(Func<Repeated> repeated, string expectedDescription)
        {
            Guard.AgainstNull(repeated, nameof(repeated));

            // Arrange
            var repeatedInstance = repeated.Invoke();

            // Act
            var description = repeatedInstance.ToString();

            // Assert
            description.Should().Be(expectedDescription);
        }

        private class RepeatDescriptionTestCase
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
            public Func<Repeated> Repeat { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
            public string ExpectedDescription { get; set; }
        }
    }
}
