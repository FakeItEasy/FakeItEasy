namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class NullCheckedMatchesConstraint
        : ArgumentConstraintTestBase<object>
    {
        protected override string ExpectedDescription => "is of type string";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new object());
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "Foo",
                string.Empty,
                "Bar");
            }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.NullCheckedMatches(x => x is string, x => x.Write("is of type string"));
        }
    }
}
