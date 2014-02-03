namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class StringIsNullOrEmptyTests
        : ArgumentConstraintTestBase<string>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "foo", "bar", "a", "b" }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { string.Empty, null }; }
        }

        protected override string ExpectedDescription
        {
            get { return "NULL or string.Empty"; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.IsNullOrEmpty();
        }
    }
}
