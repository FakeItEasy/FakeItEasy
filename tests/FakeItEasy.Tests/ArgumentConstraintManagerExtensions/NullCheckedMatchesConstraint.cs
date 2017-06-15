namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class NullCheckedMatchesConstraint
        : ArgumentConstraintTestBase<object>
    {
        protected override string ExpectedDescription => "is of type string";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new object());
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "Foo",
                string.Empty,
                "Bar");
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
            scope.NullCheckedMatches(x => x is string, x => x.Write("is of type string"));
        }
    }
}
