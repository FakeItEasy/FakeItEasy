namespace FakeItEasy.Specs;

using System;
using FakeItEasy.Tests.TestHelpers;
using FakeItEasy.Tests.TestHelpers.FSharp;
using FluentAssertions;
using LambdaTale;
using Xunit;

public static class StrictFakeSpecs
{
    public interface IMyInterface
    {
        void DoIt(Guid id);

        Guid MakeIt(string name);

        event EventHandler Event;
    }

    [Scenario]
    [InlineData(StrictFakeOptions.AllowEquals)]
    [InlineData(StrictFakeOptions.AllowObjectMethods)]
    public static void CallToEqualsAllowed(
        StrictFakeOptions strictOptions,
        IMyInterface fake,
        Exception exception)
    {
        "Given a strict fake that allows calls to Equals"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(strictOptions)));

        "When I call Equals on the fake"
            .x(() => exception = Record.Exception(
                () => fake.Equals(null)));

        "Then it shouldn't throw an exception"
            .x(() => exception.Should().BeNull());
    }

    [Scenario]
    [InlineData(StrictFakeOptions.None)]
    [InlineData(StrictFakeOptions.AllowGetHashCode)]
    [InlineData(StrictFakeOptions.AllowToString)]
    public static void CallToEqualsNotAllowed(
        StrictFakeOptions strictOptions,
        IMyInterface fake,
        Exception exception)
    {
        "Given a strict fake that doesn't allow calls to Equals"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(strictOptions)));

        "When I call Equals on the fake"
            .x(() => exception = Record.Exception(
                () => fake.Equals(null)));

        "Then it should throw an exception"
            .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
    }

    [Scenario]
    public static void CallToOverriddenEqualsAllowed(
        OverridingObject fake,
        Exception exception)
    {
        "Given a type that overrides Equals"
            .See<OverridingObject>();

        "And a strict fake of that type that allows calls to Equals"
            .x(() => fake = A.Fake<OverridingObject>(options =>
                options.Strict(StrictFakeOptions.AllowEquals)));

        "When I call Equals on the fake"
            .x(() => exception = Record.Exception(
                () => fake.Equals(default)));

        "Then it shouldn't throw an exception"
            .x(() => exception.Should().BeNull());
    }

    [Scenario]
    [InlineData(StrictFakeOptions.AllowGetHashCode)]
    [InlineData(StrictFakeOptions.AllowObjectMethods)]
    public static void CallToGetHashCodeAllowed(
        StrictFakeOptions strictOptions,
        IMyInterface fake,
        Exception exception)
    {
        "Given a strict fake that allows calls to GetHashCode"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(strictOptions)));

        "When I call GetHashCode on the fake"
            .x(() => exception = Record.Exception(
                () => fake.GetHashCode()));

        "Then it shouldn't throw an exception"
            .x(() => exception.Should().BeNull());
    }

    [Scenario]
    [InlineData(StrictFakeOptions.None)]
    [InlineData(StrictFakeOptions.AllowEquals)]
    [InlineData(StrictFakeOptions.AllowToString)]
    public static void CallToGetHashCodeNotAllowed(
        StrictFakeOptions strictOptions,
        IMyInterface fake,
        Exception exception)
    {
        "Given a strict fake that doesn't allow calls to GetHashCode"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(strictOptions)));

        "When I call GetHashCode on the fake"
            .x(() => exception = Record.Exception(
                () => fake.GetHashCode()));

        "Then it should throw an exception"
            .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
    }

    [Scenario]
    public static void CallToOverriddenGetHashCodeAllowed(
        OverridingObject fake,
        Exception exception)
    {
        "Given a type that overrides GetHashCode"
            .See<OverridingObject>();

        "And a strict fake of that type that allows calls to GetHashCode"
            .x(() => fake = A.Fake<OverridingObject>(options =>
                options.Strict(StrictFakeOptions.AllowGetHashCode)));

        "When I call Equals on the fake"
            .x(() => exception = Record.Exception(
                () => fake.GetHashCode()));

        "Then it shouldn't throw an exception"
            .x(() => exception.Should().BeNull());
    }

    [Scenario]
    [InlineData(StrictFakeOptions.AllowToString)]
    [InlineData(StrictFakeOptions.AllowObjectMethods)]
    public static void CallToToStringAllowed(
        StrictFakeOptions strictOptions,
        IMyInterface fake,
        Exception exception)
    {
        "Given a strict fake that allows calls to ToString"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(strictOptions)));

        "When I call ToString on the fake"
            .x(() => exception = Record.Exception(
                () => fake.ToString()));

        "Then it shouldn't throw an exception"
            .x(() => exception.Should().BeNull());
    }

    [Scenario]
    [InlineData(StrictFakeOptions.None)]
    [InlineData(StrictFakeOptions.AllowEquals)]
    [InlineData(StrictFakeOptions.AllowGetHashCode)]
    public static void CallToToStringNotAllowed(
        StrictFakeOptions strictOptions,
        IMyInterface fake,
        Exception exception)
    {
        "Given a strict fake that doesn't allow calls to ToString"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(strictOptions)));

        "When I call ToString on the fake"
            .x(() => exception = Record.Exception(
                () => fake.ToString()));

        "Then it should throw an exception"
            .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
    }

    [Scenario]
    public static void CallToOverriddenToStringAllowed(
        OverridingObject fake,
        Exception exception)
    {
        "Given a type that overrides ToString"
            .See<OverridingObject>();

        "And a strict fake of that type that allows calls to ToString"
            .x(() => fake = A.Fake<OverridingObject>(options =>
                options.Strict(StrictFakeOptions.AllowToString)));

        "When I call Equals on the fake"
            .x(() => exception = Record.Exception(
                () => fake.ToString()));

        "Then it shouldn't throw an exception"
            .x(() => exception.Should().BeNull());
    }

    [Scenario]
    public static void EventsManaged(IMyInterface fake, EventHandler handler)
    {
        "Given a strict fake that manages events"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(StrictFakeOptions.AllowEvents)));

        "And an event handler"
            .x(() => handler = A.Fake<EventHandler>());

        "When the handler is subscribed to the fake's event"
            .x(() => fake.Event += handler);

        "And the event is raised using Raise"
            .x(() => fake.Event += Raise.WithEmpty());

        "Then the handler is called"
            .x(() => A.CallTo(handler).MustHaveHappened());
    }

    [Scenario]
    public static void EventsNotManaged(
        IMyInterface fake,
        EventHandler handler,
        Exception exception)
    {
        "Given a strict fake that doesn't manages events"
            .x(() => fake = A.Fake<IMyInterface>(options =>
                options.Strict(StrictFakeOptions.None)));

        "And an event handler"
            .x(() => handler = A.Fake<EventHandler>());

        "When the handler is subscribed to the fake's event"
            .x(() => exception = Record.Exception(() => fake.Event += handler));

        "Then an exception is thrown"
            .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());

        "And the exception message directs the user to Manage.Event"
            .x(() => exception.Message.Should().Contain("Manage.Event"));
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
    public static void NameIncludedInExpectationException(
        IMyInterface fake,
        Exception exception)
    {
        "Given a named strict fake"
            .x(() => fake = A.Fake<IMyInterface>(o => o.Strict().Named("Foo1")));

        "And I call an unconfigured method"
            .x(() => exception = Record.Exception(() => fake.MakeIt("argument")));

        "Then it should include configured name in the thrown exception"
            .x(() => exception
                .Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage(@"Call to unconfigured method of strict fake: FakeItEasy.Specs.StrictFakeSpecs+IMyInterface.MakeIt(name: ""argument"") on Foo1."));
    }

    [Scenario]
    public static void AnonymousParameterInExpectationException(
        IHaveAMethodWithAnAnonymousParameter fake,
        Exception exception)
    {
        "Given a strict fake that has a method with anonymous parameters"
            .x(() => fake = A.Fake<IHaveAMethodWithAnAnonymousParameter>(o => o.Strict()));

        "And I call an unconfigured method"
            .x(() => exception = Record.Exception(() => fake.Save(23978)));

        "Then the thrown exception should include a placeholder parameter name"
            .x(() => exception
                .Should()
                .BeAnExceptionOfType<ExpectationException>()
                .WithMessage(@"Call to unconfigured method of strict fake: FakeItEasy.Tests.TestHelpers.FSharp.IHaveAMethodWithAnAnonymousParameter.Save(param1: 23978)."));
    }
}

public class OverridingObject
{
    public override bool Equals(object? obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public override string? ToString() => base.ToString();
}
