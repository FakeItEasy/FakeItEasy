namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class StrictFakeSpecs
    {
        public interface IMyInterface
        {
            void DoIt(Guid id);
        }

        [Scenario]
        public static void RepeatedAssertion(
            IMyInterface fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IMyInterface>(o => o.Strict()));

            "And I configure the fake to do nothing when a method is called with certain arguments"
                .x(() => A.CallTo(() => fake.DoIt(Guid.Empty)).DoesNothing());

            "And I call the method with matching arguments"
                .x(() => fake.DoIt(Guid.Empty));

            "And I call the method with matching arguments again"
                .x(() => fake.DoIt(Guid.Empty));

            "When I assert that the method must have been called exactly once"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.DoIt(Guid.Empty)).MustHaveHappened(Repeated.Exactly.Once)));

            "Then the assertion throws an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception message names the method"
                .x(() => exception.Message.Should().Contain("DoIt"));
        }
    }
}
