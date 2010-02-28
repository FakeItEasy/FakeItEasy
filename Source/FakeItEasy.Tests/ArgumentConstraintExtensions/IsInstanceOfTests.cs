using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Tests.Expressions.ArgumentConstraints;
using NUnit.Framework;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class IsInstanceOfTests
        : ArgumentValidatorTestBase<object>
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = A<object>.That.IsInstanceOf<DateTime>();
        }

        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { new object(), 1, "foo" }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { new DateTime(2000, 1, 1) }; }
        }

        protected override string ExpectedDescription
        {
            get { return "Instance of System.DateTime"; }
        }
    }
}
