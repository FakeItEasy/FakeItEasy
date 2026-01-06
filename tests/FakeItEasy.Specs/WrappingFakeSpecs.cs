namespace FakeItEasy.Specs;

using System;
using FakeItEasy.Configuration;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using LambdaTale;
using Xunit;

public static class WrappingFakeSpecs
{
    public interface IFoo
    {
        int NonVoidMethod(string? parameter);

        void VoidMethod(string? parameter);

        void OutAndRefMethod(ref int @ref, out int @out);

        event EventHandler? SomeEvent;
    }

    public interface IBar
    {
        int Id { get; }
    }

    [Scenario]
    public static void NonVoidSuccess(
        Foo realObject,
        IFoo wrapper,
        int result)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When a non-void method is called on the wrapper"
            .x(() => result = wrapper.NonVoidMethod("hello"));

        "Then the real object's method is called"
            .x(() => realObject.NonVoidMethodCalled.Should().BeTrue());

        "And the wrapper returns the value returned by the real object's method"
            .x(() => result.Should().Be(5));
    }

    [Scenario]
    public static void NonVoidException(
        Foo realObject,
        IFoo wrapper,
        Exception exception)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When a non-void method is called on the wrapper"
            .x(() => exception = Record.Exception(() => wrapper.NonVoidMethod(null)));

        "Then the real object's method is called"
            .x(() => realObject.NonVoidMethodCalled.Should().BeTrue());

        "And the wrapper throws the exception thrown by the real object's method"
            .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>());

        "And the exception stack trace is preserved"
            .x(() => exception.StackTrace.Should().Contain("FakeItEasy.Specs.WrappingFakeSpecs.Foo.NonVoidMethod"));
    }

    [Scenario]
    public static void VoidSuccess(
        Foo realObject,
        IFoo wrapper)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When a void method is called on the wrapper"
            .x(() => wrapper.VoidMethod("hello"));

        "Then the real object's method is called"
            .x(() => realObject.VoidMethodCalled.Should().BeTrue());
    }

    [Scenario]
    public static void VoidException(
        Foo realObject,
        IFoo wrapper,
        Exception exception)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When a void method is called on the wrapper"
            .x(() => exception = Record.Exception(() => wrapper.VoidMethod(null)));

        "Then the real object's method is called"
            .x(() => realObject.VoidMethodCalled.Should().BeTrue());

        "And the wrapper throws the exception thrown by the real object's method"
            .x(() => exception.Should().BeAnExceptionOfType<ArgumentNullException>());

        "And the exception stack trace is preserved"
            .x(() => exception.StackTrace.Should().Contain("FakeItEasy.Specs.WrappingFakeSpecs.Foo.VoidMethod"));
    }

    [Scenario]
    public static void OutAndRef(
        Foo realObject,
        IFoo wrapper,
        int @ref,
        int @out)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "And a @ref variable with an initial value of 1"
            .x(() => @ref = 1);

        "When a method with out and ref parameters is called on the wrapper"
            .x(() => wrapper.OutAndRefMethod(ref @ref, out @out));

        "Then the real object's method is called"
            .x(() => realObject.OutAndRefMethodCalled.Should().BeTrue());

        "And the value of @ref is incremented"
            .x(() => @ref.Should().Be(2));

        "And the value of @out is set"
            .x(() => @out.Should().Be(42));
    }

    [Scenario]
    public static void FakeEqualsFake(Foo realObject, IFoo wrapper, bool equals)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When Equals is called on the fake with itself as the argument"
            .x(() => equals = wrapper.Equals(wrapper));

        "Then it should return true"
            .x(() => equals.Should().BeTrue());
    }

    [Scenario]
    public static void FakeEqualsWrappedObject(Foo realObject, IFoo wrapper, bool equals)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When Equals is called on the fake with the real object as the argument"
            .x(() => equals = wrapper.Equals(realObject));

        "Then it should return false"
            .x(() => equals.Should().BeFalse());
    }

    [Scenario]
    public static void FakeEqualsFakeWithValueSemantics(Bar realObject, IBar wrapper, bool equals)
    {
        "Given a real object that overrides Equals with value semantics"
            .x(() => realObject = new Bar(42));

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IBar>(o => o.Wrapping(realObject)));

        "When Equals is called on the fake with itself as the argument"
            .x(() => equals = wrapper.Equals(wrapper));

        "Then it should return true"
            .x(() => equals.Should().BeTrue());
    }

    [Scenario]
    public static void FakeEqualsWrappedObjectWithValueSemantics(Bar realObject, IBar wrapper, bool equals)
    {
        "Given a real object that overrides Equals with value semantics"
            .x(() => realObject = new Bar(42));

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IBar>(o => o.Wrapping(realObject)));

        "When Equals is called on the fake with the real object as the argument"
            .x(() => equals = wrapper.Equals(realObject));

        "Then it should return true"
            .x(() => equals.Should().BeTrue());
    }

    [Scenario]
    public static void FakeClassEqualsWrappedObjectWithOverriddenReferenceSemantics(
        ClassThatOverrideEqualsForNoReason realObject,
        ClassThatOverrideEqualsForNoReason wrapper,
        bool equals)
    {
        "Given a real object that overrides Equals but keeps reference semantics"
            .x(() => realObject = new ClassThatOverrideEqualsForNoReason());

        "And a faked class of the same type that wraps the real object"
            .x(() => wrapper = A.Fake<ClassThatOverrideEqualsForNoReason>(o => o.Wrapping(realObject)));

        "When Equals is called on the fake with itself as the argument"
            .x(() => equals = wrapper.Equals(wrapper));

        "Then it should return true"
            .x(() => equals.Should().BeTrue());
    }

    [Scenario]
    public static void FakeObjectEqualsWrappedObjectWithOverriddenReferenceSemantics(
        ClassThatOverrideEqualsForNoReason realObject,
        object wrapper,
        bool equals)
    {
        "Given a real object that overrides Equals but keeps reference semantics"
            .x(() => realObject = new ClassThatOverrideEqualsForNoReason());

        "And a faked Object type that wraps the real object"
            .x(() => wrapper = A.Fake<object>(o => o.Wrapping(realObject)));

        "When Equals is called on the fake with itself as the argument"
            .x(() => equals = wrapper.Equals(wrapper));

        "Then it should return true"
            .x(() => equals.Should().BeTrue());
    }

    [Scenario]
    public static void VoidCallsWrappedMethod(Foo realObject, IFoo wrapper, bool callbackWasCalled)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When a void method on the fake is configured to invoke a callback and call the wrapped method"
            .x(() => A.CallTo(() => wrapper.VoidMethod("hello"))
                .Invokes(() => callbackWasCalled = true)
                .CallsWrappedMethod());

        "And the configured method is called on the fake"
            .x(() => wrapper.VoidMethod("hello"));

        "Then the callback is invoked"
            .x(() => callbackWasCalled.Should().BeTrue());

        "And the real object's method is called"
            .x(() => realObject.VoidMethodCalled.Should().BeTrue());
    }

    [Scenario]
    public static void NonVoidCallsWrappedMethod(Foo realObject, IFoo wrapper, bool callbackWasCalled, int returnValue)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "When a non-void method on the fake is configured to invoke a callback and call the wrapped method"
            .x(() => A.CallTo(() => wrapper.NonVoidMethod("hello"))
                .Invokes(() => callbackWasCalled = true)
                .CallsWrappedMethod());

        "And the configured method is called on the fake"
            .x(() => returnValue = wrapper.NonVoidMethod("hello"));

        "Then the callback is invoked"
            .x(() => callbackWasCalled.Should().BeTrue());

        "And the real object's method is called"
            .x(() => realObject.NonVoidMethodCalled.Should().BeTrue());

        "And the wrapper returns the value returned by the real object's method"
            .x(() => returnValue.Should().Be(5));
    }

    [Scenario]
    public static void CallWrappedMethodOnStrictWrappingFake(Foo realObject, IFoo wrapper, int returnValue)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a strict fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject).Strict()));

        "When a method on the fake is configured to call the wrapped method"
            .x(() => A.CallTo(() => wrapper.NonVoidMethod("hello"))
                .CallsWrappedMethod());

        "And the configured method is called on the fake"
            .x(() => returnValue = wrapper.NonVoidMethod("hello"));

        "Then the real object's method is called"
            .x(() => realObject.NonVoidMethodCalled.Should().BeTrue());

        "And the wrapper returns the value returned by the real object's method"
            .x(() => returnValue.Should().Be(5));
    }

    [Scenario]
    public static void NotAWrappingFakeCallsWrappedMethod(IFoo fake, Exception exception)
    {
        "Given a non-wrapping fake"
            .x(() => fake = A.Fake<IFoo>());

        "When a method on the fake is configured to call the wrapped method"
            .x(() => exception = Record.Exception(() => A.CallTo(() => fake.VoidMethod("hello")).CallsWrappedMethod()));

        "Then it throws a FakeConfigurationException"
            .x(() => exception.Should()
                .BeAnExceptionOfType<FakeConfigurationException>()
                .WithMessage("The configured fake is not a wrapping fake."));
    }

    [Scenario]
    public static void EventSubscriptionSubscribesToWrappedEvent(Foo realObject, IFoo wrapper, EventHandler handler)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "And an event handler"
            .x(() => handler = A.Fake<EventHandler>());

        "When the handler is subscribed to the fake's event"
            .x(() => wrapper.SomeEvent += handler);

        "And the event is raised using a method on the wrapped object"
            .x(() => realObject.RaiseSomeEvent());

        "Then the handler is called"
            .x(() => A.CallTo(handler).MustHaveHappened());
    }

    [Scenario]
    public static void RaisingEventOnWrappingFakeUsingRaiseSyntax(Foo realObject, IFoo wrapper, EventHandler handler, Exception exception)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "And an event handler"
            .x(() => handler = A.Fake<EventHandler>());

        "When the event is raised on the fake using the '+= Raise' syntax"
            .x(() => exception = Record.Exception(() => wrapper.SomeEvent += Raise.WithEmpty()));

        "Then an InvalidOperationException is thrown"
            .x(() => exception.Should()
                .BeAnExceptionOfType<InvalidOperationException>()
                .WithMessage("*The fake cannot raise the event because it's a wrapping fake*"));
    }

    [Scenario]
    public static void RaisingEventOnWrappingFakeWorkaround(Foo realObject, IFoo wrapper, EventHandler? handlers, EventHandler individualHandler)
    {
        "Given a real object"
            .x(() => realObject = new Foo());

        "And a fake wrapping this object"
            .x(() => wrapper = A.Fake<IFoo>(o => o.Wrapping(realObject)));

        "And an event handler"
            .x(() => individualHandler = A.Fake<EventHandler>());

        "When the fake is configured to handle event subscriptions manually"
            .x(() =>
            {
                A.CallTo(wrapper).Where(call => call.Method.Name == "add_SomeEvent").Invokes((EventHandler h) => handlers += h);
                A.CallTo(wrapper).Where(call => call.Method.Name == "remove_SomeEvent").Invokes((EventHandler h) => handlers -= h);
            });

        "And the handler is subscribed to the event"
            .x(() => wrapper.SomeEvent += individualHandler);

        "And the event is raised"
            .x(() => handlers?.Invoke(wrapper, EventArgs.Empty));

        "Then the event handler is called"
            .x(() => A.CallTo(individualHandler).MustHaveHappened());
    }

    public class Foo : IFoo
    {
        public bool NonVoidMethodCalled { get; private set; }

        public bool VoidMethodCalled { get; private set; }

        public bool OutAndRefMethodCalled { get; private set; }

        public int NonVoidMethod(string? parameter)
        {
            this.NonVoidMethodCalled = true;
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            return parameter.Length;
        }

        public void VoidMethod(string? parameter)
        {
            this.VoidMethodCalled = true;
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
        }

        public void OutAndRefMethod(ref int @ref, out int @out)
        {
            this.OutAndRefMethodCalled = true;
            @ref += 1;
            @out = 42;
        }

        public event EventHandler? SomeEvent;

#pragma warning disable CA1030 // Use events where appropriate
        public void RaiseSomeEvent() => this.SomeEvent?.Invoke(this, EventArgs.Empty);
#pragma warning restore CA1030 // Use events where appropriate
    }

    public class Bar : IBar
    {
        public Bar(int id)
        {
            this.Id = id;
        }

        public int Id { get; }

        public override bool Equals(object? obj)
        {
            return obj is IBar other && other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }

    public class ClassThatOverrideEqualsForNoReason
    {
        public override bool Equals(object? obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}
