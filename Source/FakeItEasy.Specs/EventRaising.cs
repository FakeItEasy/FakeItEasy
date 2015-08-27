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

    public delegate void CustomEventHandler(object sender, CustomEventArgs e);

    public delegate void ReferenceTypeEventHandler(ReferenceType arg);

    public delegate void ValueTypeEventHandler(int arg);

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
        public void UnsubscribedEvent(
            Exception caughtException)
        {
            "when raising unsubscribed event"
                .x(() => caughtException = Record.Exception(() => Fake.UnsubscribedEvent += Raise.WithEmpty()));

            "it should not throw"
                .x(() => caughtException.Should().BeNull());
        }

        [Scenario]
        public void WithEmpty()
        {
            "when raising event using WithEmpty"
                .x(() => Fake.SubscribedEvent += Raise.WithEmpty());

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass empty event arguments"
                .x(() => CapturedArgs1.Should().Be(EventArgs.Empty));
        }

        [Scenario]
        public void EventArguments()
        {
            "when raising event passing arguments"
                .x(() => Fake.SubscribedEvent += Raise.With(this.EventArgs));

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.EventArgs));
        }

        [Scenario]
        public void SenderAndEventArguments()
        {
            "when raising event passing sender and arguments"
                .x(() => Fake.SubscribedEvent += Raise.With(SampleSender, this.EventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.EventArgs));
        }

        [Scenario]
        public void NullSenderAndEventArguments()
        {
            "when raising event passing arguments and null sender"
                .x(() => Fake.SubscribedEvent += Raise.With(null, this.EventArgs));

            "it should pass null as the sender"
                .x(() => CapturedSender.Should().BeNull());

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.EventArgs));
        }

        [Scenario]
        public void MultipleSubscribers(
            int handler1InvocationCount,
            int handler2InvocationCount)
        {
            "establish"
                .x(() =>
                    {
                        Fake.SubscribedEvent += (s, e) => handler1InvocationCount++;
                        Fake.SubscribedEvent += (s, e) => handler2InvocationCount++;
                    });

            "when raising event with multiple subscribers"
                .x(() => Fake.SubscribedEvent += Raise.WithEmpty());

            "it should invoke the first handler once"
                .x(() => handler1InvocationCount.Should().Be(1));

            "it should invoke the second handler once"
                .x(() => handler2InvocationCount.Should().Be(1));
        }

        [Scenario]
        public void CustomEventArguments()
        {
            "when raising generic event passing arguments"
                .x(() => Fake.GenericEvent += Raise.With(this.CustomEventArgs));

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void SenderAndCustomEventArguments()
        {
            "when raising generic event passing sender and arguments"
                .x(() => Fake.GenericEvent += Raise.With(SampleSender, this.CustomEventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void NullSenderAndCustomEventArguments()
        {
            "when raising generic event passing arguments and null sender"
                .x(() => Fake.GenericEvent += Raise.With(null, this.CustomEventArgs));

            "it should pass null as the sender"
                .x(() => CapturedSender.Should().BeNull());

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void CustomEventHandler()
        {
            "when raising custom event passing sender and arguments"
                .x(() => Fake.CustomEvent += Raise.With<CustomEventHandler>(SampleSender, this.CustomEventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.CustomEventArgs));
        }

        [Scenario]
        public void ReferenceTypeEventHandler()
        {
            "when raising reference type event passing arguments"
                .x(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(this.ReferenceTypeEventArgs));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.ReferenceTypeEventArgs));
        }

        [Scenario]
        public void ReferenceTypeEventHandlerWithDerivedArguments()
        {
            "when raising reference type event passing derived arguments"
                .x(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(this.DerivedReferenceTypeEventArgs));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.DerivedReferenceTypeEventArgs));
        }

        [Scenario]
        public void ReferenceTypeEventHandlerWithInvalidArgumentType()
        {
            "when raising reference type event passing invalid argument type"
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
        public void ValueTypeEventHandlerWithNullValue()
        {
            "when raising value type event passing null argument"
                .x(() => CatchException(() =>
                                        Fake.ValueTypeEvent += Raise.With<ValueTypeEventHandler>((object)null)));

            "it should fail with good message"
                .x(() => CaughtException.Should().BeAnExceptionOfType<FakeConfigurationException>()
                             .And.Message.Should().Be(
                                 "The event has the signature (System.Int32), but the provided arguments have types (<NULL>)."));
        }
   
        [Scenario]
        public void ActionEvent()
        {
            int eventArgs1 = 19;
            bool eventArgs2 = true;

            "when raising action event passing arguments"
                .x(() => Fake.ActionEvent += Raise.With<Action<int, bool>>(eventArgs1, eventArgs2));

            "it should pass the first argument"
                .x(() => CapturedArgs1.Should().Be(eventArgs1));

            "it should pass the second argument"
                .x(() => CapturedArgs2.Should().Be(eventArgs2));
        }
    }
}
