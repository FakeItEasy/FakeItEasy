namespace FakeItEasy.Specs
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public abstract class EventRaisingSpecs : EventRaisingSpecs<object>
    {
    }

    public abstract class EventRaisingSpecs<TArgs> : EventRaisingSpecs<TArgs, object> where TArgs : new()
    {
    }

    public abstract class EventRaisingSpecs<TArgs1, TArgs2>
        where TArgs1 : new()
        where TArgs2 : new()
    {
        Establish context = () =>
        {
            Fake = A.Fake<IEvents>();

            SampleSender = new object();
            SampleEventArgs1 = new TArgs1();
            SampleEventArgs2 = new TArgs2();

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
        };

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

        protected static TArgs1 SampleEventArgs1 { get; set; }

        protected static TArgs2 SampleEventArgs2 { get; set; }

        protected static object CapturedSender { get; private set; }

        protected static object CapturedArgs1 { get; private set; }

        protected static object CapturedArgs2 { get; private set; }

        protected static object CaughtException { get; private set; }

        protected static void CatchException(Action action)
        {
            CaughtException = Catch.Exception(action);
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

    public class when_raising_unsubscribed_event : EventRaisingSpecs
    {
        Because of = () => CatchException(() => Fake.UnsubscribedEvent += Raise.WithEmpty());

        It should_not_throw = () => CaughtException.Should().BeNull();
    }

    public class when_raising_event_using_WithEmpty : EventRaisingSpecs
    {
        Because of = () => Fake.SubscribedEvent += Raise.WithEmpty();

        It should_pass_the_fake_as_sender = () => CapturedSender.Should().BeSameAs(Fake);

        It should_pass_empty_event_arguments = () => CapturedArgs1.Should().Be(EventArgs.Empty);
    }

    public class when_raising_event_passing_arguments : EventRaisingSpecs<EventArgs>
    {
        Because of = () => Fake.SubscribedEvent += Raise.With(SampleEventArgs1);

        It should_pass_the_fake_as_sender = () => CapturedSender.Should().BeSameAs(Fake);

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_event_passing_sender_and_arguments : EventRaisingSpecs<EventArgs>
    {
        Because of = () => Fake.SubscribedEvent += Raise.With(SampleSender, SampleEventArgs1);

        It should_pass_the_sender = () => CapturedSender.Should().BeSameAs(SampleSender);

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_event_passing_arguments_and_null_sender : EventRaisingSpecs<EventArgs>
    {
        Because of = () => Fake.SubscribedEvent += Raise.With(null, SampleEventArgs1);

        It should_pass_null_as_the_sender = () => CapturedSender.Should().BeNull();

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_event_with_multiple_subscribers : EventRaisingSpecs
    {
        static int handler1InvocationCount;
        static int handler2InvocationCount;

        Establish context = () =>
        {
            Fake.SubscribedEvent += (s, e) => handler1InvocationCount++;
            Fake.SubscribedEvent += (s, e) => handler2InvocationCount++;
        };

        Because of = () => Fake.SubscribedEvent += Raise.WithEmpty();

        It should_invoke_the_first_handler_once = () => handler1InvocationCount.Should().Be(1);

        It should_invoke_the_second_handler_once = () => handler2InvocationCount.Should().Be(1);
    }

    public class when_raising_generic_event_passing_arguments : EventRaisingSpecs<CustomEventArgs>
    {
        Because of = () => Fake.GenericEvent += Raise.With(SampleEventArgs1);

        It should_pass_the_fake_as_sender = () => CapturedSender.Should().BeSameAs(Fake);

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_generic_event_passing_sender_and_arguments : EventRaisingSpecs<CustomEventArgs>
    {
        Because of = () => Fake.GenericEvent += Raise.With(SampleSender, SampleEventArgs1);

        It should_pass_the_sender = () => CapturedSender.Should().BeSameAs(SampleSender);

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_generic_event_passing_arguments_and_null_sender : EventRaisingSpecs<CustomEventArgs>
    {
        Because of = () => Fake.GenericEvent += Raise.With(null, SampleEventArgs1);

        It should_pass_null_as_the_sender = () => CapturedSender.Should().BeNull();

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_custom_event_passing_sender_and_arguments : EventRaisingSpecs<CustomEventArgs>
    {
        Because of = () => Fake.CustomEvent += Raise.With<CustomEventHandler>(SampleSender, SampleEventArgs1);

        It should_pass_the_sender = () => CapturedSender.Should().BeSameAs(SampleSender);

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_reference_type_event_passing_arguments : EventRaisingSpecs<ReferenceType>
    {
        Because of = () => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(SampleEventArgs1);

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_reference_type_event_passing_derived_arguments : EventRaisingSpecs<DerivedReferenceType>
    {
        Because of = () => Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(SampleEventArgs1);

        It should_pass_the_event_arguments = () => CapturedArgs1.Should().BeSameAs(SampleEventArgs1);
    }

    public class when_raising_reference_type_event_passing_invalid_argument_type : EventRaisingSpecs
    {
        private const string ExpectedMessage =
            "The event has the signature (FakeItEasy.Specs.ReferenceType), " +
            "but the provided arguments have types (System.Collections.Hashtable).";

        Because of = () => CatchException(() =>
            Fake.ReferenceTypeEvent += Raise.With<ReferenceTypeEventHandler>(new Hashtable()));

        It should_fail_with_good_message = () =>
            CaughtException.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Be(ExpectedMessage);
    }

    public class when_raising_value_type_event_passing_null_argument : EventRaisingSpecs
    {
        Because of = () => CatchException(() =>
            Fake.ValueTypeEvent += Raise.With<ValueTypeEventHandler>((object)null));

        It should_fail_with_good_message = () =>
            CaughtException.Should().BeAnExceptionOfType<FakeConfigurationException>()
            .And.Message.Should().Be(
                "The event has the signature (System.Int32), but the provided arguments have types (<NULL>).");
    }

    public class when_raising_action_event_passing_arguments : EventRaisingSpecs<int, bool>
    {
        Establish context = () =>
        {
            SampleEventArgs1 = 19;
            SampleEventArgs2 = true;
        };

        Because of = () => Fake.ActionEvent += Raise.With<Action<int, bool>>(SampleEventArgs1, SampleEventArgs2);

        It should_pass_the_first_argument = () => CapturedArgs1.Should().Be(SampleEventArgs1);

        It should_pass_the_second_argument = () => CapturedArgs2.Should().Be(SampleEventArgs2);
    }
}
