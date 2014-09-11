namespace FakeItEasy.IntegrationTests
{
    using System;
    using System.Collections;

    using FakeItEasy.Configuration;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using Machine.Specifications;

    public class EventRaisingSpecs
    {
        Establish context = () => TypeWithEvent = A.Fake<ITypeWithEvent>();

        public delegate void DelegateEvent(ArrayList eventList);

        public delegate void DelegateEventWithValueArgument(int count);

        public interface ITypeWithEvent
        {
            event EventHandler<SomethingHappenedEventArgs> GenericEventHandler;
            
            event EventHandler EventHandler;

            event Action<int, string> ActionEvent;

            event DelegateEvent DelegateEvent;

            event DelegateEventWithValueArgument DelegateEventWithValueArgument;
        }

        protected static ITypeWithEvent TypeWithEvent { get; private set; }

        public class SomethingHappenedEventArgs
            : EventArgs
        {
            public string Message { get; set; }
        }
    }

    [Subject(typeof(Raise), "with no subscribers")]
    public class RaisingEventWithNoSubscriber
        : EventRaisingSpecs
    {
        static Exception exception;

        Because of = () =>
            exception = Catch.Exception(() => TypeWithEvent.EventHandler += Raise.With(new SomethingHappenedEventArgs()));

        It should_not_throw = () => exception.Should().BeNull();
    }

    [Subject(typeof(Raise), "with subscribers")]
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

    [Subject(typeof(Raise), "with subscribers and using Now")]
    public class RaisingEventHandlerWithSubscriberUsingNow
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

        Because of = () => TypeWithEvent.EventHandler += Raise.With(raisedWithArgs).Now;

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(TypeWithEvent);

        It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "with subscribers")]
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

    [Subject(typeof(Raise), "with subscribers and using Now")]
    public class RaisingGenericEventHandlerWithSubscriberUsingNow
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

        Because of = () => TypeWithEvent.GenericEventHandler += Raise.With(raisedWithArgs).Now;

        It should_pass_the_sender = () => capturedSender.Should().BeSameAs(TypeWithEvent);

        It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "with subscribers")]
    public class RaisingDelegateEventWithSubscriber
        : EventRaisingSpecs
    {
        static ArrayList raisedWithList;
        static ArrayList capturedList;

        Establish context = () =>
        {
            raisedWithList = new ArrayList();
            TypeWithEvent.DelegateEvent += time => capturedList = time;
        };

        Because of = () => TypeWithEvent.DelegateEvent += Raise.With<DelegateEvent>(raisedWithList);

        It should_pass_the_arguments = () => ((object)capturedList).Should().BeSameAs(raisedWithList);
    }

    [Subject(typeof(Raise), "with subscribers")]
    public class RaisingActionEventWithSubscriber
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
            TypeWithEvent.ActionEvent += (i, s) =>
            {
                capturedInt = i;
                capturedString = s;
            };
        };

        Because of = () => TypeWithEvent.ActionEvent += Raise.With<Action<int, string>>(raisedWithInt, raisedWithString);

        It should_pass_the_first_argument = () => capturedInt.Should().Be(raisedWithInt);

        It should_pass_the_second_argument = () => capturedString.Should().Be(raisedWithString);
    }

    [Subject(typeof(Raise), "With multiple subscribers")]
    public class RaisingEventWithMultipleSubscribers
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

    [Subject(typeof(Raise), "With argument of derived type")]
    public class RaisingDelegateEventDerivedArgument
        : EventRaisingSpecs
    {
        static ArrayList raisedWithList;
        static ArrayList capturedList;

        Establish context = () =>
        {
            TypeWithEvent.DelegateEvent += list => { capturedList = list; };
        };

        Because of = () =>
        {
            raisedWithList = new MyArrayList();
            TypeWithEvent.DelegateEvent += Raise.With<DelegateEvent>(raisedWithList);
        };

        It should_pass_the_arguments = () => ((object)capturedList).Should().BeSameAs(raisedWithList);

        private class MyArrayList : ArrayList
        {
        }
    }

    [Subject(typeof(Raise), "With wrong-typed argument")]
    public class RaisingDelegateEventInvalidArgument
        : EventRaisingSpecs
    {
        static Exception exception;

        Establish context = () =>
        {
            TypeWithEvent.DelegateEvent += list => { };
        };

        Because of = () =>
        {
            exception = Catch.Exception(() =>
                TypeWithEvent.DelegateEvent += Raise.With<DelegateEvent>(new Hashtable()));
        };

        It should_fail_with_good_message = () =>
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Be(
                    "The event has the signature (System.Collections.ArrayList), but the provided " +
                    "arguments have types (System.Collections.Hashtable).");
    }

    [Subject(typeof(Raise), "With null for value argument")]
    public class RaisingDelegateEventNullForValueArgument
        : EventRaisingSpecs
    {
        static Exception exception;

        Establish context = () =>
        {
            TypeWithEvent.DelegateEventWithValueArgument += list => { };
        };

        Because of = () =>
        {
            exception = Catch.Exception(() =>
                TypeWithEvent.DelegateEventWithValueArgument += Raise.With<DelegateEventWithValueArgument>((object)null));
        };

        It should_fail_with_good_message = () =>
            exception.Should().BeAnExceptionOfType<FakeConfigurationException>().And
                .Message.Should().Be(
                    "The event has the signature (System.Int32), but the provided " +
                    "arguments have types (<NULL>).");
    }
}