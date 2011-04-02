using System.Collections.Generic;
using FakeItEasy.Core;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class StringIsNullOrEmptyTests
        : ArgumentConstraintTestBase<string>
    {
        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "foo", "bar", "a", "b" }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { "", null }; }
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
