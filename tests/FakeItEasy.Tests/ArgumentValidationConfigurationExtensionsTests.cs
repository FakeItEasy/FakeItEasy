namespace FakeItEasy.Tests
{
    using System;
    using System.Linq;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class ArgumentValidationConfigurationExtensionsTests
    {
        [Fact]
        public void WithAnyArguments_with_void_call_should_call_when_arguments_match_with_predicate_that_returns_true()
        {
            // Arrange
            var configuration = A.Fake<IArgumentValidationConfiguration<IVoidConfiguration>>();

            // Act
            configuration.WithAnyArguments();

            // Assert
            var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection, bool>>(0);

            predicate.Invoke(null).Should().BeTrue();
        }

        [Fact]
        public void WithAnyArguments_with_function_call_should_call_when_arguments_match_with_predicate_that_returns_true()
        {
            // Arrange
            var configuration = A.Fake<IArgumentValidationConfiguration<IReturnValueConfiguration<int>>>();

            // Act
            configuration.WithAnyArguments();

            // Assert
            var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection, bool>>(0);

            predicate.Invoke(null).Should().BeTrue();
        }

        [Fact]
        public void WithAnyArguments_should_throw_when_configuration_is_null()
        {
            // Arrange
            IArgumentValidationConfiguration<IVoidConfiguration> validationConfig = null;

            // Act
            var exception = Record.Exception(() => validationConfig.WithAnyArguments());

            // Assert
            exception.Should().BeAnExceptionOfType<ArgumentNullException>();
        }
    }
}
