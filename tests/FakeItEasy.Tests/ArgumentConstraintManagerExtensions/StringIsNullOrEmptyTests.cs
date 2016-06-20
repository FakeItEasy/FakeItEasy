namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class StringIsNullOrEmptyTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "NULL or string.Empty";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "bar",
                "a",
                "b");
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                string.Empty,
                null);
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.IsNullOrEmpty();
        }
    }
}
