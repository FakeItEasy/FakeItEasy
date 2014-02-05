namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class IsNullTests
        : ArgumentConstraintTestBase<object>
    {
        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { null }; }
        }

        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { string.Empty, "foo", "bar" }; }
        }

        protected override string ExpectedDescription
        {
            get { return "NULL"; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.IsNull();
        }
    }
}