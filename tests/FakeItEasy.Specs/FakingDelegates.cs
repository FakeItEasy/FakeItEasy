namespace FakeItEasy.Specs;

using System;
using FakeItEasy.Configuration;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using LambdaTale;
using Xunit;

public static class FakingDelegates
{
    public delegate void VoidDelegateWithOutAndRefValues(out string outString, ref int refInt);

    public delegate int NonVoidDelegateWithOutAndRefValues(ref string? refString, out int outInt);

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
            .x(() => A.CallTo(() => fakedDelegate.Invoke(A<string>._)).ReturnsLazily((string s) => int.Parse(s)));

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
            .x(() => exception = Record.Exception(() => fakedDelegate(string.Empty)));

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
            .x(() => result = fakedDelegate(string.Empty));

        "Then it returns the configured value"
            .x(() => result.Should().Be(10));
    }

    [Scenario]
    public static void CannotBeConfiguredToCallBaseMethod(Action fake, Exception exception)
    {
        "Given a fake delegate"
            .x(() => fake = A.Fake<Action>());

        "When I configure it to call its base method"
            .x(() => exception = Record.Exception(() => A.CallTo(() => fake.Invoke()).CallsBaseMethod()));

        "Then it throws a not supported exception"
            .x(() => exception.Should()
                .BeAnExceptionOfType<FakeConfigurationException>()
                .WithMessage("Can not configure a delegate proxy to call a base method."));
    }

    [Scenario]
    public static void SetOutAndRefVoid(VoidDelegateWithOutAndRefValues fake, string theOutParameter, int theRefParameter)
    {
        "Given a fake delegate with void return and out and ref parameters"
            .x(() => fake = A.Fake<VoidDelegateWithOutAndRefValues>());

        "And I configure it to set ref and out parameters"
            .x(() =>
            {
                string outString;
                int refInt = 0;
                A.CallTo(() => fake.Invoke(out outString, ref refInt)).AssignsOutAndRefParameters("fancy out string", 3);
            });

        "When I invoke it"
            .x(() => fake.Invoke(out theOutParameter, ref theRefParameter));

        "Then it sets the proper out value"
            .x(() => theOutParameter.Should().Be("fancy out string"));

        "And it sets the proper ref value"
            .x(() => theRefParameter.Should().Be(3));
    }

    [Scenario]
    public static void SetOutAndRefNonVoid(NonVoidDelegateWithOutAndRefValues fake, string? theRefParameter, int theOutParameter)
    {
        "Given a fake delegate with non-void return and out and ref parameters"
            .x(() => fake = A.Fake<NonVoidDelegateWithOutAndRefValues>());

        "And I configure it to set ref and out parameters"
            .x(() =>
            {
                string? refString = null;
                int outInt;
                A.CallTo(() => fake.Invoke(ref refString, out outInt)).AssignsOutAndRefParameters("fancy ref string", 5);
            });

        "When I invoke it"
            .x(() => fake.Invoke(ref theRefParameter, out theOutParameter));

        "Then it sets the proper ref value"
            .x(() => theRefParameter.Should().Be("fancy ref string"));

        "And it sets the proper out value"
            .x(() => theOutParameter.Should().Be(5));
    }
}
