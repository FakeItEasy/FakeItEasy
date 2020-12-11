namespace FakeItEasy.Tests.ArgumentConstraintManagerExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using Xunit;

    public class EqualToWithComparerConstraintTests
        : ArgumentConstraintTestBase<string>
    {
        protected override string ExpectedDescription => @"equal to ""IgnoreCase""";

        public static IEnumerable<object?[]> InvalidValues()
        {
            return TestCases.FromObject(
                (object?)null,
                string.Empty,
                " ",
                "differentValue");
        }

        public static IEnumerable<object?[]> ValidValues()
        {
            return TestCases.FromObject("IgnoreCase", "ignoreCase");
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

        [Fact]
        public void IsEqualTo_should_be_null_guarded_when_comparer_is_null()
        {
            // Arrange
            var constraintManager = new DefaultArgumentConstraintManager<string>(x => { });
            Expression<Action> call = () => constraintManager.IsEqualTo(null!, StringComparer.OrdinalIgnoreCase);

            // Act
            // Assert
            call.Should().BeNullGuarded();
        }

        protected override void CreateConstraint(INegatableArgumentConstraintManager<string> scope)
        {
            scope.IsEqualTo("IgnoreCase", StringComparer.OrdinalIgnoreCase);
        }
    }
}
