using System.Collections.Generic;
using FakeItEasy.Core;
using FakeItEasy.Tests;
using NUnit.Framework;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    internal class StringContainsTests
        : ArgumentConstraintTestBase<string>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "foo", "baz", "biz", "", null, "lorem ipsum" }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { "bar", "barcode", "foo bar", "unbareable ;-)" }; }
        }

        protected override string ExpectedDescription
        {
            get { return "string that contains \"bar\""; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<string> scope)
        {
            scope.Contains("bar");
        }
    }
}
