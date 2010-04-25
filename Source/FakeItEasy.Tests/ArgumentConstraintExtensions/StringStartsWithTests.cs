using System.Collections.Generic;
using NUnit.Framework;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class StringStartsWithTests
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.constraint = A<string>.That.StartsWith("abc");
        }

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
            get { return "String that starts with \"abc\""; }
        }
    }
}
