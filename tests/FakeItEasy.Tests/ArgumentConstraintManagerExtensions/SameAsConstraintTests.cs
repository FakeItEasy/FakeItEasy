namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class SameAsConstraintTests
        : ArgumentConstraintTestBase<object>
    {
        private static readonly SomeRefType TheRealThing = new SomeRefType("Foo");

        protected override string ExpectedDescription => "same as Foo";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new SomeRefType("Foo"),
                new SomeRefType("Bar"));
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                TheRealThing);
        }

        [Theory]
        [MemberData(nameof(InvalidValues))]
        public override void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            base.IsValid_should_return_false_for_invalid_values(invalidValue);
        }

        [Theory]
        [MemberData(nameof(ValidValues))]
        public override void IsValid_should_return_true_for_valid_values(object validValue)
        {
            base.IsValid_should_return_true_for_valid_values(validValue);
        }

        protected override void CreateConstraint(INegatableArgumentConstraintManager<object> scope)
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
