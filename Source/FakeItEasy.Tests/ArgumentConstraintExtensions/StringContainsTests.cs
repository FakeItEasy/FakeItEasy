using System.Collections.Generic;
using NUnit.Framework;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class StringContainsTests
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.constraintField = A<string>.That.Contains("bar");
        }

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
            get { return "String that contains \"bar\""; }
        }
    }
}
