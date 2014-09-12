namespace FakeItEasy.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class EventRaisingSpecs
    {
        Establish context = () => TypeWithEvent = A.Fake<ITypeWithEvent>();

        public interface ITypeWithEvent
        {
            event EventHandler<SomethingHappenedEventArgs> GenericEventHandler;
            
            event EventHandler EventHandler;
        }

        protected static ITypeWithEvent TypeWithEvent { get; private set; }

        public class SomethingHappenedEventArgs
            : EventArgs
        {
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