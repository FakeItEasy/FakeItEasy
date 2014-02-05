namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    internal class SameAsConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        private static readonly SomeRefType TheRealThing = new SomeRefType { Value = "Foo" };

        protected override IEnumerable<object> InvalidValues
        {
            get
            {
                yield return new SomeRefType { Value = "Foo" };
                yield return new SomeRefType { Value = "Bar" };
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { yield return TheRealThing; }
        }

        protected override string ExpectedDescription
        {
            get { return "same as Foo"; }
        }

        protected override void CreateConstraint(IArgumentConstraintManager<object> scope)
        {
            scope.IsSameAs(TheRealThing);
        }

        private class SomeRefType
        {
            public string Value { get; set; }

            public override string ToString()
            {
                return this.Value;
            }
        }
    }
}