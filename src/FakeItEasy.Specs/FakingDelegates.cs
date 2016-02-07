namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class FakingDelegates
    {
        [Scenario]
        public static void WithoutConfiguration(
            Func<string, int> fakedDelegate)
        {
            "establish"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "when faking a delegate type and invoking without configuration"
                .x(() => fakedDelegate.Invoke("foo"));

            "it should be possible to assert the call"
                .x(() => A.CallTo(() => fakedDelegate.Invoke("foo")).MustHaveHappened());

            "it should be possible to assert the call without specifying invoke method"
                .x(() => A.CallTo(() => fakedDelegate("foo")).MustHaveHappened());
        }

        [Scenario]
        public static void WithConfiguration(
            Func<string, int> fakedDelegate,
            int result)
        {
            "establish"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "establish"
                .x(() => A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Returns(10));

            "when faking a delegate type and invoking with configuration"
                .x(() => result = fakedDelegate(null));

            "it should return configured value"
                .x(() => result.Should().Be(10));
        }

        [Scenario]
        public static void Throws(
            Func<string, int> fakedDelegate,
            FormatException expectedException,
            Exception exception)
        {
            "establish"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "establish"
                .x(() =>
                    {
                        expectedException = new FormatException();
                        A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Throws(expectedException);
                    });

            "when faking a delegate type and invoking with throwing configuration"
                .x(() => exception = Record.Exception(() => fakedDelegate(null)));

            "it should throw the configured exception"
                .x(() => exception.Should().BeSameAs(expectedException));
        }

        [Scenario]
        public static void MissingInvoke(
            Func<string, int> fakedDelegate,
            int result)
        {
            "establish"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "establish"
                .x(() => A.CallTo(() => fakedDelegate(A<string>._)).Returns(10));

            "when faking a delegate type and invoking with configuration without specifying invoke method"
                .x(() => result = fakedDelegate(null));

            "it should return configured value"
                .x(() => result.Should().Be(10));
        }
    }
}
