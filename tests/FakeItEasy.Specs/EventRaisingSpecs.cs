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

    public class CustomEventArgs : EventArgs
    {
    }

    public class ReferenceType
    {
    }

    public class DerivedReferenceType : ReferenceType
    {
    }

    public class EventRaisingSpecs
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

        private static object CaughtException { get; set; }

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
            "when raising event with empty arguments"
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
                .x(() => Fake.SubscribedEvent += Raise.With(this.eventArgs));

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.eventArgs));
        }

        [Scenario]
        public void SenderAndEventArguments()
        {
            "when raising event passing sender and arguments"
                .x(() => Fake.SubscribedEvent += Raise.With(SampleSender, this.eventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.eventArgs));
        }

        [Scenario]
        public void NullSenderAndEventArguments()
        {
            "when raising event passing arguments and null sender"
                .x(() => Fake.SubscribedEvent += Raise.With(null, this.eventArgs));

            "it should pass null as the sender"
                .x(() => CapturedSender.Should().BeNull());

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.eventArgs));
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
                .x(() => Fake.GenericEvent += Raise.With(this.customEventArgs));

            "it should pass the fake as sender"
                .x(() => CapturedSender.Should().BeSameAs(Fake));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void SenderAndCustomEventArguments()
        {
            "when raising generic event passing sender and arguments"
                .x(() => Fake.GenericEvent += Raise.With(SampleSender, this.customEventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void NullSenderAndCustomEventArguments()
        {
            "when raising generic event passing arguments and null sender"
                .x(() => Fake.GenericEvent += Raise.With(null, this.customEventArgs));

            "it should pass null as the sender"
                .x(() => CapturedSender.Should().BeNull());

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void CustomEventHandler()
        {
            "when raising custom event passing sender and arguments"
                .x(() => Fake.CustomEvent += Raise.With<CustomEventHandler>(SampleSender, this.customEventArgs));

            "it should pass the sender"
                .x(() => CapturedSender.Should().BeSameAs(SampleSender));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.customEventArgs));
        }

        [Scenario]
        public void ReferenceTypeEventHandler()
        {
            "when raising reference type event passing arguments"
                .x(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(this.referenceTypeEventArgs));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.referenceTypeEventArgs));
        }

        [Scenario]
        public void ReferenceTypeEventHandlerWithDerivedArguments()
        {
            "when raising reference type event passing derived arguments"
                .x(() => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(this.derivedReferenceTypeEventArgs));

            "it should pass the event arguments"
                .x(() => CapturedArgs1.Should().BeSameAs(this.derivedReferenceTypeEventArgs));
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
            var eventArgs1 = 19;
            var eventArgs2 = true;

            "when raising action event passing arguments"
                .x(() => Fake.ActionEvent += Raise.With<Action<int, bool>>(eventArgs1, eventArgs2));

            "it should pass the first argument"
                .x(() => CapturedArgs1.Should().Be(eventArgs1));

            "it should pass the second argument"
                .x(() => CapturedArgs2.Should().Be(eventArgs2));
        }

        private static void CatchException(Action action)
        {
            CaughtException = Record.Exception(action);
        }
    }
}
