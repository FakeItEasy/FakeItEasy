namespace FakeItEasy.Specs;

using System;
using FakeItEasy.Core;
using FakeItEasy.Tests.TestHelpers;
using FluentAssertions;
using Xbehave;
using Xunit;

public static class ResetFakeSpecs
{
    public interface IFoo
    {
        int AMethod();

        int AValueTypeProperty { get; set; }

        object AReferenceTypeProperty { get; set; }

        event Action AnEvent;
    }

    [Scenario]
    public static void MethodNotConfiguredAtCreationTime(IFoo fake, int result)
    {
        "Given a fake with a method not configured at creation time"
            .x(() => fake = A.Fake<IFoo>());

        "And the method is configured after creation"
            .x(() => A.CallTo(() => fake.AMethod()).Returns(123));

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the method is called"
            .x(() => result = fake.AMethod());

        "Then it returns the default value"
            .x(() => result.Should().Be(0));
    }

    [Scenario]
    public static void MethodConfiguredAtCreationTime(IFoo fake, int result)
    {
        "Given a fake with a method configured at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f =>
                {
                    A.CallTo(() => f.AMethod()).Returns(42);
                });
            }));

        "And the method is configured differently after creation"
            .x(() => A.CallTo(() => fake.AMethod()).Returns(123));

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the method is called"
            .x(() => result = fake.AMethod());

        "Then it returns the value configured at creation time"
            .x(() => result.Should().Be(42));
    }

    [Scenario]
    public static void MethodConfiguredOnceAtCreationTimeAndCalledAfterCreation(IFoo fake, int result)
    {
        "Given a fake with a method configured for a single call at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f =>
                {
                    A.CallTo(() => f.AMethod()).Returns(42).Once();
                });
            }));

        "And the method is called once after creation"
            .x(() => fake.AMethod());

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the method is called"
            .x(() => result = fake.AMethod());

        "Then it returns the value configured at creation time"
            .x(() => result.Should().Be(42));
    }

    [Scenario]
    public static void MethodConfiguredOnceAndCalledAtCreationTime(IFoo fake, int result)
    {
        "Given a fake with a method configured for a single call then called at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f =>
                {
                    A.CallTo(() => f.AMethod()).Returns(42).Once();
                    f.AMethod();
                });
            }));

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the method is called"
            .x(() => result = fake.AMethod());

        "Then it returns the default value"
            .x(() => result.Should().Be(0));
    }

    [Scenario]
    public static void ValueTypePropertyNotSetAtCreationTime(IFoo fake, int result)
    {
        "Given a fake with a value-type property not set at creation time"
            .x(() => fake = A.Fake<IFoo>());

        "And the property is set"
            .x(() => fake.AValueTypeProperty = 123);

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the property is called"
            .x(() => result = fake.AValueTypeProperty);

        "Then it returns the default value"
            .x(() => result.Should().Be(0));
    }

    [Scenario]
    public static void ValueTypePropertySetAtCreationTime(IFoo fake, int result)
    {
        "Given a fake with a value-type property set at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f => f.AValueTypeProperty = 42);
            }));

        "And the property is set differently after creation"
            .x(() => fake.AValueTypeProperty = 123);

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the property is called"
            .x(() => result = fake.AValueTypeProperty);

        "Then it returns the value set at creation time"
            .x(() => result.Should().Be(42));
    }

    [Scenario]
    public static void ReferenceTypePropertyNotSetAtCreationTime(IFoo fake, object result)
    {
        "Given a fake with a reference-type property not set at creation time"
            .x(() => fake = A.Fake<IFoo>());

        "And the property is set to a given value after creation"
            .x(() => fake.AReferenceTypeProperty = "hello");

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the property is called"
            .x(() => result = fake.AReferenceTypeProperty);

        "Then it returns a dummy object"
            .x(() => result.Should().BeAFake());
    }

    [Scenario]
    public static void ReferenceTypePropertySetAtCreationTime(IFoo fake, object result)
    {
        "Given a fake with a reference-type property set at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f => f.AReferenceTypeProperty = "hello");
            }));

        "And the property is set differently after creation"
            .x(() => fake.AReferenceTypeProperty = "world");

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the property is called"
            .x(() => result = fake.AReferenceTypeProperty);

        "Then it returns the value set at creation time"
            .x(() => result.Should().Be("hello"));
    }

    [Scenario]
    public static void ReferenceTypePropertyReadAtCreationTime(IFoo fake, object initialValue, object result)
    {
        "Given a fake with a reference-type property read at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f => initialValue = f.AReferenceTypeProperty);
            }));

        "And the property is set to a given value after creation"
            .x(() => fake.AReferenceTypeProperty = "hello");

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the property is called"
            .x(() => result = fake.AReferenceTypeProperty);

        "Then it returns the value set at creation time"
            .x(() => result.Should().Be(initialValue));
    }

    [Scenario]
    public static void EventNotSubscribedAtCreationTime(IFoo fake, Action handler)
    {
        "Given a fake with an event not subscribed at creation time"
            .x(() => fake = A.Fake<IFoo>());

        "And an event handler"
            .x(() => handler = A.Fake<Action>());

        "And the event is subscribed to after creation"
            .x(() => fake.AnEvent += handler);

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the event is raised"
            .x(() => fake.AnEvent += Raise.FreeForm.With());

        "Then the handler is not invoked"
            .x(() => A.CallTo(handler).MustNotHaveHappened());
    }

    [Scenario]
    public static void EventSubscribedAtCreationTimeAndUnsubscribedAfterCreation(IFoo fake, Action handler)
    {
        "Given an event handler"
            .x(() => handler = A.Fake<Action>());

        "And a fake with an event subscribed at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f => f.AnEvent += handler);
            }));

        "And the event is unsubscribed after creation"
            .x(() => fake.AnEvent -= handler);

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the event is raised"
            .x(() => fake.AnEvent += Raise.FreeForm.With());

        "Then the handler is invoked"
            .x(() => A.CallTo(handler).MustHaveHappenedOnceExactly());
    }

    [Scenario]
    public static void EventSubscribedAtCreationTimeAndSubscribedAgainAfterCreation(IFoo fake, Action initialHandler, Action secondHandler)
    {
        "Given two event handlers"
            .x(() =>
            {
                initialHandler = A.Fake<Action>();
                secondHandler = A.Fake<Action>();
            });

        "And a fake with an event subscribed at creation time with the first handler"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f => f.AnEvent += initialHandler);
            }));

        "And the event is subscribed with the second handler after creation"
            .x(() => fake.AnEvent += secondHandler);

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the event is raised"
            .x(() => fake.AnEvent += Raise.FreeForm.With());

        "Then the initial handler is invoked"
            .x(() => A.CallTo(initialHandler).MustHaveHappenedOnceExactly());

        "And the second handler is not invoked"
            .x(() => A.CallTo(secondHandler).MustNotHaveHappened());
    }

    [Scenario]
    public static void RecordedCallsWhenNoCallMadeAtCreationTime(IFoo fake)
    {
        "Given a fake on which no call was made at creation time"
            .x(() => fake = A.Fake<IFoo>());

        "And a call is made after creation"
            .x(() => fake.AMethod());

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "Then there should be no recorded call on the fake"
            .x(() => A.CallTo(fake).MustNotHaveHappened());
    }

    [Scenario]
    public static void RecordedCallsWhenACallIsMadeAtCreationTime(IFoo fake)
    {
        "Given a fake on which a call was made at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f =>
                {
                    f.AMethod();
                });
            }));

        "And another call is made after creation"
            .x(() => fake.AMethod());

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "Then there should be only one recorded call on the fake"
            .x(() => A.CallTo(fake).MustHaveHappenedOnceExactly());
    }

    [Scenario]
    public static void InterceptionListenerAddedAfterCreationTime(IFoo fake, IInterceptionListener listener)
    {
        "Given an interception listener"
            .x(() => listener = A.Fake<IInterceptionListener>());

        "And a fake with no interception listener added at creation time"
            .x(() => fake = A.Fake<IFoo>());

        "And the interception listener is added after creation"
            .x(() => Fake.GetFakeManager(fake).AddInterceptionListener(listener));

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And a call is made on the fake"
            .x(() => fake.AMethod());

        "Then the interception listener is not called"
            .x(() => A.CallTo(listener).MustNotHaveHappened());
    }

    [Scenario]
    public static void InterceptionListenerAddedAtCreationTime(IFoo fake, IInterceptionListener listener)
    {
        "Given an interception listener"
            .x(() => listener = A.Fake<IInterceptionListener>());

        "And a fake with an interception listener added at creation time"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f =>
                {
                    Fake.GetFakeManager(f).AddInterceptionListener(listener);
                });
            }));

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And a call is made on the fake"
            .x(() => fake.AMethod());

        "Then the interception listener is called"
            .x(() => A.CallTo(() => listener.OnBeforeCallIntercepted(A<IFakeObjectCall>._)).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => listener.OnAfterCallIntercepted(A<ICompletedFakeObjectCall>._)).MustHaveHappenedOnceExactly()));
    }

    [Scenario]
    public static void StrictFake(IFoo fake, Exception exception)
    {
        "Given a strict fake"
            .x(() => fake = A.Fake<IFoo>(options => options.Strict()));

        "And a method is configured on the fake"
            .x(() => A.CallTo(() => fake.AMethod()).Returns(42));

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "And the same method is called on it"
            .x(() => exception = Record.Exception(() => fake.AMethod()));

        "Then it throws an ExpectationException"
            .x(() => exception.Should().BeOfType<ExpectationException>());
    }

    [Scenario]
    public static void StatefulUserRule(IFoo fake)
    {
        "Given a fake with a stateful user rule that is applied once during creation"
            .x(() => fake = A.Fake<IFoo>(options =>
            {
                options.ConfigureFake(f =>
                {
                    Fake.GetFakeManager(f).AddRuleFirst(new CallCounterRule());
                    f.AMethod();
                });
            }));

        "And another call is made on the fake after creation"
            .x(() => fake.AMethod());

        "When the fake is reset"
            .x(() => Fake.Reset(fake));

        "Then the stateful rule is reset to its initial state"
            .x(() => fake.AMethod().Should().Be(2));
    }

    private class CallCounterRule : IStatefulFakeObjectCallRule
    {
        private int numberOfTimesCalled;

        public int? NumberOfTimesToCall => null;

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return fakeObjectCall.Method.ReturnType == typeof(int);
        }

        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            fakeObjectCall.SetReturnValue(++this.numberOfTimesCalled);
        }

        public IFakeObjectCallRule GetSnapshot()
        {
            return new CallCounterRule
            {
                numberOfTimesCalled = this.numberOfTimesCalled
            };
        }
    }
}
