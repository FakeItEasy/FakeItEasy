namespace FakeItEasy.IntegrationTests
{
    using System;
    using Machine.Specifications;

    public class EventRaisingSpecs
    {
        protected static ITypeWithEvent typeWithEvent;

        Establish context = () => 
            typeWithEvent = A.Fake<ITypeWithEvent>();

        public interface ITypeWithEvent
        {
            event EventHandler<SomethingHappenedEventArgs> SomethingHappened;
        }

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
            exception = Catch.Exception(() => typeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs()).Now);

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
            typeWithEvent.SomethingHappened += (sender, e) =>
            {
                RaisingEventWithSubscriber.sender = sender;
                raisedWithArgs = e;
            };
        };

        Because of = () =>
            typeWithEvent.SomethingHappened += Raise.With(new SomethingHappenedEventArgs() {Message = "message"}).Now;

        It should_pass_the_sender = () => sender.ShouldBeTheSameAs(typeWithEvent);

        It should_pass_the_event_arguments = () => raisedWithArgs.Message = "message";

    }
}