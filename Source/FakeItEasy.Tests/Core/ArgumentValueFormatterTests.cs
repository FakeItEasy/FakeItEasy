namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Core;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentValueFormatterTests
    {
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private readonly object[] specificCases = TestCases.Create(
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
            }).AsTestCaseSource();

        private ArgumentValueFormatter formatter;
        private List<IArgumentValueFormatter> registeredTypeFormatters;

        [SetUp]
        public void Setup()
        {
            this.registeredTypeFormatters = new List<IArgumentValueFormatter>();

            this.formatter = new ArgumentValueFormatter(this.registeredTypeFormatters);
        }

        [Test]
        public void Should_write_null_values_correct()
        {
            // Arrange

            // Act
            var description = this.formatter.GetArgumentValueAsString(null);

            // Assert
            description.Should().Be("<NULL>");
        }

        [Test]
        public void Should_use_ToString_when_no_matching_formatter_is_registered()
        {
            // Arrange

            // Act
            var description = this.formatter.GetArgumentValueAsString(1);

            // Assert
            description.Should().Be("1");
        }

        [Test]
        public void Should_use_injected_formatter_when_available()
        {
            // Arrange
            this.AddTypeFormatter<DateTime>("Y2K");

            // Act
            var result = this.formatter.GetArgumentValueAsString(new DateTime(2000, 1, 1));

            // Assert
            result.Should().Be("Y2K");
        }

        [Test]
        public void Should_use_injected_formatter_that_is_for_base_type_of_value()
        {
            // Arrange
            this.AddTypeFormatter<Stream>("stream");

            // Act
            var result = this.formatter.GetArgumentValueAsString(new MemoryStream());

            // Assert
            result.Should().Be("stream");
        }

        [TestCaseSource("specificCases")]
        public void Should_favor_most_specific_formatter_when_more_than_one_is_applicable(Type lessSpecific, Type moreSpecific, object value)
        {
            // Arrange
            this.AddTypeFormatter(lessSpecific, "less specific");
            this.AddTypeFormatter(moreSpecific, "more specific");

            // Act
            var result = this.formatter.GetArgumentValueAsString(value);

            // Assert
            Assert.That(result, Is.EqualTo("more specific"), "Less specific type formatter was used.");
        }

        [Test]
        public void Should_use_formatter_with_highest_priority_when_multiple_exists_for_the_same_type()
        {
            // Arrange
            this.AddTypeFormatter(typeof(string), "low priority", 0);
            this.AddTypeFormatter(typeof(string), "high priority", 1);

            // Act
            var result = this.formatter.GetArgumentValueAsString("string value");

            // Assert
            result.Should().Be("high priority");
        }

        [TestCase("", Result = "string.Empty")]
        [TestCase("string value", Result = "\"string value\"")]
        public string Should_format_string_values_correct_by_default(string value)
        {
            return this.formatter.GetArgumentValueAsString(value);
        }

        [Test]
        public void Should_prefer_exact_type_formatter_to_interface_formatter()
        {
            // Arrange
            this.AddTypeFormatter(typeof(IEnumerable), "an enumerable", 0);
            this.AddTypeFormatter(typeof(string), "a string", 0);

            // Act
            var result = this.formatter.GetArgumentValueAsString("string value");

            // Assert
            result.Should().Be("a string");
        }

        [Test]
        public void Provided_formatters_should_have_negative_priority()
        {
            // Arrange
            var allArgumentValueFormatters = typeof(A).Assembly.GetTypes()
                .Where(t => t.CanBeInstantiatedAs(typeof(IArgumentValueFormatter)))
                .Select(Activator.CreateInstance)
                .Cast<IArgumentValueFormatter>();

            // Act
            var typesWithNonNegativePriority = allArgumentValueFormatters
                .Where(f => f.Priority >= new Priority(0))
                .Select(f => f.GetType());

            // Assert
            Assert.That(
                typesWithNonNegativePriority,
                Is.Empty,
                "because no formatters should have non-negative priority");
        }

        private void AddTypeFormatter<T>(string formattedValue)
        {
            this.AddTypeFormatter(typeof(T), formattedValue);
        }

        private void AddTypeFormatter(Type type, string formattedValue)
        {
            this.AddTypeFormatter(type, formattedValue, short.MinValue);
        }

        private void AddTypeFormatter(Type type, string formattedValue, short priority)
        {
            var newFormatter = A.Fake<IArgumentValueFormatter>();
            A.CallTo(() => newFormatter.ForType).Returns(type);
            A.CallTo(() => newFormatter.GetArgumentValueAsString(A<object>._)).Returns(formattedValue);
            A.CallTo(() => newFormatter.Priority).Returns(new Priority(priority));
            this.registeredTypeFormatters.Add(newFormatter);
        }
    }
}
