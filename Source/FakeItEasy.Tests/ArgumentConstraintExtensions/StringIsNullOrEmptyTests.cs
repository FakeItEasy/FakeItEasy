using System.Collections.Generic;
using NUnit.Framework;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class StringIsNullOrEmptyTests
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.constraintField = A<string>.That.IsNullOrEmpty();
        }

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
            get { return "(NULL or string.Empty)"; }
        }
    }
}
