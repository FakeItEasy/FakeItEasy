namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentValueFormatterTests
    {
        private ArgumentValueFormatter formatter;
        private List<IArgumentValueFormatter> registeredTypeFormatters;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used reflectively.")]
        private object[] specificCases = TestCases.Create(
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
            Assert.That(description, Is.EqualTo("<NULL>"));
        }

        [Test]
        public void Should_use_ToString_when_no_matching_formatter_is_registered()
        {
            // Arrange

            // Act
            var description = this.formatter.GetArgumentValueAsString(1);

            // Assert
            Assert.That(description, Is.EqualTo("1"));
        }

        [Test]
        public void Should_use_injected_formatter_when_available()
        {
            // Arrange
            this.AddTypeFormatter<DateTime>("Y2K");

            // Act
            var result = this.formatter.GetArgumentValueAsString(new DateTime(2000, 1, 1));

            // Assert
            Assert.That(result, Is.EqualTo("Y2K"));
        }

        [Test]
        public void Should_use_injected_formatter_that_is_for_base_type_of_value()
        {
            // Arrange
            this.AddTypeFormatter<Stream>("stream");

            // Act
            var result = this.formatter.GetArgumentValueAsString(new MemoryStream());

            // Assert
            Assert.That(result, Is.EqualTo("stream"));
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
            Assert.That(result, Is.EqualTo("high priority"));
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
            Assert.That(result, Is.EqualTo("a string"));
        }

        private void AddTypeFormatter<T>(string formattedValue)
        {
            this.AddTypeFormatter(typeof(T), formattedValue);
        }

        private void AddTypeFormatter(Type type, string formattedValue)
        {
            this.AddTypeFormatter(type, formattedValue, int.MinValue);
        }

        private void AddTypeFormatter(Type type, string formattedValue, int priority)
        {
            var formatter = A.Fake<IArgumentValueFormatter>();
            A.CallTo(() => formatter.ForType).Returns(type);
            A.CallTo(() => formatter.GetArgumentValueAsString(A<object>._)).Returns(formattedValue);
            A.CallTo(() => formatter.Priority).Returns(priority);
            this.registeredTypeFormatters.Add(formatter);
        }
    }
}
