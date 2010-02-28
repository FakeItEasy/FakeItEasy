using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Tests.Expressions.ArgumentConstraints;
using NUnit.Framework;

namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    [TestFixture]
    public class StringContainsTests
        : ArgumentValidatorTestBase<string>
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = A<string>.That.Contains("bar");
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

    [TestFixture]
    public class StringStartsWithTests
        : ArgumentValidatorTestBase<string>
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = A<string>.That.StartsWith("abc");
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

    [TestFixture]
    public class StringIsNullOrEmptyTests
        : ArgumentValidatorTestBase<string>
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = A<string>.That.IsNullOrEmpty();
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

    [TestFixture]
    public class ComparableGreaterThanTests
        : ArgumentValidatorTestBase<int>
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = A<int>.That.IsGreaterThan(100);
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
