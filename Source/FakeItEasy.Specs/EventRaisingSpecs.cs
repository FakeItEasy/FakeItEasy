﻿namespace FakeItEasy.IntegrationTests
{
    using System;
    using FluentAssertions;
    using Machine.Specifications;

    public class EventRaisingSpecs
    {
        Establish context = () => TypeWithEvent = A.Fake<ITypeWithEvent>();

        public interface ITypeWithEvent
        {
            event EventHandler<SomethingHappenedEventArgs> SomethingHappened;

            event Action<int> SomethingHappenedThatJustNeedsAnInt;
        }

        protected static ITypeWithEvent TypeWithEvent { get; set; }

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
            exception = Catch.Exception(() => TypeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs()));

        It should_not_throw = () => exception.Should().BeNull();
    }

    [Subject(typeof(Raise), "with subscribers")]
    public class RaisingEventWithSubscriber
        : EventRaisingSpecs
    {
        static SomethingHappenedEventArgs raisedWithArgs;
        static object sender;

        private Establish context = () =>
        {
            TypeWithEvent.SomethingHappened += (sender, e) =>
            {
                RaisingEventWithSubscriber.sender = sender;
                raisedWithArgs = e;
            };
        };

        Because of = () => TypeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs() { Message = "message" });

        It should_pass_the_sender = () => sender.Should().BeSameAs(TypeWithEvent);

        It should_pass_the_event_arguments = () => raisedWithArgs.Message = "message";
    }

    [Subject(typeof(Raise), "with subscribers and using Now")]
    public class RaisingEventWithSubscriberUsingNow
        : EventRaisingSpecs
    {
        static SomethingHappenedEventArgs raisedWithArgs;
        static object sender;

        private Establish context = () =>
        {
            TypeWithEvent.SomethingHappened += (sender, e) =>
            {
                RaisingEventWithSubscriberUsingNow.sender = sender;
                raisedWithArgs = e;
            };
        };

        Because of = () => TypeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs() { Message = "message" }).Now;

        It should_pass_the_sender = () => sender.Should().BeSameAs(TypeWithEvent);

        It should_pass_the_event_arguments = () => raisedWithArgs.Message = "message";
    }

    [Subject(typeof(Raise), "nonstandard event")]
    public class RaisingNonstandardEvent
        : EventRaisingSpecs
    {
        static int argument;

        private Establish context = () =>
        {
            TypeWithEvent.SomethingHappenedThatJustNeedsAnInt += i =>
            {
                RaisingNonstandardEvent.argument = i;
            };
        };

        Because of = () => TypeWithEvent.SomethingHappenedThatJustNeedsAnInt += Raise.Event<Action<int>>(19);

        private It should_pass_the_arguments = () => argument.Should().Be(19);
    }

    [Subject(typeof(Raise), "With multiple subscribers")]
    public class RaisingEventWithMultipleSubscribers
        : EventRaisingSpecs
    {
        static int firstWasRaisedNumberOfTimes;
        static int secondWasRaisedNumberOfTimes;

        Establish context = () =>
        {
            TypeWithEvent.SomethingHappened += (s, e) => firstWasRaisedNumberOfTimes++;
            TypeWithEvent.SomethingHappened += (s, e) => secondWasRaisedNumberOfTimes++;
        };

        Because of = () => TypeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs());

        It should_invoke_the_first_handler = () => firstWasRaisedNumberOfTimes.Should().Be(1);

        It should_invoke_the_second_handler = () => secondWasRaisedNumberOfTimes.Should().Be(1);
    }
}