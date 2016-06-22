namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class EqualToConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        public override string ExpectedDescription => "equal to 10";

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                9,
                11,
                null,
                "foo");
        }

        public override IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                10);
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.IsEqualTo(10);
        }
    }
}
