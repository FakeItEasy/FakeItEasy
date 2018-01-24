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

            Guid MakeIt(string name);
        }

        [Scenario]
        public static void ArgumentMismatchVoid(
            IMyInterface fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IMyInterface>(o => o.Strict()));

            "And I configure the fake to do nothing when a void method is called with certain arguments"
                .x(() => A.CallTo(() => fake.DoIt(Guid.Empty)).DoesNothing());

            "When I call the method with non-matching arguments"
                .x(() => exception = Record.Exception(() => fake.DoIt(new Guid("a762f030-d39e-4316-92b1-a51eff3dc51f"))));

            "Then the fake throws an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception message describes the call"
                .x(() => exception.Message.Should().Be(
                    "Call to unconfigured method of strict fake: FakeItEasy.Specs.StrictFakeSpecs+IMyInterface.DoIt(id: a762f030-d39e-4316-92b1-a51eff3dc51f)."));
        }

        [Scenario]
        public static void ArgumentMismatchNonVoid(
            IMyInterface fake,
            Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IMyInterface>(o => o.Strict()));

            "And I configure the fake to return a value when a non-void method is called with certain arguments"
                .x(() => A.CallTo(() => fake.MakeIt("empty")).Returns(Guid.Empty));

            "When I call the method with non-matching arguments"
                .x(() => exception = Record.Exception(() => fake.MakeIt("something")));

            "Then the fake throws an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception message describes the call"
                .x(() => exception.Message.Should().Be(
                    @"Call to unconfigured method of strict fake: FakeItEasy.Specs.StrictFakeSpecs+IMyInterface.MakeIt(name: ""something"")."));
        }

        [Scenario]
        public static void CallCountAssertion(
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
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.DoIt(Guid.Empty)).MustHaveHappenedOnceExactly()));

            "Then the assertion throws an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception message names the method"
                .x(() => exception.Message.Should().Contain("DoIt"));
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

            "When I use Repeated to assert that the method must have been called exactly once"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.DoIt(Guid.Empty)).MustHaveHappened(Repeated.Exactly.Once)));

            "Then the assertion throws an expectation exception"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

            "And the exception message names the method"
                .x(() => exception.Message.Should().Contain("DoIt"));
        }
    }
}
