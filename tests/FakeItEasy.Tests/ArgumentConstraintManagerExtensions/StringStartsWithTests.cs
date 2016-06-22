namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class StringStartsWithTests
        : ArgumentConstraintTestBase<string>
    {
        public override string ExpectedDescription => "string that starts with \"abc\"";

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "bar",
                "biz",
                "baz",
                "lorem ipsum",
                null);
        }

        public override IEnumerable<object[]> ValidValues()
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
