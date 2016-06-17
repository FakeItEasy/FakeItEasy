namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class CollectionContainsTests
        : ArgumentConstraintTestBase<IEnumerable<object>>
    {
        protected override string ExpectedDescription => "sequence that contains the value 10";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new object[] { },
                new object[] { null },
                new object[] { 1, 2, 3, "foo", "bar" });
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new object[] { 10 },
                new object[] { 10, 11 },
                new object[] { "foo", 10 });
        }

        protected override void CreateConstraint(IArgumentConstraintManager<IEnumerable<object>> scope)
        {
            scope.Contains(10);
        }
    }
}
