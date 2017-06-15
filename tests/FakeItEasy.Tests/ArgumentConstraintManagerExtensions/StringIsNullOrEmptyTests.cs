namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class StringIsNullOrEmptyTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "NULL or string.Empty";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "bar",
                "a",
                "b");
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                string.Empty,
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
            scope.IsNullOrEmpty();
        }
    }
}
