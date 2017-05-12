namespace FakeItEasy.Tests
{
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FluentAssertions;
    using Xunit;

    public class AssertConfigurationExtensionsTests
    {
        [Fact]
        public void MustHaveHappened_should_call_configuration_with_repeat_once()
        {
            // Arrange
            var configuration = A.Fake<IAssertConfiguration>();

            // Act
            configuration.MustHaveHappened();

            // Assert
            A.CallTo(() => configuration.MustHaveHappened(A<Repeated>.That.Matches(x => x.Matches(1))))
                .MustHaveHappened(Repeated.Exactly.Once); // avoid .MustHaveHappened(), since we're testing it
        }

        [Fact]
        public void MustHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => A.Fake<IAssertConfiguration>().MustHaveHappened();
            call.Should().BeNullGuarded();
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(3, false)]
        public void MustNotHaveHappened_should_call_configuration_with_repeat_that_validates_correctly(int repeat, bool expectedResult)
        {
            // Arrange
            var configuration = A.Fake<IAssertConfiguration>();

            // Act
            configuration.MustNotHaveHappened();

            // Assert
            var specifiedRepeat = Fake.GetCalls(configuration).Single().Arguments.Get<Repeated>(0);
            specifiedRepeat.Matches(repeat).Should().Be(expectedResult);
        }

        [Fact]
        public void MustNotHaveHappened_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            Expression<System.Action> call = () => A.Fake<IAssertConfiguration>().MustNotHaveHappened();
            call.Should().BeNullGuarded();
        }
    }
}
