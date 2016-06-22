namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class NullCheckedMatchesConstraint
        : ArgumentConstraintTestBase<object>
    {
        public override string ExpectedDescription => "is of type string";

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new object());
        }

        public override IEnumerable<object[]> ValidValues()
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
