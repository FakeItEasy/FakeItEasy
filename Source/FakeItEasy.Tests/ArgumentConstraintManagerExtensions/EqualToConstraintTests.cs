namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class EqualToConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get 
            {
                yield return 9;
                yield return 11;
                yield return null;
                yield return "foo";
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { yield return 10; }
        }

        protected override string ExpectedDescription
        {
            get { return "equal to 10"; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.IsEqualTo(10);
        }
    }
}