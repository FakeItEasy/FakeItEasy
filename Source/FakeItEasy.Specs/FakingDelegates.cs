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
            "establish"._(() =>
            {
                fakedDelegate = A.Fake<Func<string, int>>();
            });

            "when faking a delegate type"._(() =>
            {
                fakedDelegate.Invoke("foo");
            });

            "it should be possible to assert the call"._(() =>
            {
                A.CallTo(() => fakedDelegate.Invoke("foo")).MustHaveHappened();
            });

            "it should be possible to assert the call without specifying invoke method"._(() =>
            {
                A.CallTo(() => fakedDelegate("foo")).MustHaveHappened();
            });
        }

        [Scenario]
        public void when_faking_a_delegate_type_and_invoking_with_configuration(
            Func<string, int> fakedDelegate,
            int result)
        {
            "establish"._(() =>
            {
                fakedDelegate = A.Fake<Func<string, int>>();
            });

            "establish"._(() =>
            {
                A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Returns(10);
            });

            "when faking a delegate type"._(() =>
            {
                result = fakedDelegate(null);
            });

            "it should return configured value"._(() =>
            {
                result.Should().Be(10);
            });
        }

        [Scenario]
        public void when_faking_a_delegate_type_and_invoking_with_throwing_configuration(
            Func<string, int> fakedDelegate,
            FormatException expectedException,
            Exception exception)
        {
            "establish"._(() =>
            {
                fakedDelegate = A.Fake<Func<string, int>>();
            });

            "establish"._(() =>
            {
                expectedException = new FormatException();
                A.CallTo(() => fakedDelegate.Invoke(A<string>._)).Throws(expectedException);
            });

            "when faking a delegate type"._(() =>
            {
                exception = Catch.Exception(() => fakedDelegate(null));
            });

            "it should throw the configured exception"._(() =>
            {
                exception.Should().BeSameAs(expectedException);
            });
        }

        [Scenario]
        public void when_faking_a_delegate_type_and_invoking_with_configuration_without_specifying_invoke_method(
            Func<string, int> fakedDelegate,
            int result)
        {
            "establish"._(() =>
            {
                fakedDelegate = A.Fake<Func<string, int>>();
            });

            "establish"._(() =>
            {
                A.CallTo(() => fakedDelegate(A<string>._)).Returns(10);
            });

            "when faking a delegate type"._(() =>
            {
                result = fakedDelegate(null);
            });

            "it should return configured value"._(() =>
            {
                result.Should().Be(10);
            });
        }
    }
}