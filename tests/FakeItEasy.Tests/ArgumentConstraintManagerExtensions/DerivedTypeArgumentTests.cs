namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class DerivedTypeArgumentTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "string that is \"foo\" or is empty";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "bar",
                123,
                12.3f);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "foo",
                null);
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            Guard.AgainstNull(scope, "scope");

            scope.Matches(x => x == null || x == "foo", x => x.Write("string that is \"foo\" or is empty"));
        }
    }
}
