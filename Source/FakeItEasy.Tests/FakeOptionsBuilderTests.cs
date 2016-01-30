namespace FakeItEasy.Tests
{
    using System;
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

        private class FakeOptionsBuilderTestsOptionsBuilder : FakeOptionsBuilder<FakeOptionsBuilderTests>
        {
            protected override void BuildOptions(IFakeOptions<FakeOptionsBuilderTests> options)
            {
            }
        }
    }
}
