namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class NullCheckedMatchesConstraint
        : ArgumentConstraintTestBase<object>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get
            {
                yield return null;
                yield return new object();
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get 
            {
                yield return "Foo";
                yield return string.Empty;
                yield return "Bar";
            }
        }

        protected override string ExpectedDescription
        {
            get { return "is of type string"; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.NullCheckedMatches(x => x is string, x => x.Write("is of type string"));
        }
    }
}