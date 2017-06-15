namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class StringContainsTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "string that contains \"bar\"";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "foo",
                "baz",
                "biz",
                string.Empty,
                null,
                "lorem ipsum");
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "bar",
                "barcode",
                "foo bar",
                "unbareable ;-)");
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
            scope.Contains("bar");
        }
    }
}
