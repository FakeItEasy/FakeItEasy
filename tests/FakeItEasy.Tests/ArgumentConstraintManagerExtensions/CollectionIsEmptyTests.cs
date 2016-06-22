namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using System.Linq;

    public class CollectionIsEmptyTests
        : ArgumentConstraintTestBase<IEnumerable<object>>
    {
        public override string ExpectedDescription => "empty collection";

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new List<object> { 1, 2 },
                new object[] { "foo" },
                Enumerable.Range(1, 10).Cast<object>());
        }

        public override IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new List<object>(),
                Enumerable.Empty<object>(),
                new object[0]);
        }

        protected override void CreateConstraint(IArgumentConstraintManager<IEnumerable<object>> scope)
        {
            scope.IsEmpty();
        }
    }
}
