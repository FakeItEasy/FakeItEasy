namespace FakeItEasy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FakeItEasy.Tests.ArgumentConstraintManagerExtensions;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ArgumentConstraintTestBase
    {
        public static object[][] TestContextTypes =
        {
            new object[] { new CollectionContainsTests() },
            new object[] { new CollectionIsEmptyTests() },
        };

        [Theory]
        [TypedContextData(nameof(TestContextTypes),
            nameof(ArgumentConstraintTestBase<IEnumerable<object>>.InvalidValues))]
        public void IsValid_should_return_false_for_invalid_values(ArgumentConstraintTestBase<IEnumerable<object>> testContext, object invalidValue)
        {
            testContext.ConstraintField.IsValid(invalidValue).Should().BeFalse();
        }

        [Theory]
        [TypedContextData(nameof(TestContextTypes),
            nameof(ArgumentConstraintTestBase<IEnumerable<object>>.ValidValues))]
        public void IsValid_should_return_true_for_valid_values(ArgumentConstraintTestBase<IEnumerable<object>> testContext, object validValue)
        {
            var result = testContext.ConstraintField.IsValid(validValue);

            result.Should().BeTrue();
        }

        [Theory]
        [TypedContextData(nameof(TestContextTypes))]
        public virtual void Constraint_should_provide_correct_description(ArgumentConstraintTestBase<IEnumerable<object>> testContext)
        {
            var output = new StringBuilder();

            testContext.ConstraintField.WriteDescription(new StringBuilderOutputWriter(output));

            output.ToString().Should().Be("<" + testContext.ExpectedDescription + ">");
        }
    }
}
