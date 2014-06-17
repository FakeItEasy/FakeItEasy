namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class DerivedTypeArgumentTests
        : ArgumentConstraintTestBase<string>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "bar", 123, 12.3f }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { "foo", null }; }
        }

        protected override string ExpectedDescription
        {
            get { return "string that is \"foo\" or is empty"; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            FakeItEasy.Guard.AgainstNull(scope, "scope");

            scope.Matches(x => x == null || x == "foo", x => x.Write("string that is \"foo\" or is empty"));
        }
    }
}
