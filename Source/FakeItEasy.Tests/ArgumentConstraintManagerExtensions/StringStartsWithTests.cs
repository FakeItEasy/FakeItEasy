namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class StringStartsWithTests
        : ArgumentConstraintTestBase<string>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "foo", "bar", "biz", "baz", "lorem ipsum", null }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { "abc", "abcd", "abc abc", "abc lorem ipsum" }; }
        }

        protected override string ExpectedDescription
        {
            get { return "string that starts with \"abc\""; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.StartsWith("abc");
        }
    }
}
