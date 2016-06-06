namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public interface IMyInterface
    {
        void DoIt(Guid id);
    }

    public static class StrictFakeSpecs
    {
        [Scenario]
        public static void RepeatedAssertion(
            IMyInterface fake,
            Exception exception)
        {
            "establish"
                .x(() =>
                    {
                        fake = A.Fake<IMyInterface>(o => o.Strict());
                        ////fake = A.Fake<IMyInterface>();

                        A.CallTo(() => fake.DoIt(Guid.Empty)).Invokes(() => { });

                        fake.DoIt(Guid.Empty);
                        fake.DoIt(Guid.Empty);
                    });

            "when asserting must have happened when did not happen"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.DoIt(Guid.Empty)).MustHaveHappened(Repeated.Exactly.Once)));

            "it should throw an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "it should have an exception message containing the name of the method"
                .x(() => exception.Message.Should().Contain("DoIt"));
        }
    }
}
