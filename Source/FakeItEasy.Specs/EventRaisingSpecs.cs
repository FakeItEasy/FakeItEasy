﻿namespace FakeItEasy.IntegrationTests
{
    using System;
    using Machine.Specifications;

    public class EventRaisingSpecs
    {
        Establish context = () => TypeWithEvent = A.Fake<ITypeWithEvent>();

        public interface ITypeWithEvent
        {
            event EventHandler<SomethingHappenedEventArgs> SomethingHappened;
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
            exception = Catch.Exception(() => TypeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs()).Now);

        It should_not_throw = () => exception.ShouldBeNull();
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

        Because of = () => TypeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs() { Message = "message" }).Now;

        It should_pass_the_sender = () => sender.ShouldBeTheSameAs(TypeWithEvent);

        It should_pass_the_event_arguments = () => raisedWithArgs.Message = "message";
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

        Because of = () => TypeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs()).Now;

        It should_invoke_the_first_handler = () => firstWasRaisedNumberOfTimes.ShouldEqual(1);

        It should_invoke_the_second_handler = () => secondWasRaisedNumberOfTimes.ShouldEqual(1);
    }
}