namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class StringStartsWithTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "string that starts with \"abc\"";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "bar",
                "biz",
                "baz",
                "lorem ipsum",
                null);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "abc",
                "abcd",
                "abc abc",
                "abc lorem ipsum");
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.StartsWith("abc");
        }
    }
}
