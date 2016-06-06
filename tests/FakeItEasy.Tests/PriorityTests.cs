namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FluentAssertions;
    using Xunit;

    public class PriorityTests
    {
        public static IEnumerable<object[]> EqualCases()
        {
            return new[]
            {
                new object[] { Priority.Default, Priority.Default },
                new object[] { Priority.Default, new Priority(0) },
                new object[] { new Priority(0), new Priority(0) },
                new object[] { new Priority(1), new Priority(1) },
                new object[] { new Priority(byte.MaxValue), new Priority(byte.MaxValue) },
                new object[] { Priority.Internal, Priority.Internal },
            };
        }

        public static IEnumerable<object[]> LessThanCases()
        {
            return new[]
            {
                new object[] { Priority.Internal, Priority.Default },
                new object[] { Priority.Internal, new Priority(0) },
                new object[] { Priority.Internal, new Priority(1) },
                new object[] { Priority.Default, new Priority(1) },
                new object[] { new Priority(1), new Priority(2) },
                new object[] { new Priority(3), new Priority(byte.MaxValue) }
            };
        }

        [Theory]
        [MemberData(nameof(EqualCases))]
        public void CompareTo_should_return_zero_when_left_value_is_equal_to_right(Priority left, Priority right)
        {
            left.CompareTo(right).Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(LessThanCases))]
        public void CompareTo_should_return_negative_when_left_value_is_less_than_right(Priority left, Priority right)
        {
            left.CompareTo(right).Should().BeNegative();
        }

        [Theory]
        [MemberData(nameof(GreaterThanCases))]
        public void CompareTo_should_return_positive_when_left_value_is_greater_than_right(Priority left, Priority right)
        {
            left.CompareTo(right).Should().BePositive();
        }

        [Theory]
        [MemberData(nameof(LessThanCases))]
        public void Less_than_operator_should_return_true_when_left_value_is_less_than_right(Priority left, Priority right)
        {
            (left < right).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GreaterThanOrEqualToCases))]
        public void Less_than_operator_should_return_false_when_left_value_is_not_less_than_right(Priority left, Priority right)
        {
            (left < right).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GreaterThanCases))]
        public void Greater_than_operator_should_return_true_when_left_value_is_greater_than_right(Priority left, Priority right)
        {
            (left > right).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(LessThanOrEqualToCases))]
        public void Greater_than_operator_should_return_false_when_left_value_is_not_greater_than_right(Priority left, Priority right)
        {
            (left > right).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(LessThanOrEqualToCases))]
        public void Less_than_or_equal_to_operator_should_return_true_when_left_value_is_less_than_or_equal_to_right(Priority left, Priority right)
        {
            (left <= right).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GreaterThanCases))]
        public void Less_than_or_equal_to_operator_should_return_false_when_left_value_is_greater_than_right(Priority left, Priority right)
        {
            (left <= right).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GreaterThanOrEqualToCases))]
        public void
            Greater_than_or_equal_to_operator_should_return_true_when_left_value_is_greater_than_or_equal_to_right(Priority left, Priority right)
        {
            (left >= right).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(LessThanCases))]
        public void Greater_than_or_equal_to_operator_should_return_false_when_left_value_is_less_than_right(Priority left, Priority right)
        {
            (left >= right).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(EqualCases))]
        public void Equals_priority_should_return_true_when_this_value_is_equal_to_other(Priority left, Priority right)
        {
            left.Equals(right).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(UnequalCases))]
        public void Equals_priority_should_return_false_when_this_value_is_not_equal_to_other(Priority left, Priority right)
        {
            left.Equals(right).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(EqualCases))]
        public void Equals_object_should_return_true_when_this_value_is_equal_to_other(Priority left, object right)
        {
            left.Equals(right).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(UnequalCases))]
        public void Equals_object_should_return_false_when_this_value_is_not_equal_to_other(Priority left, object right)
        {
            left.Equals(right).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData((byte)0)]
        [InlineData("zero")]
        public void Equals_object_should_return_false_when_other_value_is_another_type(object other)
        {
            Priority.Default.Equals(other).Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(47)]
        [InlineData(byte.MaxValue)]
        public void GetHashCode_should_return_same_hash_code_as_wrapped_value(byte value)
        {
            new Priority(value).GetHashCode().Should().Be(value.GetHashCode());
        }

        [Fact]
        public void GetHashCode_for_internal_priority_should_return_same_hash_code_as_negative_one()
        {
            Priority.Internal.GetHashCode().Should().Be(-1.GetHashCode());
        }

        [Theory]
        [MemberData(nameof(EqualCases))]
        public void Equality_operator_should_return_true_when_left_value_is_equal_to_right(Priority left, Priority right)
        {
            (left == right).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(UnequalCases))]
        public void Equality_operator_should_return_false_when_left_value_is_not_equal_to_right(Priority left, Priority right)
        {
            (left == right).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(EqualCases))]
        public void Inequality_operator_should_return_false_when_left_value_is_equal_to_right(Priority left, Priority right)
        {
            (left != right).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(UnequalCases))]
        public void Inequality_operator_should_return_true_when_left_value_is_not_equal_to_right(Priority left, Priority right)
        {
            (left != right).Should().BeTrue();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
        private static IEnumerable<object[]> UnequalCases()
        {
            return LessThanCases().Concat(GreaterThanCases());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
        private static IEnumerable<object[]> GreaterThanCases()
        {
            return LessThanCases().Select(pair => new[] { pair[1], pair[0] });
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
        private static IEnumerable<object[]> LessThanOrEqualToCases()
        {
            return LessThanCases().Concat(EqualCases());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used reflectively.")]
        private static IEnumerable<object[]> GreaterThanOrEqualToCases()
        {
            return GreaterThanCases().Concat(EqualCases());
        }
    }
}
