namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public static class FakingDelegates
    {
        [Scenario]
        public static void WithoutConfiguration(Func<string, int> fakedDelegate)
        {
            "Given a faked delegate"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "When I invoke it via the Invoke method"
                .x(() => fakedDelegate.Invoke("foo"));

            "Then an assertion that Invoke was called passes"
                .x(() => A.CallTo(() => fakedDelegate.Invoke("foo")).MustHaveHappened());

            "And an assertion that the delegate was called without mentioning Invoke passes"
                .x(() => A.CallTo(() => fakedDelegate("foo")).MustHaveHappened());
        }

        [Scenario]
        public static void ConfiguredToReturn(Func<string, int> fakedDelegate, int result)
        {
            "Given a faked delegate"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "And I configure it to return 10"
                .x(() => A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Returns(10));

            "When I invoke it"
                .x(() => result = fakedDelegate.Invoke("foo"));

            "Then it returns the configured value"
                .x(() => result.Should().Be(10));
        }

        [Scenario]
        public static void ConfiguredToReturnLazily(Func<string, int> fakedDelegate, int result)
        {
            "Given a faked delegate"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "And I configure it to return lazily"
                .x(() => A.CallTo(() => fakedDelegate.Invoke(A<string>._)).ReturnsLazily((string s) => Int32.Parse(s)));

            "When I invoke it"
                .x(() => result = fakedDelegate.Invoke("-27"));

            "Then it returns a value constructed from the input"
                .x(() => result.Should().Be(-27));
        }

        [Scenario]
        public static void Throws(Func<string, int> fakedDelegate, FormatException expectedException, Exception exception)
        {
            "Given a faked delegate"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "And I configure it to throw an exception"
                .x(() => A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Throws(expectedException = new FormatException()));

            "When I invoke it"
                .x(() => exception = Record.Exception(() => fakedDelegate(null)));

            "Then it throws the configured exception"
                .x(() => exception.Should().BeSameAs(expectedException));
        }

        [Scenario]
        public static void MissingInvoke(Func<string, int> fakedDelegate, int result)
        {
            "Given a faked delegate"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "And I configure it to return 10 without specifying the Invoke method explicitly"
                .x(() => A.CallTo(() => fakedDelegate(A<string>._)).Returns(10));

            "When I invoke it without specifying the Invoke method explicitly"
                .x(() => result = fakedDelegate(null));

            "Then it returns the configured value"
                .x(() => result.Should().Be(10));
        }
    }
}
