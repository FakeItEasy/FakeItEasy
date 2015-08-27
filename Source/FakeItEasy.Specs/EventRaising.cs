namespace FakeItEasy.Specs
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Xbehave;

    public class CustomEventArgs : EventArgs
    {
    }

    public class ReferenceType
    {
    }

    public class DerivedReferenceType : ReferenceType
    {
    }

    public class EventRaising
    {
        [Background]
        public void Background()
        {
            Fake = A.Fake<IEvents>();

            SampleSender = new object();

            Fake.SubscribedEvent += (sender, e) =>
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

        public delegate void CustomEventHandler(object sender, CustomEventArgs e);

        public delegate void ReferenceTypeEventHandler(ReferenceType arg);

        public delegate void ValueTypeEventHandler(int arg);

        public interface IEvents
        {
            event EventHandler UnsubscribedEvent;

            event EventHandler SubscribedEvent;

            event EventHandler<CustomEventArgs> GenericEvent;

            event CustomEventHandler CustomEvent;

            event ReferenceTypeEventHandler ReferenceTypeEvent;

            event ValueTypeEventHandler ValueTypeEvent;

            [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Test.")]
            event Action<int, bool> ActionEvent;
        }

        protected static IEvents Fake { get; private set; }

        protected static object SampleSender { get; set; }

        protected EventArgs EventArgs = new EventArgs();

        protected CustomEventArgs CustomEventArgs = new CustomEventArgs();

        private ReferenceType ReferenceTypeEventArgs = new ReferenceType();

        private DerivedReferenceType DerivedReferenceTypeEventArgs = new DerivedReferenceType();

        protected static object CapturedSender { get; private set; }

        protected static object CapturedArgs1 { get; private set; }

        protected static object CapturedArgs2 { get; private set; }

        protected static object CaughtException { get; private set; }

        protected static void CatchException(Action action)
        {
            CaughtException = Catch.Exception(action);
        }

        [Scenario]
        public void when_raising_unsubscribed_event(
            Exception caughtException)
        {
            "when raising unsubscribed event"
                .x(() => caughtException = Record.Exception(() => Fake.UnsubscribedEvent += Raise.WithEmpty()));

            "it should not throw"
                .x(() => caughtException.Should().BeNull());
        }

        [Scenario]
        public void when_raising_event_using_WithEmpty()
        {
            "when raising unsubscribed event"
                .x(() => Fake.SubscribedEvent += Raise.WithEmpty());

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass empty event arguments"
                .x(() => CapturedArgs1.Should().Be(EventArgs.Empty));
        }

        [Scenario]
        public void when_raising_event_passing_arguments()
        {
            "when raising unsubscribed event"
                .x(() => Fake.SubscribedEvent += Raise.With(this.EventArgs));

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.EventArgs));
        }

        [Scenario]
        public void when_raising_event_passing_sender_and_arguments()
        {
            "when raising unsubscribed event"
                .x(() => Fake.SubscribedEvent += Raise.With(SampleSender, this.EventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.EventArgs));
        }

        [Scenario]
        public void when_raising_event_passing_arguments_and_null_sender()
        {
            "when raising unsubscribed event"
                .x(() => Fake.SubscribedEvent += Raise.With(null, this.EventArgs));

            "it should pass null as the sender"
                .x(() => CapturedSender.Should().BeNull());

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.EventArgs));
        }

        [Scenario]
        public void when_raising_event_with_multiple_subscribers(
            int handler1InvocationCount,
            int handler2InvocationCount)
        {
            "establish"
                .x(() =>
                    {
                        Fake.SubscribedEvent += (s, e) => handler1InvocationCount++;
                        Fake.SubscribedEvent += (s, e) => handler2InvocationCount++;
                    });

            "when raising unsubscribed event"
                .x(() => Fake.SubscribedEvent += Raise.WithEmpty());

            "it should invoke the first handler once"
                .x(() => handler1InvocationCount.Should().Be(1));

            "it should invoke the second handler once"
                .x(() => handler2InvocationCount.Should().Be(1));
        }

        [Scenario]
        public void when_raising_generic_event_passing_arguments()
        {
            "when raising unsubscribed event"
                .x(() => Fake.GenericEvent += Raise.With(this.CustomEventArgs));

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void when_raising_generic_event_passing_sender_and_arguments()
        {
            "when raising unsubscribed event"
                .x(() => Fake.GenericEvent += Raise.With(SampleSender, this.CustomEventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void when_raising_generic_event_passing_arguments_and_null_sender()
        {
            "when raising unsubscribed event"
                .x(() => Fake.GenericEvent += Raise.With(null, this.CustomEventArgs));

            "it should pass null as the sender"
                .x(() => CapturedSender.Should().BeNull());

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void when_raising_custom_event_passing_sender_and_arguments()
        {
            "when raising unsubscribed event"
                .x(() => Fake.CustomEvent += Raise.With<CustomEventHandler>(SampleSender, this.CustomEventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void when_raising_reference_type_event_passing_arguments()
        {
            "when raising unsubscribed event"
                .x(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(this.ReferenceTypeEventArgs));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.ReferenceTypeEventArgs));
        }

        [Scenario]
        public void when_raising_reference_type_event_passing_derived_arguments()
        {
            "when raising unsubscribed event"
                .x(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(this.DerivedReferenceTypeEventArgs));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.DerivedReferenceTypeEventArgs));
        }

        [Scenario]
        public void when_raising_reference_type_event_passing_invalid_argument_type()
        {
            "when raising unsubscribed event"
                .x(() => CatchException(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(new Hashtable())));

            "it should fail with good message"
                .x(() =>
                    {
                        const string ExpectedMessage =
                    "The event has the signature (FakeItEasy.Specs.ReferenceType), " +
                    "but the provided arguments have types (System.Collections.Hashtable).";

                        CaughtException.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                        .Message.Should().Be(ExpectedMessage);
                    });
        }
   
        [Scenario]
        public void when_raising_value_type_event_passing_null_argument()
        {
            "when raising unsubscribed event"
                .x(() => CatchException(() =>
                                        Fake.ValueTypeEvent += Raise.With<ValueTypeEventHandler>((object)null)));

            "it should fail with good message"
                .x(() => CaughtException.Should().BeAnExceptionOfType<FakeConfigurationException>()
                             .And.Message.Should().Be(
                                 "The event has the signature (System.Int32), but the provided arguments have types (<NULL>)."));
        }
   
        [Scenario]
        public void when_raising_action_event_passing_arguments()
        {
            int eventArgs1 = 19;
            bool eventArgs2 = true;

            "when raising unsubscribed event"
                .x(() => Fake.ActionEvent += Raise.With<Action<int, bool>>(eventArgs1, eventArgs2));

            "it should pass the first argument"
                .x(() => CapturedArgs1.Should().Be(eventArgs1));

            "it should pass the second argument"
                .x(() => CapturedArgs2.Should().Be(eventArgs2));
        }
    }
}
