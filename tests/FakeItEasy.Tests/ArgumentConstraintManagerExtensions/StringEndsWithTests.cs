namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class StringEndsWithTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => "string that ends with \"table\"";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                "rabbit",
                "apple",
                "bear",
                "chicken",
                "lorem ipsum",
                null);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                "comfortable",
                "portable",
                "immutable",
                "lorem ipsum table");
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
            scope.EndsWith("table");
        }
    }
}
