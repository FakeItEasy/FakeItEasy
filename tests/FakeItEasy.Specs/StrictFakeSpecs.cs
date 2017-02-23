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

            int GetIt(IFoo foo);
        }

        public interface IFoo
        {
        }

        [Scenario]
        public static void RepeatedAssertion(
            IMyInterface fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IMyInterface>(o => o.Strict()));

            "And a call configured to invoke a delegate"
                .x(() => A.CallTo(() => fake.DoIt(Guid.Empty)).Invokes(() => { }));

            "When I call the configured method twice with a matching argument"
                .x(() =>
                    {
                        fake.DoIt(Guid.Empty);
                        fake.DoIt(Guid.Empty);
                    });

            "And I assert that the call happened exactly once"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.DoIt(Guid.Empty)).MustHaveHappened(Repeated.Exactly.Once)));

            "Then it should throw an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And it should have an exception message containing the name of the method"
                .x(() => exception.Message.Should().Contain("DoIt"));
        }

        [Scenario]
        public static void CallDescriptionWithStrictFakeParameter(
            IMyInterface fake,
            IFoo parameter,
            Exception ex)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IMyInterface>());

            "And a strict fake parameter"
                .x(() => parameter = A.Fake<IFoo>(o => o.Strict()));

            "When I assert that the fake has been called with the fake parameter"
                .x(() => ex = Record.Exception(() => A.CallTo(() => fake.GetIt(parameter)).MustHaveHappened()));

            "Then it throws an expectation exception"
                .x(() => ex.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception message should say that the call didn't happen"
                .x(() => ex.Message
                        .Should().Contain("Assertion failed for the following call:")
                        .And.Contain("FakeItEasy.Specs.StrictFakeSpecs+IMyInterface.GetIt(Faked FakeItEasy.Specs.StrictFakeSpecs+IFoo)"));
        }

        [Scenario]
        public static void CallMatchingSuccessWithStrictFakeParameter(
            IMyInterface fake,
            IFoo parameter,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IMyInterface>());

            "And a strict fake parameter"
                .x(() => parameter = A.Fake<IFoo>(o => o.Strict()));

            "And the fake is configured to return a value when called with the parameter"
                .x(() => A.CallTo(() => fake.GetIt(parameter)).Returns(42));

            "When I call the fake with the parameter"
                .x(() => result = fake.GetIt(parameter));

            "Then it returns the configured value"
                .x(() => result.Should().Be(42));
        }

        [Scenario]
        public static void CallMatchingFailureWithStrictFakeParameter(
            IMyInterface fake,
            IFoo parameter1,
            IFoo parameter2,
            int result)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IMyInterface>());

            "And a strict fake parameter"
                .x(() => parameter1 = A.Fake<IFoo>(o => o.Strict()));

            "And another strict fake parameter"
                .x(() => parameter2 = A.Fake<IFoo>(o => o.Strict()));

            "And the fake is configured to return a value when called with the first parameter"
                .x(() => A.CallTo(() => fake.GetIt(parameter1)).Returns(42));

            "When I call the fake with the second parameter"
                .x(() => result = fake.GetIt(parameter2));

            "Then it doesn't return the configured value"
                .x(() => result.Should().Be(0));
        }
    }
}
