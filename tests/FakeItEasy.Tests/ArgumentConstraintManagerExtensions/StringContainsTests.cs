namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class StringContainsTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "string that contains \"bar\"";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "baz",
                "biz",
                string.Empty,
                null,
                "lorem ipsum");
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "bar",
                "barcode",
                "foo bar",
                "unbareable ;-)");
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.Contains("bar");
        }
    }
}
