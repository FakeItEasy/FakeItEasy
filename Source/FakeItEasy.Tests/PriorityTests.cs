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
            new[] { Priority.Default, Priority.Default },
            new[] { Priority.Default, new Priority(0) },
            new[] { new Priority(0), new Priority(0) },
            new[] { new Priority(1), new Priority(1) },
            new[] { new Priority(byte.MaxValue), new Priority(byte.MaxValue) },
            new[] { Priority.Internal, Priority.Internal },
        };

        private static readonly Priority[][] LessThanCases =
        {
            new[] { Priority.Internal, Priority.Default },
            new[] { Priority.Internal, new Priority(0) },
            new[] { Priority.Internal, new Priority(1) },
            new[] { Priority.Default, new Priority(1) },
            new[] { new Priority(1), new Priority(2) },
            new[] { new Priority(3), new Priority(byte.MaxValue) }
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
        public void
            Greater_than_or_equal_to_operator_should_return_true_when_left_value_is_greater_than_or_equal_to_right(Priority left, Priority right)
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
            Priority.Default.Equals(other).Should().BeFalse();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(47)]
        [TestCase(byte.MaxValue)]
        public void GetHashCode_should_return_same_hash_code_as_wrapped_value(byte value)
        {
            new Priority(value).GetHashCode().Should().Be(value.GetHashCode());
        }

        [Test]
        public void GetHashCode_for_internal_priority_should_return_same_hash_code_as_negative_one()
        {
            Priority.Internal.GetHashCode().Should().Be(-1.GetHashCode());
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

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> UnequalCases()
        {
            return LessThanCases.Concat(GreaterThanCases());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> GreaterThanCases()
        {
            return LessThanCases.Select(pair => new[] { pair[1], pair[0] });
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> LessThanOrEqualToCases()
        {
            return LessThanCases.Concat(EqualCases);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Required for testing.")]
        private static IEnumerable<Priority[]> GreaterThanOrEqualToCases()
        {
            return GreaterThanCases().Concat(EqualCases);
        }
    }
}
