namespace FakeItEasy.IntegrationTests
{
    using System;
    using FakeItEasy.Tests;
    using Xunit;

    public class AssertionsTests
    {
        public interface ISomething
        {
            void SomethingMethod();
        }

        [Fact]
        public void Method_that_is_configured_to_throw_should_still_be_recorded()
        {
            // Arrange
            var fake = A.Fake<IFoo>();

            A.CallTo(() => fake.Bar()).Throws(new InvalidOperationException()).Once();

            // Act
            try
            {
                fake.Bar();
            }
            catch (InvalidOperationException)
            {
            }

            // Assert
            A.CallTo(() => fake.Bar()).MustHaveHappened();
        }

        [Fact]
        public void Should_not_throw_when_asserting_while_calls_are_being_made_on_the_fake()
        {
            var fake = A.Fake<ISomething>();

            fake.SomethingMethod();

            A.CallTo(fake)
                .Where(
                    call =>
                    {
                        // Simulate a concurrent call being made in another thread while the `MustHaveHappened` assertion
                        // is being evaluated. This method is unorthodox, but is deterministic, not relying on the
                        // vagaries of the thread scheduler to ensure that the calls overlap.
                        fake.SomethingMethod();
                        return true;
                    },
                    output => { })
                .MustHaveHappened();
        }
    }
}
