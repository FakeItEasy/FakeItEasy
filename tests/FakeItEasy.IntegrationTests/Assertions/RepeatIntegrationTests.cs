namespace FakeItEasy.IntegrationTests.Assertions
{
    using FakeItEasy.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class RepeatIntegrationTests
    {
        private IFoo foo;

        [SetUp]
        public void Setup()
        {
            this.foo = A.Fake<IFoo>();
        }

        [Test]
        public void Assert_happened_once_exactly_should_pass_when_call_has_been_made_once()
        {
            // Arrange
            this.foo.Bar();

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Assert_happened_once_exactly_should_fail_when_call_never_happened()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Test]
        public void Assert_happened_once_exactly_should_fail_when_call_happened_more_than_once()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();

            // Act

            // Assert
            Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Test]
        public void Assert_happened_once_should_fail_when_call_never_happened()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened());
        }

        [Test]
        public void Assert_happened_once_should_pass_when_call_happened_once()
        {
            // Arrange
            this.foo.Bar();

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).MustHaveHappened();
        }

        [Test]
        public void Assert_happened_once_should_pass_when_call_happened_twice()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).MustHaveHappened();
        }

        [Test]
        public void Assert_happened_times_should_pass_when_call_has_happened_specified_number_of_times()
        {
            // Arrange
            this.foo.Bar();
            this.foo.Bar();
            this.foo.Bar();

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.AtLeast.Times(3));
        }

        [Test]
        public void Assert_happened_never_should_pass_when_no_call_has_been_made()
        {
            // Arrange

            // Act

            // Assert
            A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Never);
        }

        [Test]
        public void Assert_happened_never_should_fail_when_a_call_has_been_made()
        {
            // Arrange
            this.foo.Bar();

            // Act

            // Assert
            Assert.Throws<ExpectationException>(() =>
                A.CallTo(() => this.foo.Bar()).MustHaveHappened(Repeated.Never));
        }
    }
}
