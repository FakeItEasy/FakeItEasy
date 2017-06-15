namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class DerivedTypeArgumentTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "string that is \"foo\" or is empty";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "bar",
                123,
                12.3f);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "foo",
                null);
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

        protected override void CreateConstraint(INegatableArgumentConstraintManager<string> scope)
        {
            Guard.AgainstNull(scope, nameof(scope));

            scope.Matches(x => x == null || x == "foo", x => x.Write("string that is \"foo\" or is empty"));
        }
    }
}
