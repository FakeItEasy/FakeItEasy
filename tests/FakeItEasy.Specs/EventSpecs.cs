namespace FakeItEasy.Specs
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using LambdaTale;
    using Xunit;

    public delegate void CustomEventHandler(object sender, CustomEventArgs e);

    public delegate void ReferenceTypeEventHandler(ReferenceType arg);

    public delegate void ValueTypeEventHandler(int arg);

    public class CustomEventArgs : EventArgs
    {
    }

    public class ReferenceType
    {
    }

    public class DerivedReferenceType : ReferenceType
    {
    }

    public class EventSpecs
    {
        private readonly EventArgs eventArgs = new EventArgs();

        private readonly CustomEventArgs customEventArgs = new CustomEventArgs();

        private readonly ReferenceType referenceTypeEventArgs = new ReferenceType();

        private readonly DerivedReferenceType derivedReferenceTypeEventArgs = new DerivedReferenceType();

        public interface IEvents
        {
            event EventHandler UnsubscribedEvent;

            event EventHandler SubscribedEvent;

            event EventHandler<CustomEventArgs> GenericEvent;

            event EventHandler<ReferenceType> EventHandlerOfNonEventArgsEvent;

            event CustomEventHandler CustomEvent;

            event ReferenceTypeEventHandler ReferenceTypeEvent;

            event ValueTypeEventHandler ValueTypeEvent;

            [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Test.")]
            event Action<int, bool> ActionEvent;
        }

        private static IEvents? fake;
        private static object? sampleSender;

        private static IEvents Fake { get => fake ?? throw new InvalidOperationException(nameof(Fake) + " is not initialized"); }

        private static object SampleSender { get => sampleSender ?? throw new InvalidOperationException(nameof(SampleSender) + " is not initialized"); }

        private static object? CapturedSender { get; set; }

        private static object? CapturedArgs1 { get; set; }

        private static object? CapturedArgs2 { get; set; }

        [Background]
        public void Background()
        {
            fake = A.Fake<IEvents>();

            sampleSender = new object();

            Fake.SubscribedEvent += (sender, e) =>
            {
                CapturedSender = sender;
                CapturedArgs1 = e;
            };

            Fake.EventHandlerOfNonEventArgsEvent += (sender, e) =>
            {
                CapturedSender = sender;
                CapturedArgs1 = e;
            };

            Fake.GenericEvent += (sender, e) =>
            {
                CapturedSender = sender;
                CapturedArgs1 = e;
            };

            Fake.CustomEvent += (sender, e) =>
            {
                CapturedSender = sender;
                CapturedArgs1 = e;
            };

            Fake.ReferenceTypeEvent += arg =>
            {
                CapturedArgs1 = arg;
            };

            Fake.ValueTypeEvent += arg =>
            {
                CapturedArgs1 = arg;
            };

            Fake.ActionEvent += (arg1, arg2) =>
            {
                CapturedArgs1 = arg1;
                CapturedArgs2 = arg2;
            };
        }

        [Scenario]
        public void UnsubscribedEvent(
            Exception exception)
        {
            "Given an event with no subscribers"
                .See(() => nameof(Fake.UnsubscribedEvent));

            "When I raise the event"
                .x(() => exception = Record.Exception(() => Fake.UnsubscribedEvent += Raise.WithEmpty()));

            "Then the call doesn't throw"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public void WithEmpty()
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "When I raise the event without specifying sender or arguments"
                .x(() => Fake.SubscribedEvent += Raise.WithEmpty());

            "Then the fake is passed as the sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "And an empty EventArgs is passed as the event arguments"
                .x(() => CapturedArgs1.Should().Be(EventArgs.Empty));
        }

        [Scenario]
        public void EventArguments()
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "When I raise the event specifying only the event arguments"
                .x(() => Fake.SubscribedEvent += Raise.With(this.eventArgs));

            "Then the fake is passed as the sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.eventArgs));
        }

        [Scenario]
        public void SenderAndEventArguments()
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "When I raise the event specifying the sender and the event arguments"
                .x(() => Fake.SubscribedEvent += Raise.With(SampleSender, this.eventArgs));

            "Then the supplied sender is passed as the event sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.eventArgs));
        }

        [Scenario]
        public void NullSenderAndEventArguments()
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "When I raise the event specifying the event arguments and a null sender"
                .x(() => Fake.SubscribedEvent += Raise.With(null, this.eventArgs));

            "Then null is passed as the event sender"
                .x(() => CapturedSender.Should().BeNull());

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.eventArgs));
        }

        [Scenario]
        public void MultipleSubscribers(
            int handler1InvocationCount,
            int handler2InvocationCount)
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "And the event has multiple subscribers"
                .x(() =>
                {
                    Fake.SubscribedEvent += (s, e) => handler1InvocationCount++;
                    Fake.SubscribedEvent += (s, e) => handler2InvocationCount++;
                });

            "When I raise the event"
                .x(() => Fake.SubscribedEvent += Raise.WithEmpty());

            "Then the first subscriber is invoked once"
                .x(() => handler1InvocationCount.Should().Be(1));

            "And the second subscriber is invoked once"
                .x(() => handler2InvocationCount.Should().Be(1));
        }

        [Scenario]
        public void CustomEventArguments()
        {
            "Given an event of type EventHandler with custom event arguments type"
                .See(() => nameof(Fake.GenericEvent));

            "When I raise the event specifying only the event arguments"
                .x(() => Fake.GenericEvent += Raise.With(this.customEventArgs));

            "Then the fake is passed as the sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void SenderAndCustomEventArguments()
        {
            "Given an event of type EventHandler with custom event arguments type"
                .See(() => nameof(Fake.GenericEvent));

            "When I raise the event specifying the sender and the event arguments"
                .x(() => Fake.GenericEvent += Raise.With(SampleSender, this.customEventArgs));

            "Then the supplied sender is passed as the event sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void NullSenderAndCustomEventArguments()
        {
            "Given an event of type EventHandler with custom event arguments type"
                .See(() => nameof(Fake.GenericEvent));

            "When I raise the event specifying the event arguments and a null sender"
                .x(() => Fake.GenericEvent += Raise.With(null, this.customEventArgs));

            "Then null is passed as the event sender"
                .x(() => CapturedSender.Should().BeNull());

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void EventArgumentsThatDoNotExtendEventArg()
        {
            "Given an event of type EventHandler with custom event arguments type that does not extend EventArgs"
                .See(() => nameof(Fake.EventHandlerOfNonEventArgsEvent));

            "When I raise the event specifying only the event arguments"
                .x(() => Fake.EventHandlerOfNonEventArgsEvent += Raise.With(this.referenceTypeEventArgs));

            "Then the fake is passed as the sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.referenceTypeEventArgs));
        }

        [Scenario]
        public void FreeformEventHandler()
        {
            "Given an event of a custom delegate type that takes a sender and event arguments"
                .See(() => nameof(Fake.CustomEvent));

            "When I raise the specifying the sender and the event arguments"
                .x(() => Fake.CustomEvent += Raise.FreeForm<CustomEventHandler>.With(SampleSender, this.customEventArgs));

            "Then the supplied sender is passed as the event sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void FreeformReferenceTypeEventHandler()
        {
            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event as freeform specifying the arguments"
                .x(() => Fake.ReferenceTypeEvent += Raise.FreeForm<ReferenceTypeEventHandler>.With(this.referenceTypeEventArgs));

            "Then the supplied value is passed as the event argument"
                .x(() => CapturedArgs1.Should().BeSameAs(this.referenceTypeEventArgs));
        }

        [Scenario]
        public void FreeformReferenceTypeEventHandlerWithDerivedArguments()
        {
            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event as freeform specifying an argument of a derived type"
                .x(() => Fake.ReferenceTypeEvent +=
                    Raise.FreeForm<ReferenceTypeEventHandler>.With(this.derivedReferenceTypeEventArgs));

            "Then the supplied value is passed as the event argument"
                .x(() => CapturedArgs1.Should().BeSameAs(this.derivedReferenceTypeEventArgs));
        }

        [Scenario]
        public void FreeformReferenceTypeEventHandlerWithInvalidArgumentType(
            Exception exception)
        {
            const string ExpectedMessage =
                "The event has the signature (FakeItEasy.Specs.ReferenceType), " +
                "but the provided arguments have types (System.Collections.Hashtable).";

            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event as freeform specifying an argument of an invalid type"
                .x(() => exception = Record.Exception(() =>
                        Fake.ReferenceTypeEvent += Raise.FreeForm<ReferenceTypeEventHandler>.With(new Hashtable())));

            "Then the call fails with a meaningful message"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                    .Message.Should().Be(ExpectedMessage));
        }

        [Scenario]
        public void FreeformValueTypeEventHandlerWithNullValue(
            Exception exception)
        {
            "Given an event of a custom delegate type taking only a value type as an argument"
                .See(() => nameof(Fake.ValueTypeEvent));

            "When I raise the event as freeform specifying a null argument"
                .x(() => exception = Record.Exception(() =>
                        Fake.ValueTypeEvent += Raise.FreeForm<ValueTypeEventHandler>.With((object?)null)));

            "Then the call fails with a meaningful message"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Be(
                        "The event has the signature (System.Int32), but the provided arguments have types (NULL)."));
        }

        [Scenario]
        public void FreeformActionEvent()
        {
            "Given an event of type Action"
                .See(() => nameof(Fake.ActionEvent));

            "When I raise the event as freeform specifying the arguments"
                .x(() => Fake.ActionEvent += Raise.FreeForm<Action<int, bool>>.With(19, true));

            "Then the first value is passed as the first event argument"
                .x(() => CapturedArgs1.Should().Be(19));

            "Then the second value is passed as the second event argument"
                .x(() => CapturedArgs2.Should().Be(true));
        }

        [Scenario]
        public void DynamicCustomEventHandler()
        {
            "Given an event of a custom delegate type that takes a sender and event arguments"
                .See(() => nameof(Fake.CustomEvent));

            "When I raise the event specifying the sender and the event arguments"
                .x(() => Fake.CustomEvent += Raise.FreeForm.With(SampleSender, this.customEventArgs));

            "Then the supplied sender is passed as the event sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void DynamicReferenceTypeEventHandler()
        {
            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event specifying the arguments"
                .x(() => Fake.ReferenceTypeEvent += Raise.FreeForm.With(this.referenceTypeEventArgs));

            "Then the supplied value is passed as the event argument"
                .x(() => CapturedArgs1.Should().BeSameAs(this.referenceTypeEventArgs));
        }

        [Scenario]
        public void DynamicReferenceTypeEventHandlerWithDerivedArguments()
        {
            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event specifying an argument of a derived type"
                .x(() => Fake.ReferenceTypeEvent +=
                    Raise.FreeForm.With(this.derivedReferenceTypeEventArgs));

            "Then the supplied value is passed as the event argument"
                .x(() => CapturedArgs1.Should().BeSameAs(this.derivedReferenceTypeEventArgs));
        }

        [Scenario]
        public void DynamicReferenceTypeEventHandlerWithInvalidArgumentType(
            Exception exception)
        {
            const string ExpectedMessage =
                "The event has the signature (FakeItEasy.Specs.ReferenceType), " +
                "but the provided arguments have types (System.Collections.Hashtable).";

            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event specifying an argument of an invalid type"
                .x(() => exception = Record.Exception(() =>
                    Fake.ReferenceTypeEvent += Raise.FreeForm.With(new Hashtable())));

            "Then the call fails with a meaningful message"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                    .Message.Should().Be(ExpectedMessage));
        }

        [Scenario]
        public void DynamicValueTypeEventHandlerWithNullValue(
            Exception exception)
        {
            "Given an event of a custom delegate type taking only a value type as an argument"
                .See(() => nameof(Fake.ValueTypeEvent));

            "When I raise the event specifying a null argument"
                .x(() => exception = Record.Exception(() =>
                    Fake.ValueTypeEvent += Raise.FreeForm.With((object?)null)));

            "Then the call fails with a meaningful message"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Be(
                        "The event has the signature (System.Int32), but the provided arguments have types (NULL)."));
        }

        [Scenario]
        public void DynamicActionEvent()
        {
            "Given an event of type Action"
                .See(() => nameof(Fake.ActionEvent));

            "When I raise the event specifying the arguments"
                .x(() => Fake.ActionEvent += Raise.FreeForm.With(19, true));

            "Then the first value is passed as the first event argument"
                .x(() => CapturedArgs1.Should().Be(19));

            "Then the second value is passed as the second event argument"
                .x(() => CapturedArgs2.Should().Be(true));
        }

        [Scenario]
        public void DynamicRaiseAssignmentToNonDelegate(string s, Exception exception)
        {
            "When I assign Raise.FreeForm.With to something that isn't a delegate"
                .x(() => exception = Record.Exception(() => s = Raise.FreeForm.With("foo", 42)));

            "Then it throws an InvalidCastException"
                .x(() => exception.Should().BeAnExceptionOfType<InvalidCastException>());

            "And the exception message describes the attempted cast"
                .x(() => exception.Message.Should().Be(
                    "Unable to cast object of type 'FakeItEasy.Core.DynamicRaiser' to type 'System.String'."));
        }

        [Scenario]
        public void DynamicRaiseWithWrongArguments(Exception exception)
        {
            "Given an event of a custom delegate type that takes a sender and event arguments"
                .See(() => nameof(Fake.CustomEvent));

            "When I raise the event specifying incorrect arguments"
                .x(() => exception = Record.Exception(() => Fake.CustomEvent += Raise.FreeForm.With("foo", 42, true)));

            "Then it throws a FakeConfigurationException"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>());

            "And the exception message describes the expected and actual arguments"
                .x(() => exception.Message.Should().Be(
                    "The event has the signature (System.Object, FakeItEasy.Specs.CustomEventArgs), but the provided arguments have types (System.String, System.Int32, System.Boolean)."));
        }

        [Scenario]
        public void RaisingInternalEvent(ClassWithInternalEventVisibleToDynamicProxy fake, EventHandler handler)
        {
            "Given a fake of a class with an internal event"
                .x(() => fake = A.Fake<ClassWithInternalEventVisibleToDynamicProxy>());

            "And a fake event handler"
                .x(() => handler = A.Fake<EventHandler>());

            "And the fake handler is subscribed to the fake's event"
                .x(() => fake.TheEvent += handler);

            "When I raise the event"
                .x(() => fake.TheEvent += Raise.WithEmpty());

            "Then the handler is called"
                .x(() => A.CallTo(handler).MustHaveHappened());
        }

        [Scenario]
        public void SubscribeWithNullHandler(IEvents fake, Exception? exception)
        {
            "Given a fake with an event"
                .x(() => fake = A.Fake<IEvents>());

            "When subscribing to the event with a null handler"
                .x(() => exception = Record.Exception(() => fake.UnsubscribedEvent += null));

            "Then it should not throw"
                .x(() => exception.Should().BeNull());
        }

        [Scenario]
        public void UnsubscribeWithNullHandler(IEvents fake, Exception? exception)
        {
            "Given a fake with an event"
                .x(() => fake = A.Fake<IEvents>());

            "When unsubscribing from the event with a null handler"
                .x(() => exception = Record.Exception(() => fake.UnsubscribedEvent -= null));

            "Then it should not throw"
                .x(() => exception.Should().BeNull());
        }
    }
}
