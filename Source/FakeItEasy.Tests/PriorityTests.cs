namespace FakeItEasy.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;

    public class PriorityTests
    {
        private static readonly Priority[][] EqualCases =
        {
            new[] { BytePriority(0), BytePriority(0) },
            new[] { BytePriority(1), BytePriority(1) },
            new[] { BytePriority(byte.MaxValue), BytePriority(byte.MaxValue) },
            new[] { ShortPriority(short.MinValue), ShortPriority(short.MinValue) },
            new[] { ShortPriority(-1), ShortPriority(-1) },
            new[] { ShortPriority(0), ShortPriority(0) },
            new[] { ShortPriority(1), ShortPriority(1) },
            new[] { ShortPriority(short.MaxValue), ShortPriority(short.MaxValue) },
            new[] { BytePriority(0), ShortPriority(0) },
            new[] { ShortPriority(1), BytePriority(1) },
        };

        private static readonly Priority[][] LessThanCases =
        {
            new[] { BytePriority(0), BytePriority(1) },
            new[] { BytePriority(0), BytePriority(2) },
            new[] { BytePriority(0), BytePriority(byte.MaxValue) },
            new[] { ShortPriority(0), BytePriority(1) },
            new[] { BytePriority(0), ShortPriority(2) },
            new[] { BytePriority(byte.MaxValue), ShortPriority(short.MaxValue) },
            new[] { ShortPriority(-1), BytePriority(0) },
            new[] { ShortPriority(short.MinValue), ShortPriority(-1) },
            new[] { ShortPriority(short.MinValue), BytePriority(5) },
        };

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Required for testing.")]
        private static readonly short[] VariousPriorityValues =
        {
            0,
            1,
            -1,
            -19,
            47,
            byte.MaxValue,
            short.MaxValue,
            short.MaxValue,
        };

        [TestCaseSource("EqualCases")]
        public void CompareTo_should_return_zero_when_left_value_is_equal_to_right(Priority left, Priority right)
        {
            left.CompareTo(right).Should().Be(0);
        }

        [TestCaseSource("LessThanCases")]
        public void CompareTo_should_return_negative_when_left_value_is_less_than_right(Priority left, Priority right)
        {
            left.CompareTo(right).Should().BeNegative();
        }

        [TestCaseSource("GreaterThanCases")]
        public void CompareTo_should_return_positive_when_left_value_is_greater_than_right(Priority left, Priority right)
        {
            left.CompareTo(right).Should().BePositive();
        }

        [TestCaseSource("LessThanCases")]
        public void Less_than_operator_should_return_true_when_left_value_is_less_than_right(Priority left, Priority right)
        {
            (left < right).Should().BeTrue();
        }

        [TestCaseSource("GreaterThanOrEqualToCases")]
        public void Less_than_operator_should_return_false_when_left_value_is_not_less_than_right(Priority left, Priority right)
        {
            (left < right).Should().BeFalse();
        }

        [TestCaseSource("GreaterThanCases")]
        public void Greater_than_operator_should_return_true_when_left_value_is_greater_than_right(Priority left, Priority right)
        {
            (left > right).Should().BeTrue();
        }

        [TestCaseSource("LessThanOrEqualToCases")]
        public void Greater_than_operator_should_return_false_when_left_value_is_not_greater_than_right(Priority left, Priority right)
        {
            (left > right).Should().BeFalse();
        }

        [TestCaseSource("LessThanOrEqualToCases")]
        public void Less_than_or_equal_to_operator_should_return_true_when_left_value_is_less_than_or_equal_to_right(Priority left, Priority right)
        {
            (left <= right).Should().BeTrue();
        }

        [TestCaseSource("GreaterThanCases")]
        public void Less_than_or_equal_to_operator_should_return_false_when_left_value_is_greater_than_right(Priority left, Priority right)
        {
            (left <= right).Should().BeFalse();
        }

        [TestCaseSource("GreaterThanOrEqualToCases")]
        public void Greater_than_or_equal_to_operator_should_return_true_when_left_value_is_greater_than_or_equal_to_right(Priority left, Priority right)
        {
            (left >= right).Should().BeTrue();
        }

        [TestCaseSource("LessThanCases")]
        public void Greater_than_or_equal_to_operator_should_return_false_when_left_value_is_less_than_right(Priority left, Priority right)
        {
            (left >= right).Should().BeFalse();
        }

        [TestCaseSource("EqualCases")]
        public void Equals_priority_should_return_true_when_this_value_is_equal_to_other(Priority left, Priority right)
        {
            left.Equals(right).Should().BeTrue();
        }

        [TestCaseSource("UnequalCases")]
        public void Equals_priority_should_return_false_when_this_value_is_not_equal_to_other(Priority left, Priority right)
        {
            left.Equals(right).Should().BeFalse();
        }

        [TestCaseSource("EqualCases")]
        public void Equals_object_should_return_true_when_this_value_is_equal_to_other(Priority left, object right)
        {
            left.Equals(right).Should().BeTrue();
        }

        [TestCaseSource("UnequalCases")]
        public void Equals_object_should_return_false_when_this_value_is_not_equal_to_other(Priority left, object right)
        {
            left.Equals(right).Should().BeFalse();
        }

        [TestCase(null)]
        [TestCase(0)]
        [TestCase((byte)0)]
        [TestCase("zero")]
        public void Equals_object_should_return_false_when_other_value_is_another_type(object other)
        {
            ShortPriority(0).Equals(other).Should().BeFalse();
        }

        [TestCaseSource("VariousPriorityValues")]
        public void GetHashCode_should_return_same_hash_code_as_wrapped_value(short value)
        {
            ShortPriority(value).GetHashCode().Should().Be(value.GetHashCode());
        }

        [TestCaseSource("EqualCases")]
        public void Equality_operator_should_return_true_when_left_value_is_equal_to_right(Priority left, Priority right)
        {
            (left == right).Should().BeTrue();
        }

        [TestCaseSource("UnequalCases")]
        public void Equality_operator_should_return_false_when_left_value_is_not_equal_to_right(Priority left, Priority right)
        {
            (left == right).Should().BeFalse();
        }

        [TestCaseSource("EqualCases")]
        public void Inequality_operator_should_return_false_when_left_value_is_equal_to_right(Priority left, Priority right)
        {
            (left != right).Should().BeFalse();
        }

        [TestCaseSource("UnequalCases")]
        public void Inequality_operator_should_return_true_when_left_value_is_not_equal_to_right(Priority left, Priority right)
        {
            (left != right).Should().BeTrue();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> UnequalCases()
        {
            return LessThanCases.Concat(GreaterThanCases());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> GreaterThanCases()
        {
            return LessThanCases.Select(pair => new[] { pair[1], pair[0] });
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> LessThanOrEqualToCases()
        {
            return LessThanCases.Concat(EqualCases);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> GreaterThanOrEqualToCases()
        {
            return GreaterThanCases().Concat(EqualCases);
        }

        private static Priority BytePriority(byte value)
        {
            return new Priority(value);
        }

        private static Priority ShortPriority(short value)
        {
            return new Priority(value);
        }
    }
}
