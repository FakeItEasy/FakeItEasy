namespace FakeItEasy.Specs
{
    using System;
    using FakeItEasy.Tests.TestHelpers;
    using FluentAssertions;
    using LambdaTale;
    using Xunit;

    public class ManageEventsSpecs
    {
        public interface IFoo
        {
            event EventHandler<int> Bar;

            event EventHandler<int> Baz;
        }

        [Scenario]
        public void ManageSpecificEvent(IFoo fake, EventHandler<int> handler)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(o => o.Strict()));

            "And an event handler"
                .x(() => handler = A.Fake<EventHandler<int>>());

            "And the fake is configured to manage the Bar event"
                .x(() => Manage.Event(nameof(IFoo.Bar)).Of(fake));

            "When the handler is subscribed to the Bar event"
                .x(() => fake.Bar += handler);

            "And the Bar event is raised using Raise"
                .x(() => fake.Bar += Raise.With(1));

            "Then the handler is called"
                .x(() => A.CallTo(() => handler(fake, 1)).MustHaveHappened());
        }

        [Scenario]
        public void ManageSpecificEventUnsubscribe(IFoo fake, EventHandler<int> handler)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(o => o.Strict()));

            "And an event handler"
                .x(() => handler = A.Fake<EventHandler<int>>());

            "And the fake is configured to manage the Bar event"
                .x(() => Manage.Event(nameof(IFoo.Bar)).Of(fake));

            "When the handler is subscribed to the Bar event"
                .x(() => fake.Bar += handler);

            "And the handler is unsubscribed from the Bar event"
                .x(() => fake.Bar -= handler);

            "And the Bar event is raised using Raise"
                .x(() => fake.Bar += Raise.With(1));

            "Then the handler is not called"
                .x(() => A.CallTo(handler).MustNotHaveHappened());
        }

        [Scenario]
        public void ManageSpecificEventDoesntHandlerOtherEvents(IFoo fake, EventHandler<int> handler, Exception exception)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(o => o.Strict()));

            "And an event handler"
                .x(() => handler = A.Fake<EventHandler<int>>());

            "And the fake is configured to manage the Bar event"
                .x(() => Manage.Event(nameof(IFoo.Bar)).Of(fake));

            "When the handler is subscribed to the Baz event"
                .x(() => exception = Record.Exception(() => fake.Baz += handler));

            "Then it throws an ExpectationException"
                .x(() => exception.Should().BeAnExceptionOfType<ExpectationException>());
        }

        [Scenario]
        public void ManageAllEvents(IFoo fake, EventHandler<int> barHandler, EventHandler<int> bazHandler)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(o => o.Strict()));

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler<int>>(), A.Fake<EventHandler<int>>()));

            "And the fake is configured to manage any event"
                .x(() => Manage.AllEvents.Of(fake));

            "When the handler for Bar is subscribed to the Bar event"
                .x(() => fake.Bar += barHandler);

            "And the handler for Baz is subscribed to the Baz event"
                .x(() => fake.Baz += bazHandler);

            "And the Bar event is raised using Raise"
                .x(() => fake.Bar += Raise.With(1));

            "And the Baz event is raised using Raise"
                .x(() => fake.Baz += Raise.With(2));

            "Then the handler for Bar is called"
                .x(() => A.CallTo(() => barHandler(fake, 1)).MustHaveHappened());

            "And the handler for Baz is called"
                .x(() => A.CallTo(() => bazHandler(fake, 2)).MustHaveHappened());
        }

        [Scenario]
        public void MultipleCallsToManageEvent(IFoo fake, EventHandler<int> handler)
        {
            "Given a strict fake"
                .x(() => fake = A.Fake<IFoo>(o => o.Strict()));

            "And an event handler"
                .x(() => handler = A.Fake<EventHandler<int>>());

            "When the fake is configured to manage the Bar event"
                .x(() => Manage.Event(nameof(IFoo.Bar)).Of(fake));

            "And the handler is subscribed to the Bar event"
                .x(() => fake.Bar += handler);

            "And the fake is configured again to manage the Bar event"
                .x(() => Manage.Event(nameof(IFoo.Bar)).Of(fake));

            "And the Bar event is raised using Raise"
                .x(() => fake.Bar += Raise.With(1));

            "Then the handler is called"
                .x(() => A.CallTo(() => handler(fake, 1)).MustHaveHappened());
        }
    }
}
