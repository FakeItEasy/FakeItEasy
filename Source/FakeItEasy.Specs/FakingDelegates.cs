namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class FakingDelegates
    {
        [Scenario]
        public void when_faking_a_delegate_type_and_invoking_without_configuration(
            Func<string, int> fakedDelegate)
        {
            "establish"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "when faking a delegate type"
                .x(() => fakedDelegate.Invoke("foo"));

            "it should be possible to assert the call"
                .x(() => A.CallTo(() => fakedDelegate.Invoke("foo")).MustHaveHappened());

            "it should be possible to assert the call without specifying invoke method"
                .x(() => A.CallTo(() => fakedDelegate("foo")).MustHaveHappened());
        }

        [Scenario]
        public void when_faking_a_delegate_type_and_invoking_with_configuration(
            Func<string, int> fakedDelegate,
            int result)
        {
            "establish"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "establish"
                .x(() => A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Returns(10));

            "when faking a delegate type"
                .x(() => result = fakedDelegate(null));

            "it should return configured value"
                .x(() => result.Should().Be(10));
        }

        [Scenario]
        public void when_faking_a_delegate_type_and_invoking_with_throwing_configuration(
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

            "when faking a delegate type"
                .x(() => exception = Catch.Exception(() => fakedDelegate(null)));

            "it should throw the configured exception"
                .x(() => exception.Should().BeSameAs(expectedException));
        }

        [Scenario]
        public void when_faking_a_delegate_type_and_invoking_with_configuration_without_specifying_invoke_method(
            Func<string, int> fakedDelegate,
            int result)
        {
            "establish"
                .x(() => fakedDelegate = A.Fake<Func<string, int>>());

            "establish"
                .x(() => A.CallTo(() => fakedDelegate(A<string>._)).Returns(10));

            "when faking a delegate type"
                .x(() => result = fakedDelegate(null));

            "it should return configured value"
                .x(() => result.Should().Be(10));
        }
    }
}