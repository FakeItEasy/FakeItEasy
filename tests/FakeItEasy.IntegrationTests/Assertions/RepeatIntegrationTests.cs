namespace FakeItEasy.IntegrationTests.Assertions
{
    using FakeItEasy.Tests;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xunit;

    public class RepeatIntegrationTests
    {
        private readonly IFoo foo;

        public RepeatIntegrationTests()
        {
            this.foo = A.Fake<IFoo>();
        }

        [Fact]
        public void Assert_happened_once_exactly_should_pass_when_call_has_been_made_once()
        {
            // Arrange
            this.foo.Bar();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Exactly.Once));

            // Assert
            exception.Should().BeNull("because the assertion should have passed");
        }

        [Fact]
        public void Assert_happened_once_exactly_should_fail_when_call_never_happened()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Exactly.Once));

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Fact]
        public void Assert_happened_once_exactly_should_fail_when_call_happened_more_than_once()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Exactly.Once));

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Fact]
        public void Assert_happened_once_should_fail_when_call_never_happened()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened());

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }

        [Fact]
        public void Assert_happened_once_should_pass_when_call_happened_once()
        {
            // Arrange
            this.foo.Bar();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened());

            // Assert
            exception.Should().BeNull("because the assertion should have passed");
        }

        [Fact]
        public void Assert_happened_once_should_pass_when_call_happened_twice()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened());

            // Assert
            exception.Should().BeNull("because the assertion should have passed");
        }

        [Fact]
        public void Assert_happened_times_should_pass_when_call_has_happened_specified_number_of_times()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();
            this.foo.Bar();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.AtLeast.Times(3)));

            // Assert
            exception.Should().BeNull("because the assertion should have passed");
        }

        [Fact]
        public void Assert_happened_never_should_pass_when_no_call_has_been_made()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Never));

            // Assert
            exception.Should().BeNull("because the assertion should have passed");
        }

        [Fact]
        public void Assert_happened_never_should_fail_when_a_call_has_been_made()
        {
            // Arrange
            this.foo.Bar();

            // Act
            var exception = Record.Exception(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Never));

            // Assert
            exception.Should().BeAnExceptionOfType<ExpectationException>();
        }
    }
}
