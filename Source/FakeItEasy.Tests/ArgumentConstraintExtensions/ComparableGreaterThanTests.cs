using System.Collections.Generic;
using NUnit.Framework;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class ComparableGreaterThanTests
        : ArgumentConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.constraintField = A<int>.That.IsGreaterThan(100);
        }

        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { int.MinValue, -100, 0, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 100 }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { 101, 1000, 78990, int.MaxValue }; }
        }

        protected override string ExpectedDescription
        {
            get { return "Greater than 100"; }
        }
    }
}
