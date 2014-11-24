namespace FakeItEasy.Specs
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public class EventRaisingSpecs
    {
        Establish context = () => TypeWithEvent = A.Fake<ITypeWithEvent>();

        public delegate void DelegateEvent(ArrayList eventList);

        public delegate void DelegateWithValueArgumentEvent(int count);

        [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances",
            Justification = "Required for testing nonstandard events.")]
        public delegate void DerivedArgsEventHandler(object sender, SomethingHappenedEventArgs args);

        public interface ITypeWithEvent
        {
            event EventHandler<SomethingHappenedEventArgs> GenericEventHandler;
            
            event EventHandler EventHandler;

            [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
                Justification = "Required for testing nonstandard events.")]
            event DerivedArgsEventHandler DerivedEventHandler;

            [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
                Justification = "Required for testing nonstandard events.")]
            [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
                Justification = "Required for testing nonstandard events.")]
            event Action<int, string> ActionEventHandler;

            [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
                Justification = "Required for testing nonstandard events.")]
            [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
                Justification = "Required for testing nonstandard events.")]
            event DelegateEvent DelegateEventHandler;

            [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
                Justification = "Required for testing nonstandard events.")]
            [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
                Justification = "Required for testing nonstandard events.")]
            event DelegateWithValueArgumentEvent DelegateWithValueArgumentEventHandler;
        }

        protected static ITypeWithEvent TypeWithEvent { get; private set; }

        public class SomethingHappenedEventArgs
            : EventArgs
        {
        }
    }

    [Subject(typeof(Raise), "eventhandler with no subscribers")]
    public class RaisingEventWithNoSubscriber
        : EventRaisingSpecs
    {
        static Exception exception;

        Because of = () =>
            exception = Catch.Exception(() => TypeWithEvent.EventHandler += Raise.With(new SomethingHappenedEventArgs()));

        It should_not_throw = () => exception.Should().BeNull();
    }

    [Subject(typeof(Raise), "eventhandler with subscriber")]
    public class RaisingEventHandlerWithSubscriber
        : EventRaisingSpecs
    {
        static EventArgs raisedWithArgs;
        static EventArgs capturedArgs;
        static object capturedSender;

        Establish context = () =>
        {
            raisedWithArgs = new EventArgs();
            TypeWithEvent.EventHandler += (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };
        };

        Because of = () => TypeWithEvent.EventHandler += Raise.With(raisedWithArgs);

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(TypeWithEvent);

        It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "eventhandler with subscriber specifying sender and args")]
    public class RaisingEventHandlerWithSubscriberSpecifyingSenderAndArgs
        : EventRaisingSpecs
    {
        static EventArgs raisedWithArgs;
        static object raisedWithSender;
        static EventArgs capturedArgs;
        static object capturedSender;

        Establish context = () =>
        {
            raisedWithSender = new object();
            raisedWithArgs = new EventArgs();
            TypeWithEvent.EventHandler += (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };
        };

        Because of = () => TypeWithEvent.EventHandler += Raise.With(raisedWithSender, raisedWithArgs);

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(raisedWithSender);

        It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "eventhandler with subscriber using WithEmpty")]
    public class RaisingEventHandlerWithSubscriberUsingWithEmpty
        : EventRaisingSpecs
    {
        static EventArgs capturedArgs;
        static object capturedSender;

        Establish context = () =>
        {
            TypeWithEvent.EventHandler += (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };
        };

        Because of = () => TypeWithEvent.EventHandler += Raise.WithEmpty();

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(TypeWithEvent);

        It should_pass_empty_event_arguments = () => capturedArgs.Should().Be(EventArgs.Empty);
    }

    [Subject(typeof(Raise), "generic eventhandler with subscriber")]
    public class RaisingGenericEventHandlerWithSubscriber
        : EventRaisingSpecs
    {
        static SomethingHappenedEventArgs raisedWithArgs;
        static SomethingHappenedEventArgs capturedArgs;
        static object capturedSender;

        Establish context = () =>
        {
            raisedWithArgs = new SomethingHappenedEventArgs();
            TypeWithEvent.GenericEventHandler += (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };
        };

        Because of = () => TypeWithEvent.GenericEventHandler += Raise.With(raisedWithArgs);

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(TypeWithEvent);

        It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "generic eventhandler with subscriber specifying sender and args")]
    public class RaisingGenericEventHandlerWithSubscriberSpecifyingSenderAndArgs
        : EventRaisingSpecs
    {
        static SomethingHappenedEventArgs raisedWithArgs;
        static object raisedWithSender;
        static SomethingHappenedEventArgs capturedArgs;
        static object capturedSender;

        Establish context = () =>
        {
            raisedWithSender = new object();
            raisedWithArgs = new SomethingHappenedEventArgs();
            TypeWithEvent.GenericEventHandler += (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };
        };

        Because of = () => TypeWithEvent.GenericEventHandler += Raise.With(raisedWithSender, raisedWithArgs);

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(raisedWithSender);

        It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "derived eventhandler with subscriber specifying sender and args")]
    public class RaisingDerivedEventHandlerWithSubscriberSpecifyingSenderAndArgs
        : EventRaisingSpecs
    {
        static SomethingHappenedEventArgs raisedWithArgs;
        static object raisedWithSender;
        static SomethingHappenedEventArgs capturedArgs;
        static object capturedSender;

        Establish context = () =>
        {
            raisedWithArgs = new SomethingHappenedEventArgs();
            raisedWithSender = new object();
            TypeWithEvent.DerivedEventHandler += (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };
        };

        Because of = () => TypeWithEvent.DerivedEventHandler += Raise.With<DerivedArgsEventHandler>(raisedWithSender, raisedWithArgs);

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(raisedWithSender);

        It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "delegate with subscriber")]
    public class RaisingDelegateEventSubscriber
        : EventRaisingSpecs
    {
        static ArrayList raisedWithList;
        static ArrayList capturedList;

        Establish context = () =>
        {
            raisedWithList = new ArrayList();
            TypeWithEvent.DelegateEventHandler += list => capturedList = list;
        };

        Because of = () => TypeWithEvent.DelegateEventHandler += Raise.With<DelegateEvent>(raisedWithList);

        It should_pass_the_argument = () => ((object)capturedList).Should().BeSameAs(raisedWithList);
    }

    [Subject(typeof(Raise), "action with subscriber")]
    public class RaisingActionEventSubscriber
        : EventRaisingSpecs
    {
        static int raisedWithInt;
        static string raisedWithString;
        static int capturedInt;
        static string capturedString;

        Establish context = () =>
        {
            raisedWithInt = 19;
            raisedWithString = "raisedWithString";
            TypeWithEvent.ActionEventHandler += (i, s) =>
            {
                capturedInt = i;
                capturedString = s;
            };
        };

        Because of = () => TypeWithEvent.ActionEventHandler += Raise.With<Action<int, string>>(raisedWithInt, raisedWithString);

        It should_pass_the_first_argument = () => capturedInt.Should().Be(raisedWithInt);

        It should_pass_the_second_argument = () => capturedString.Should().Be(raisedWithString);
    }

    [Subject(typeof(Raise), "delegate with subscriber specifying argument of derived type")]
    public class RaisingDelegateEventSubscriberSpecifyingDerivedArgument
        : EventRaisingSpecs
    {
        static ArrayList raisedWithList;
        static ArrayList capturedList;

        Establish context = () =>
        {
            TypeWithEvent.DelegateEventHandler += list => { capturedList = list; };
        };

        Because of = () =>
        {
            raisedWithList = new MyArrayList();
            TypeWithEvent.DelegateEventHandler += Raise.With<DelegateEvent>(raisedWithList);
        };

        It should_pass_the_argument = () => ((object)capturedList).Should().BeSameAs(raisedWithList);

        private class MyArrayList : ArrayList
        {
        }
    }

    [Subject(typeof(Raise), "delegate with subscriber specifying invalid argument")]
    public class RaisingDelegateEventSubscriberSpecifyingInvalidArgument
        : EventRaisingSpecs
    {
        private const string ExpectedMessage =
            "The event has the signature (System.Collections.ArrayList), but the provided arguments have types " +
            "(System.Collections.Hashtable).";

        static Exception exception;

        Establish context = () =>
        {
            TypeWithEvent.DelegateEventHandler += list => { };
        };

        Because of = () =>
        {
            exception = Catch.Exception(() =>
                TypeWithEvent.DelegateEventHandler += Raise.With<DelegateEvent>(new Hashtable()));
        };

        It should_fail_with_good_message = () =>
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Be(ExpectedMessage);
    }

    [Subject(typeof(Raise), "delegate with subscriber specifying null for value argument")]
    public class RaisingDelegateEventSubscriberSpecifyingNullForValueArgument
        : EventRaisingSpecs
    {
        private const string ExpectedMessage = 
            "The event has the signature (System.Int32), but the provided arguments have types " +
            "(<NULL>).";

        static Exception exception;

        Establish context = () =>
        {
            TypeWithEvent.DelegateWithValueArgumentEventHandler += list => { };
        };

        Because of = () =>
        {
            exception = Catch.Exception(() =>
                TypeWithEvent.DelegateWithValueArgumentEventHandler += Raise.With<DelegateWithValueArgumentEvent>((object)null));
        };

        It should_fail_with_good_message = () =>
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Be(ExpectedMessage);
    }

    [Subject(typeof(Raise), "eventhandler with multiple subscribers")]
    public class RaisingEventHandlerWithMultipleSubscribers
        : EventRaisingSpecs
    {
        static int firstWasRaisedNumberOfTimes;
        static int secondWasRaisedNumberOfTimes;

        Establish context = () =>
        {
            TypeWithEvent.EventHandler += (s, e) => firstWasRaisedNumberOfTimes++;
            TypeWithEvent.EventHandler += (s, e) => secondWasRaisedNumberOfTimes++;
        };

        Because of = () => TypeWithEvent.EventHandler += Raise.With(new SomethingHappenedEventArgs());

        It should_invoke_the_first_handler = () => firstWasRaisedNumberOfTimes.Should().Be(1);

        It should_invoke_the_second_handler = () => secondWasRaisedNumberOfTimes.Should().Be(1);
    }
}