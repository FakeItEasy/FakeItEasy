namespace FakeItEasy.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class EventRaisingSpecs
    {
        Establish context = () => TypeWithEvent = A.Fake<ITypeWithEvent>();

        public delegate void TimedEvent(DateTime eventStart);

        public interface ITypeWithEvent
        {
            event EventHandler<SomethingHappenedEventArgs> GenericEventHandler;
            
            event EventHandler EventHandler;

            event Action<int, string> ActionEvent;

            event TimedEvent EventStarted;
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

        private Establish context = () =>
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

        private It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "with subscribers and using Now")]
    public class RaisingEventHandlerWithSubscriberUsingNow
        : EventRaisingSpecs
    {
        static EventArgs raisedWithArgs;
        static EventArgs capturedArgs;
        static object capturedSender;

        private Establish context = () =>
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

        private It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "with subscribers")]
    public class RaisingGenericEventHandlerWithSubscriber
        : EventRaisingSpecs
    {
        static SomethingHappenedEventArgs raisedWithArgs;
        static SomethingHappenedEventArgs capturedArgs;
        static object capturedSender;

        private Establish context = () =>
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

        private It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "with subscribers and using Now")]
    public class RaisingGenericEventHandlerWithSubscriberUsingNow
        : EventRaisingSpecs
    {
        static SomethingHappenedEventArgs raisedWithArgs;
        static SomethingHappenedEventArgs capturedArgs;
        static object capturedSender;

        private Establish context = () =>
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

        private It should_pass_the_event_arguments = () => capturedArgs.Should().BeSameAs(raisedWithArgs);
    }

    [Subject(typeof(Raise), "with subscribers")]
    public class RaisingDelegateEventWithSubscriber
        : EventRaisingSpecs
    {
        static DateTime raisedWithTime;
        static DateTime capturedTime;

        private Establish context = () =>
        {
            raisedWithTime = DateTime.Now;
            TypeWithEvent.EventStarted += time => capturedTime = time;
        };

        private Because of = () => TypeWithEvent.EventStarted += Raise.Event<TimedEvent>(raisedWithTime);

        It should_pass_the_arguments = () => capturedTime.Should().Be(raisedWithTime);
    }

    [Subject(typeof(Raise), "with subscribers")]
    public class RaisingActionEventWithSubscriber
        : EventRaisingSpecs
    {
        static int raisedWithInt;
        static string raisedWithString;
        static int capturedInt;
        static string capturedString;

        private Establish context = () =>
        {
            raisedWithInt = 19;
            raisedWithString = "raisedWithString";
            TypeWithEvent.ActionEvent += (i, s) =>
            {
                capturedInt = i;
                capturedString = s;
            };
        };

        private Because of = () => TypeWithEvent.ActionEvent += Raise.Event<Action<int, string>>(raisedWithInt, raisedWithString);

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
}