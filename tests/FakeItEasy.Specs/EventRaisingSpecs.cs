namespace FakeItEasy.Specs
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public delegate void CustomEventHandler(object sender, CustomEventArgs e);

    public delegate void ReferenceTypeEventHandler(ReferenceType arg);

    public delegate void ValueTypeEventHandler(int arg);

    public static class EventRaisingSpecs
    {
        private static readonly EventArgs SampleEventArgs = new EventArgs();

        private static readonly CustomEventArgs SampleCustomEventArgs = new CustomEventArgs();

        private static readonly ReferenceType SampleReferenceTypeEventArgs = new ReferenceType();

        private static readonly DerivedReferenceType SampleDerivedReferenceTypeEventArgs = new DerivedReferenceType();

        public interface IEvents
        {
            event EventHandler UnsubscribedEvent;

            event EventHandler SubscribedEvent;

            event EventHandler<CustomEventArgs> GenericEvent;

#if !FEATURE_EVENT_ARGS_MUST_EXTEND_EVENTARGS
            event EventHandler<ReferenceType> EventHandlerOfNonEventArgsEvent;
#endif

            event CustomEventHandler CustomEvent;

            event ReferenceTypeEventHandler ReferenceTypeEvent;

            event ValueTypeEventHandler ValueTypeEvent;

            [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Test.")]
            event Action<int, bool> ActionEvent;
        }

        private static IEvents Fake { get; set; }

        private static object SampleSender { get; set; }

        private static object CapturedSender { get; set; }

        private static object CapturedArgs1 { get; set; }

        private static object CapturedArgs2 { get; set; }

        [Background]
        public static void Background()
        {
            Fake = A.Fake<IEvents>();

            SampleSender = new object();

            Fake.SubscribedEvent += (sender, e) =>
            {
                CapturedSender = sender;
                CapturedArgs1 = e;
            };

#if !FEATURE_EVENT_ARGS_MUST_EXTEND_EVENTARGS
            Fake.EventHandlerOfNonEventArgsEvent += (sender, e) =>
            {
                CapturedSender = sender;
                CapturedArgs1 = e;
            };
#endif

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
        public static void UnsubscribedEvent(
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
        public static void WithEmpty()
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
        public static void EventArguments()
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "When I raise the event specifying only the event arguments"
                .x(() => Fake.SubscribedEvent += Raise.With(SampleEventArgs));

            "Then the fake is passed as the sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleEventArgs));
        }

        [Scenario]
        public static void SenderAndEventArguments()
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "When I raise the event specifying the sender and the event arguments"
                .x(() => Fake.SubscribedEvent += Raise.With(SampleSender, SampleEventArgs));

            "Then the supplied sender is passed as the event sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleEventArgs));
        }

        [Scenario]
        public static void NullSenderAndEventArguments()
        {
            "Given an event of type EventHandler"
                .See(() => nameof(Fake.SubscribedEvent));

            "When I raise the event specifying the event arguments and a null sender"
                .x(() => Fake.SubscribedEvent += Raise.With(null, SampleEventArgs));

            "Then null is passed as the event sender"
                .x(() => CapturedSender.Should().BeNull());

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleEventArgs));
        }

        [Scenario]
        public static void MultipleSubscribers(
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
        public static void CustomEventArguments()
        {
            "Given an event of type EventHandler with custom event arguments type"
                .See(() => nameof(Fake.GenericEvent));

            "When I raise the event specifying only the event arguments"
                .x(() => Fake.GenericEvent += Raise.With(SampleCustomEventArgs));

            "Then the fake is passed as the sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleCustomEventArgs));
        }

        [Scenario]
        public static void SenderAndCustomEventArguments()
        {
            "Given an event of type EventHandler with custom event arguments type"
                .See(() => nameof(Fake.GenericEvent));

            "When I raise the event specifying the sender and the event arguments"
                .x(() => Fake.GenericEvent += Raise.With(SampleSender, SampleCustomEventArgs));

            "Then the supplied sender is passed as the event sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleCustomEventArgs));
        }

        [Scenario]
        public static void NullSenderAndCustomEventArguments()
        {
            "Given an event of type EventHandler with custom event arguments type"
                .See(() => nameof(Fake.GenericEvent));

            "When I raise the event specifying the event arguments and a null sender"
                .x(() => Fake.GenericEvent += Raise.With(null, SampleCustomEventArgs));

            "Then null is passed as the event sender"
                .x(() => CapturedSender.Should().BeNull());

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleCustomEventArgs));
        }

#if !FEATURE_EVENT_ARGS_MUST_EXTEND_EVENTARGS
        [Scenario]
        public static void EventArgumentsThatDoNotExtenedEventArg()
        {
            "Given an event of type EventHandler with custom event arguments type that does not extend EventArgs"
                .See(() => nameof(Fake.EventHandlerOfNonEventArgsEvent));

            "When I raise the event specifying only the event arguments"
                .x(() => Fake.EventHandlerOfNonEventArgsEvent += Raise.With(SampleReferenceTypeEventArgs));

            "Then the fake is passed as the sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleReferenceTypeEventArgs));
        }
#endif

        [Scenario]
        public static void CustomEventHandler()
        {
            "Given an event of a custom delegate type that takes a sender and event arguments"
                .See(() => nameof(Fake.CustomEvent));

            "When I raise the event specifying the sender and the event arguments"
                .x(() => Fake.CustomEvent += Raise.With<CustomEventHandler>(SampleSender, SampleCustomEventArgs));

            "Then the supplied sender is passed as the event sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "And the supplied value is passed as the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleCustomEventArgs));
        }

        [Scenario]
        public static void ReferenceTypeEventHandler()
        {
            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event specifying the arguments"
                .x(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(SampleReferenceTypeEventArgs));

            "Then the supplied value is passed as the event argument"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleReferenceTypeEventArgs));
        }

        [Scenario]
        public static void ReferenceTypeEventHandlerWithDerivedArguments()
        {
            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event specifying an argument of a derived type"
                .x(() => Fake.ReferenceTypeEvent +=
                    Raise.With<ReferenceTypeEventHandler>(SampleDerivedReferenceTypeEventArgs));

            "Then the supplied value is passed as the event argument"
                .x(() => CapturedArgs1.Should().BeSameAs(SampleDerivedReferenceTypeEventArgs));
        }

        [Scenario]
        public static void ReferenceTypeEventHandlerWithInvalidArgumentType(
            Exception exception)
        {
            const string ExpectedMessage =
                "The event has the signature (FakeItEasy.Specs.ReferenceType), " +
                "but the provided arguments have types (System.Collections.Hashtable).";

            "Given an event of a custom delegate type taking only a reference type as an argument"
                .See(() => nameof(Fake.ReferenceTypeEvent));

            "When I raise the event specifying an argument of an invalid type"
                .x(() => exception = Record.Exception(() =>
                        Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(new Hashtable())));

            "Then the call fails with a meaningful message"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                    .Message.Should().Be(ExpectedMessage));
        }

        [Scenario]
        public static void ValueTypeEventHandlerWithNullValue(
            Exception exception)
        {
            "Given an event of a custom delegate type taking only a value type as an argument"
                .See(() => nameof(Fake.ValueTypeEvent));

            "When I raise the event specifying a null argument"
                .x(() => exception = Record.Exception(() =>
                        Fake.ValueTypeEvent += Raise.With<ValueTypeEventHandler>((object)null)));

            "Then the call fails with a meaningful message"
                .x(() => exception.Should().BeAnExceptionOfType<FakeConfigurationException>()
                    .And.Message.Should().Be(
                        "The event has the signature (System.Int32), but the provided arguments have types (<NULL>)."));
        }

        [Scenario]
        public static void ActionEvent()
        {
            "Given an event of type Action"
                .See(() => nameof(Fake.ActionEvent));

            "When I raise the event specifying the arguments"
                .x(() => Fake.ActionEvent += Raise.With<Action<int, bool>>(19, true));

            "Then the first value is passed as the first event argument"
                .x(() => CapturedArgs1.Should().Be(19));

            "Then the second value is passed as the second event argument"
                .x(() => CapturedArgs2.Should().Be(true));
        }
    }

    public class CustomEventArgs : EventArgs
    {
    }

    public class ReferenceType
    {
    }

    public class DerivedReferenceType : ReferenceType
    {
    }
}
