namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;

    public class SameAsConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        private static readonly SomeRefType TheRealThing = new SomeRefType("Foo");

        public override string ExpectedDescription => "same as Foo";

        public override IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new SomeRefType("Foo"),
                new SomeRefType("Bar"));
        }

        public override IEnumerable<object[]> ValidValues()
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
            private readonly string value;

            public SomeRefType(string value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return this.value;
            }
        }
    }
}
