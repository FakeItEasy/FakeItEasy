namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class CollectionIsEmptyTests
        : ArgumentConstraintTestBase<IEnumerable<object>>
    {
        protected override string ExpectedDescription => "empty collection";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                null,
                new List<object> { 1, 2 },
                new object[] { "foo" },
                Enumerable.Range(1, 10).Cast<object>());
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new List<object>(),
                Enumerable.Empty<object>(),
                Array.Empty<object>());
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
            scope.IsEmpty();
        }
    }
}
