namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class IsSameSequenceAsTests
        : ArgumentConstraintTestBase<IEnumerable<int>>
    {
        protected override string ExpectedDescription => "specified sequence";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new[] { 1, 2 },
                new int[] { },
                null,
                new[] { 1, 2, 3, 4 },
                new[] { 9, 8 });
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new[] { 1, 2, 3 },
                new List<int> { 1, 2, 3 });
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public override void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            base.IsValid_should_return_false_for_invalid_values(invalidValue);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public override void IsValid_should_return_true_for_valid_values(object validValue)
        {
            base.IsValid_should_return_true_for_valid_values(validValue);
        }

        protected override void CreateConstraint(INegatableArgumentConstraintManager<IEnumerable<int>> scope)
        {
            scope.IsSameSequenceAs(new[] { 1, 2, 3 });
        }
    }
}
