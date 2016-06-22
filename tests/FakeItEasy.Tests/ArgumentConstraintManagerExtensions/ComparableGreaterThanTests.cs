namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class ComparableGreaterThanTests
        : ArgumentConstraintTestBase<int>
    {
        public override string ExpectedDescription => "greater than 100";

        public override IEnumerable<object[]> InvalidValues()
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

        public override IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                101,
                1000,
                78990,
                int.MaxValue);
        }

        protected override void CreateConstraint(IArgumentConstraintManager<int> scope)
        {
            scope.IsGreaterThan(100);
        }
    }
}
