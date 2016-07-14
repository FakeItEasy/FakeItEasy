namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ArgumentValueFormatterTests
    {
        private readonly ArgumentValueFormatter formatter;
        private readonly List<IArgumentValueFormatter> registeredTypeFormatters;

        public ArgumentValueFormatterTests()
        {
            this.registeredTypeFormatters = new List<IArgumentValueFormatter>();

            this.formatter = new ArgumentValueFormatter(this.registeredTypeFormatters);
        }

        public static IEnumerable<object> SpecificCases()
        {
            return TestCases.FromProperties(
                new
                {
                    LessSpecific = typeof(object),
                    MoreSpecific = typeof(DateTime),
                    Value = (object)new DateTime(2000, 1, 1)
                },
                new
                {
                    LessSpecific = typeof(object),
                    MoreSpecific = typeof(Stream),
                    Value = (object)new MemoryStream()
                },
                new
                {
                    LessSpecific = typeof(object),
                    MoreSpecific = typeof(IFoo),
                    Value = (object)A.Fake<IFoo>()
                });
        }

        [Fact]
        public void Should_write_null_values_correct()
        {
            // Arrange

            // Act
            var description = this.formatter.GetArgumentValueAsString(null);

            // Assert
            description.Should().Be("<NULL>");
        }

        [Fact]
        public void Should_use_ToString_when_no_matching_formatter_is_registered()
        {
            // Arrange

            // Act
            var description = this.formatter.GetArgumentValueAsString(1);

            // Assert
            description.Should().Be("1");
        }

        [Fact]
        public void Should_use_injected_formatter_when_available()
        {
            // Arrange
            this.AddTypeFormatter<DateTime>("Y2K");

            // Act
            var result = this.formatter.GetArgumentValueAsString(new DateTime(2000, 1, 1));

            // Assert
            result.Should().Be("Y2K");
        }

        [Fact]
        public void Should_use_injected_formatter_that_is_for_base_type_of_value()
        {
            // Arrange
            this.AddTypeFormatter<Stream>("stream");

            // Act
            var result = this.formatter.GetArgumentValueAsString(new MemoryStream());

            // Assert
            result.Should().Be("stream");
        }

        [Theory]
        [MemberData(nameof(SpecificCases))]
        public void Should_favor_most_specific_formatter_when_more_than_one_is_applicable(Type lessSpecific, Type moreSpecific, object value)
        {
            // Arrange
            this.AddTypeFormatter(lessSpecific, "less specific");
            this.AddTypeFormatter(moreSpecific, "more specific");

            // Act
            var result = this.formatter.GetArgumentValueAsString(value);

            // Assert
            result.Should().Be("more specific");
        }

        [Fact]
        public void Should_use_formatter_with_highest_priority_when_multiple_exists_for_the_same_type()
        {
            // Arrange
            this.AddTypeFormatter(typeof(string), "low priority", new Priority(0));
            this.AddTypeFormatter(typeof(string), "high priority", new Priority(1));

            // Act
            var result = this.formatter.GetArgumentValueAsString("string value");

            // Assert
            result.Should().Be("high priority");
        }

        [Theory]
        [InlineData("", "string.Empty")]
        [InlineData("string value", "\"string value\"")]
        public void Should_format_string_values_correct_by_default(string value, string expectedResult)
        {
            this.formatter.GetArgumentValueAsString(value).Should().Be(expectedResult);
        }

        [Fact]
        public void Should_prefer_exact_type_formatter_to_interface_formatter()
        {
            // Arrange
            this.AddTypeFormatter(typeof(IEnumerable), "an enumerable");
            this.AddTypeFormatter(typeof(string), "a string");

            // Act
            var result = this.formatter.GetArgumentValueAsString("string value");

            // Assert
            result.Should().Be("a string");
        }

        [Fact]
        public void Built_in_formatters_should_have_lower_than_default_priority()
        {
            // Arrange
            var allArgumentValueFormatters = typeof(A).GetTypeInformation().Assembly.GetTypes()
                .Where(t => t.CanBeInstantiatedAs(typeof(IArgumentValueFormatter)))
                .Select(Activator.CreateInstance)
                .Cast<IArgumentValueFormatter>();

            // Act
            var typesWithNonNegativePriority = allArgumentValueFormatters
                .Where(f => f.Priority >= Priority.Default)
                .Select(f => f.GetType());

            // Assert
            typesWithNonNegativePriority.Should().BeEmpty("because no built-in formatters should have priority equal to or greater than the default");
        }

        private void AddTypeFormatter<T>(string formattedValue)
        {
            this.AddTypeFormatter(typeof(T), formattedValue);
        }

        private void AddTypeFormatter(Type type, string formattedValue)
        {
            this.AddTypeFormatter(type, formattedValue, Priority.Default);
        }

        private void AddTypeFormatter(Type type, string formattedValue, Priority priority)
        {
            var newFormatter = A.Fake<IArgumentValueFormatter>();
            A.CallTo(() => newFormatter.ForType).Returns(type);
            A.CallTo(() => newFormatter.GetArgumentValueAsString(A<object>._)).Returns(formattedValue);
            A.CallTo(() => newFormatter.Priority).Returns(priority);
            this.registeredTypeFormatters.Add(newFormatter);
        }
    }
}
