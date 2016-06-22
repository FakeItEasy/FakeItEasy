namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class StringContainsTests
        : ArgumentConstraintTestBase<string>
    {
        public override string ExpectedDescription => "string that contains \"bar\"";

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "baz",
                "biz",
                string.Empty,
                null,
                "lorem ipsum");
        }

        public override IEnumerable<object[]> ValidValues()
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
