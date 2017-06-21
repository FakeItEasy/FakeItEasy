namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class IsNullTests
        : ArgumentConstraintTestBase<object>
    {
        protected override string ExpectedDescription => "NULL";

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                (object)null);
        }

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                string.Empty,
                "foo",
                "bar");
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
            scope.IsNull();
        }
    }
}
