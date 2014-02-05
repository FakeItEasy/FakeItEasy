namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class StringEndsWithTests
        : ArgumentConstraintTestBase<string>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "rabbit", "apple", "bear", "chicken", "lorem ipsum", null }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { "comfortable", "portable", "immutable", "lorem ipsum table" }; }
        }

        protected override string ExpectedDescription
        {
            get { return "string that ends with \"table\""; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.EndsWith("table");
        }
    }
}