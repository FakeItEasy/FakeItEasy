namespace FakeItEasy.Tests
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FluentAssertions;
    using Xunit;

    public class WhereConfigurationExtensionsTests
    {
        [Fact]
        public void Where_should_return_configuration_from_configuration()
        {
            // Arrange
            var configurationToReturn = A.Dummy<IVoidConfiguration>();

            var configuration = A.Fake<IWhereConfiguration<IVoidConfiguration>>();
            A.CallTo(configuration).WithReturnType<IVoidConfiguration>().Returns(configurationToReturn);

            // Act
            var returnedConfiguration = configuration.Where(x => true);

            // Assert
            returnedConfiguration.Should().BeSameAs(returnedConfiguration);
        }

        [Fact]
        public void Where_should_pass_writer_that_writes_predicate_as_string()
        {
            // Arrange
            var configuration = A.Fake<IWhereConfiguration<IVoidConfiguration>>();

            // Act
            configuration.Where(x => true);

            // Assert
            A.CallTo(() => configuration.Where(
                A<Func<IFakeObjectCall, bool>>._,
                A<Action<IOutputWriter>>.That.Writes("x => True"))).MustHaveHappened();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Where_should_pass_compiled_predicate_to_configuration(bool predicateReturnValue)
        {
            // Arrange
            var configuration = A.Fake<IWhereConfiguration<IVoidConfiguration>>();

            // Act
            configuration.Where(x => predicateReturnValue);

            // Assert
            A.CallTo(() => configuration.Where(
                A<Func<IFakeObjectCall, bool>>.That.Returns(A.Dummy<IFakeObjectCall>(), predicateReturnValue),
                A<Action<IOutputWriter>>._)).MustHaveHappened();
        }
    }
}
