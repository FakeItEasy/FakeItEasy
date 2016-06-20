namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class IsNullTests
        : ArgumentConstraintTestBase<object>
    {
        protected override string ExpectedDescription => "NULL";

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                (object)null);
        }

        public static IEnumerable<object[]> InvalidValues()
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
