namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;


    public interface IMyInterface
    {
        void DoIt(Guid id);
    }

    public class StrictFake
    {
        [Scenario]
        public void when_asserting_must_have_happened_when_did_not_happen(
            IMyInterface fake,
            Exception exception)
        {
            "establish"._(() =>
            {
                fake = A.Fake<IMyInterface>(o => o.Strict());
                ////fake = A.Fake<IMyInterface>();

                A.CallTo(() => fake.DoIt(Guid.Empty)).Invokes(() => { });

                fake.DoIt(Guid.Empty);
                fake.DoIt(Guid.Empty);
            });

            "when asserting must have happened when did not happen"._(() =>
            {
                exception = Record.Exception(() => A.CallTo(() => fake.DoIt(Guid.Empty)).MustHaveHappened(Repeated.Exactly.Once));
            });

            "it should throw an expectation exception"._(() =>
            {
                exception.Should().BeAnExceptionOfType<ExpectationException>();
            });

            "it should have an exception message containing the name of the method"._(() =>
            {
                exception.Message.Should().Contain("DoIt");
            });
        }
    }
}
