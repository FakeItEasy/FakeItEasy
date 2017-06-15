namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class ComparableGreaterThanTests
        : ArgumentConstraintTestBase<int>
    {
        protected override string ExpectedDescription => "greater than 100";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                int.MinValue,
                -100,
                0,
                1,
                2,
                3,
                5,
                8,
                13,
                21,
                34,
                55,
                89,
                100);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                101,
                1000,
                78990,
                int.MaxValue);
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

        protected override void CreateConstraint(INegatableArgumentConstraintManager<int> scope)
        {
            scope.IsGreaterThan(100);
        }
    }
}
