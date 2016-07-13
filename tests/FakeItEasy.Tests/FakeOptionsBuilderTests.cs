namespace FakeItEasy.Tests
{
#if !NETCORE
    extern alias mscorlib;
#endif
    using System;
    using System.Linq;
    using FakeItEasy.Creation;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;
#if NETCORE
    using System.Reflection;
#else
    using mscorlib::System.Reflection;
#endif

    public class FakeOptionsBuilderTests
    {
        [Fact]
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

        [Fact]
        public void Built_in_options_builders_should_have_lower_than_default_priority()
        {
            // Arrange
            var allOptionsBuilders = typeof(A).GetTypeInfo().Assembly.GetTypes()
                .Where(t => t.CanBeInstantiatedAs(typeof(IFakeOptionsBuilder)))
                .Select(Activator.CreateInstance)
                .Cast<IFakeOptionsBuilder>();

            // Act
            var typesWithNonNegativePriority = allOptionsBuilders
                .Where(f => f.Priority >= Priority.Default)
                .Select(f => f.GetType());

            // Assert
            typesWithNonNegativePriority.Should().BeEmpty("because no built-in options builders should have priority equal to or greater than the default");
        }

        private class FakeOptionsBuilderTestsOptionsBuilder : FakeOptionsBuilder<FakeOptionsBuilderTests>
        {
            protected override void BuildOptions(IFakeOptions<FakeOptionsBuilderTests> options)
            {
            }
        }
    }
}
