namespace FakeItEasy.Tests
{
    using System;
    using System.Linq;
    using FakeItEasy.Creation;
    using FluentAssertions;
    using NUnit.Framework;
    using TestHelpers;

    public class FakeOptionsBuilderTests
    {
        [Test]
        public void BuildOptions_should_throw_when_passed_wrong_type()
        {
            // Arrange
            IFakeOptionsBuilder target = new FakeOptionsBuilderTestsOptionsBuilder();

            // Act
            var exception = Record.Exception(() => target.BuildOptions(typeof(string), A.Fake<IFakeOptions>()));

            // Assert
            exception.Should().BeAnExceptionOfType<InvalidOperationException>()
                .WithMessage("Specified type 'System.String' is not valid. Only 'FakeItEasy.Tests.FakeOptionsBuilderTests' is allowed.");
        }

        [Test]
        public void Provided_options_builders_should_have_negative_priority()
        {
            // Arrange
            var allOptionsBuilders = typeof(A).Assembly.GetTypes()
                .Where(t => t.CanBeInstantiatedAs(typeof(IFakeOptionsBuilder)))
                .Select(Activator.CreateInstance)
                .Cast<IFakeOptionsBuilder>();

            // Act
            var typesWithNonNegativePriority = allOptionsBuilders
                .Where(f => f.Priority >= 0)
                .Select(f => f.GetType());

            // Assert
            typesWithNonNegativePriority.Should().BeEmpty("because no options builders should have non-negative priority");
        }

        private class FakeOptionsBuilderTestsOptionsBuilder : FakeOptionsBuilder<FakeOptionsBuilderTests>
        {
            protected override void BuildOptions(IFakeOptions<FakeOptionsBuilderTests> options)
            {
            }
        }
    }
}
