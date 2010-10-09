namespace FakeItEasy.Tests.ArgumentValidationExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class CollectionContainsTests
        : ArgumentConstraintTestBase<IEnumerable<object>>
    {

        protected override FakeItEasy.Expressions.ArgumentConstraint<IEnumerable<object>> CreateConstraint(FakeItEasy.Expressions.ArgumentConstraintScope<IEnumerable<object>> scope)
        {
            return scope.Contains(10);
        }

        protected override IEnumerable<object> InvalidValues
        {
            get
            {
                yield return new object[] { };
                yield return new object[] { null };
                yield return new object[] { 1, 2, 3, "foo", "bar" };
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get 
            {
                yield return new object[] { 10 };
                yield return new object[] { 10, 11 };
                yield return new object[] { "foo", 10 };
            }
        }

        protected override string ExpectedDescription
        {
            get 
            {
                return "sequence that contains the value 10";
            }
        }
    }
}
