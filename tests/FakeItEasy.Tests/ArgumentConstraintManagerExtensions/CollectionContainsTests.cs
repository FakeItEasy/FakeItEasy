namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System.Collections.Generic;
    using Xunit;

    public class CollectionContainsTests
        : ArgumentConstraintTestBase<IEnumerable<object>>
    {
        protected override string ExpectedDescription => "sequence that contains the value 10";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new object[] { },
                new object[] { null },
                new object[] { 1, 2, 3, "foo", "bar" });
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new object[] { 10 },
                new object[] { 10, 11 },
                new object[] { "foo", 10 });
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

        protected override void CreateConstraint(INegatableArgumentConstraintManager<IEnumerable<object>> scope)
        {
            scope.Contains(10);
        }
    }
}
