namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class StringIsNullOrEmptyTests
        : ArgumentConstraintTestBase<string>
    {
        public override string ExpectedDescription => "NULL or string.Empty";

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "bar",
                "a",
                "b");
        }

        public override IEnumerable<object[]> ValidValues()
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
