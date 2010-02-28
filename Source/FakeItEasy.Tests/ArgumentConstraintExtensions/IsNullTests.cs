using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Tests.Expressions.ArgumentConstraints;
using NUnit.Framework;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class IsNullTests
        : ArgumentConstraintTestBase<string>
    {
        [SetUp]
        public void SetUp()
        {
            this.Constraint = A<string>.That.IsNull();
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { null }; }
        }

        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "", "foo", "bar" }; }
        }

        protected override string ExpectedDescription
        {
            get { return "NULL"; }
        }
    }

}