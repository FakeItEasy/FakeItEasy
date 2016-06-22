namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class IsNullTests
        : ArgumentConstraintTestBase<object>
    {
        public override string ExpectedDescription => "NULL";

        public override IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                (object)null);
        }

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                string.Empty,
                "foo",
                "bar");
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.IsNull();
        }
    }
}
