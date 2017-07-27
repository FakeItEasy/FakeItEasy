namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class IsInstanceOfTests
        : ArgumentConstraintTestBase<object>
    {
        protected override string ExpectedDescription => "Instance of System.DateTime";

        public static IEnumerable<object[]> InvalidValues()
        {
            return TestCases.FromObject(
                new object(),
                1,
                "foo",
                null);
        }

        public static IEnumerable<object[]> ValidValues()
        {
            return TestCases.FromObject(
                new DateTime(2000, 1, 1));
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
            scope.IsInstanceOf(typeof(DateTime));
        }
    }
}
