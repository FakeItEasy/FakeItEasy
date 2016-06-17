namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class StringEndsWithTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "string that ends with \"table\"";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "rabbit",
                "apple",
                "bear",
                "chicken",
                "lorem ipsum",
                null);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "comfortable",
                "portable",
                "immutable",
                "lorem ipsum table");
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.EndsWith("table");
        }
    }
}
