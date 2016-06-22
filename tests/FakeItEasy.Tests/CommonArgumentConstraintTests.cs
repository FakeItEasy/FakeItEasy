namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FakeItEasy.Tests.ArgumentConstraintManagerExtensions;
    using FakeItEasy.Tests.Expressions.ArgumentConstraints;
    using FakeItEasy.Tests.ExpressionsConstraints;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class CommonArgumentConstraintTests
    {
        public static object[][] TestContextTypes =
        {
            new object[] { new AggregateArgumentConstraintTests() },
            new object[] { new CollectionContainsTests() },
            new object[] { new CollectionIsEmptyTests() },
            new object[] { new ComparableGreaterThanTests() },
            new object[] { new DerivedTypeArgumentTests() },
            new object[] { new EqualityArgumentConstraintTests() },
            new object[] { new EqualToConstraintTests() },
            new object[] { new IsInstanceOfTests() },
            new object[] { new IsNullTests() },
            new object[] { new IsSameSequenceAsTests() },
            new object[] { new NullCheckedMatchesConstraint() },
            new object[] { new SameAsConstraintTests() },
            new object[] { new StringContainsTests() },
            new object[] { new StringEndsWithTests() },
            new object[] { new StringIsNullOrEmptyTests() },
            new object[] { new StringStartsWithTests() },
        };

        [Theory]
        [TypedContextData(nameof(TestContextTypes),
            nameof(ArgumentConstraintTestBase<IEnumerable<object>>.InvalidValues))]
        public void IsValid_should_return_false_for_invalid_values(ArgumentConstraintTestBase testContext, object invalidValue)
        {
            testContext.ConstraintField.IsValid(invalidValue).Should().BeFalse();
        }

        [Theory]
        [TypedContextData(nameof(TestContextTypes),
            nameof(ArgumentConstraintTestBase<IEnumerable<object>>.ValidValues))]
        public void IsValid_should_return_true_for_valid_values(ArgumentConstraintTestBase testContext, object validValue)
        {
            var result = testContext.ConstraintField.IsValid(validValue);

            result.Should().BeTrue();
        }

        [Theory]
        [TypedContextData(nameof(TestContextTypes))]
        public virtual void Constraint_should_provide_correct_description(ArgumentConstraintTestBase testContext)
        {
            var output = new StringBuilder();

            testContext.ConstraintField.WriteDescription(new StringBuilderOutputWriter(output));

            output.ToString().Should().Be("<" + testContext.ExpectedDescription + ">");
        }
    }
}
