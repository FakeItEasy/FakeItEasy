namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    internal class CollectionIsEmptyTests
        : ArgumentConstraintTestBase<IEnumerable<object>>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get 
            {
                yield return null;
                yield return new List<object>() { 1, 2 };
                yield return new object[] { "foo" };
                yield return Enumerable.Range(1, 10).Cast<object>();
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get 
            {
                yield return new List<object>();
                yield return Enumerable.Empty<object>();
                yield return new object[] { };
            }
        }

        protected override string ExpectedDescription
        {
            get 
            {
                return "empty collection";
            }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<IEnumerable<object>> scope)
        {
            scope.IsEmpty();
        }
    }
}
