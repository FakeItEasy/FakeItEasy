namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class AssertingCallCountSpecs
    {
        public interface IFoo
        {
            void Method();
        }

        [Scenario]
        public static void MustHaveHappenedNoCalls(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make no calls to a faked method"
                .x(() => { });

            "When I assert that the call must have happened"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Method()).MustHaveHappened()));

            "Then the assertion fails"
                .x(() => exception.Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage(@"

  Assertion failed for the following call:
    FakeItEasy.Specs.AssertingCallCountSpecs+IFoo.Method()
  Expected to find it at least once but no calls were made to the fake object.

"));
        }

        [Scenario]
        public static void MustHaveHappened1Call(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make 1 call to a faked method"
                .x(() => fake.Method());

            "When I assert that the call must have happened"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Method()).MustHaveHappened()));

            "Then the assertion succeeds"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void MustHaveHappened4Calls(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make 4 calls to a faked method"
                .x(() =>
                {
                    fake.Method();
                    fake.Method();
                    fake.Method();
                    fake.Method();
                });

            "When I assert that the call must have happened"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Method()).MustHaveHappened()));

            "Then the assertion succeeds"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void MustNotHaveHappenedNoCalls(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make no calls to a faked method"
                .x(() => { });

            "When I assert that the call must not have happened"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Method()).MustNotHaveHappened()));

            "Then the assertion succeeds"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public static void MustNotHaveHappened1Call(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make 1 call to a faked method"
                .x(() => fake.Method());

            "When I assert that the call must not have happened"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Method()).MustNotHaveHappened()));

            "Then the assertion fails"
                .x(() => exception.Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage(@"

  Assertion failed for the following call:
    FakeItEasy.Specs.AssertingCallCountSpecs+IFoo.Method()
  Expected to find it never but found it once among the calls:
    1: FakeItEasy.Specs.AssertingCallCountSpecs+IFoo.Method()

"));
        }

        [Scenario]
        public static void MustNotHaveHappened3Calls(IFoo fake, Exception exception)
        {
            "Given a Fake"
                .x(() => fake = A.Fake<IFoo>());

            "And I make 3 calls to a faked method"
                .x(() =>
                {
                    fake.Method();
                    fake.Method();
                    fake.Method();
                });

            "When I assert that the call must not have happened"
                .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Method()).MustNotHaveHappened()));

            "Then the assertion fails"
                .x(() => exception.Should()
                    .BeAnExceptionOfType<ExpectationException>()
                    .WithMessage(@"

  Assertion failed for the following call:
    FakeItEasy.Specs.AssertingCallCountSpecs+IFoo.Method()
  Expected to find it never but found it 3 times among the calls:
    1: FakeItEasy.Specs.AssertingCallCountSpecs+IFoo.Method() 3 times
    ..."));
        }
    }
}
