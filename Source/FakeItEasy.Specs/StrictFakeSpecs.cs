namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public interface IMyInterface
    {
        void DoIt(Guid id);
    }

    public class when_asserting_must_have_happened_when_did_not_happen
    {
        static IMyInterface fake;
        static Exception exception;

        Establish context = () =>
        {
            fake = A.Fake<IMyInterface>(o => o.Strict());
            ////fake = A.Fake<IMyInterface>();

            A.CallTo(() => fake.DoIt(Guid.Empty)).Invokes(() => { });

            fake.DoIt(Guid.Empty);
            fake.DoIt(Guid.Empty);
        };

        Because of = () => exception = Record.Exception(() => A.CallTo(() => fake.DoIt(Guid.Empty)).MustHaveHappened(Repeated.Exactly.Once));

        It should_throw_an_expectation_exception = () => exception.Should().BeAnExceptionOfType<ExpectationException>();

        It should_have_an_exception_message_containing_the_name_of_the_method = () => exception.Message.Should().Contain("DoIt");
    }
}
