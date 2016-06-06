namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class SameAsConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        private static readonly SomeRefType TheRealThing = new SomeRefType { Value = "Foo" };

        protected override string ExpectedDescription
        {
            get { return "same as Foo"; }
        }

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new SomeRefType { Value = "Foo" },
                new SomeRefType { Value = "Bar" });
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                TheRealThing);
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
