namespace FakeItEasy.Specs
{
    using System;
    using FluentAssertions;
    using LambdaTale;

    public class EventActionSpecs
    {
        public interface IFoo
        {
            event EventHandler Bar;

            event EventHandler Baz;
        }

        [Scenario]
        public void AddSpecificEvent(IFoo fake, EventHandler barHandler, EventHandler bazHandler, EventHandler handlers)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And subscription to the Bar event of the fake is configured to update a delegate"
                .x(() => A.CallTo(fake, EventAction.Add(nameof(IFoo.Bar))).Invokes((EventHandler h) => handlers += h));

            "When the handler for Bar is subscribed to the Bar event"
                .x(() => fake.Bar += barHandler);

            "And the handler for Baz is subscribed to the Baz event"
                .x(() => fake.Baz += bazHandler);

            "Then the handler list contains the handler for Bar"
                .x(() => handlers.GetInvocationList().Should().Contain(barHandler));

            "And the handler list doesn't contain the handler for Baz"
                .x(() => handlers.GetInvocationList().Should().NotContain(bazHandler));
        }

        [Scenario]
        public void AddAnyEvent(IFoo fake, EventHandler barHandler, EventHandler bazHandler, EventHandler handlers)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And subscription to any event of the fake is configured to update a delegate"
                .x(() => A.CallTo(fake, EventAction.Add()).Invokes((EventHandler h) => handlers += h));

            "When the handler for Bar is subscribed to the Bar event"
                .x(() => fake.Bar += barHandler);

            "And the handler for Baz is subscribed to the Baz event"
                .x(() => fake.Baz += bazHandler);

            "Then the handler list contains the handler for Bar"
                .x(() => handlers.GetInvocationList().Should().Contain(barHandler));

            "And the handler list contains the handler for Baz"
                .x(() => handlers.GetInvocationList().Should().Contain(bazHandler));
        }

        [Scenario]
        public void RemoveSpecificEvent(IFoo fake, EventHandler barHandler, EventHandler bazHandler, EventHandler removedHandlers)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And unsubscription from the Bar event of the fake is configured to update a delegate"
                .x(() => A.CallTo(fake, EventAction.Remove(nameof(IFoo.Bar))).Invokes((EventHandler h) => removedHandlers += h));

            "When the handler for Bar is unsubscribed from the Bar event"
                .x(() => fake.Bar -= barHandler);

            "And the handler for Baz is unsubscribed from the Baz event"
                .x(() => fake.Baz -= bazHandler);

            "Then the removed handler list contains the handler for Bar"
                .x(() => removedHandlers.GetInvocationList().Should().Contain(barHandler));

            "And the removed handler list doesn't contain the handler for Baz"
                .x(() => removedHandlers.GetInvocationList().Should().NotContain(bazHandler));
        }

        [Scenario]
        public void RemoveAnyEvent(IFoo fake, EventHandler barHandler, EventHandler bazHandler, EventHandler removedHandlers)
        {
            "Given a fake"
                .x(() => fake = A.Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And unsubscription from any event of the fake is configured to update a delegate"
                .x(() => A.CallTo(fake, EventAction.Remove()).Invokes((EventHandler h) => removedHandlers += h));

            "When the handler for Bar is unsubscribed from the Bar event"
                .x(() => fake.Bar -= barHandler);

            "And the handler for Baz is unsubscribed from the Baz event"
                .x(() => fake.Baz -= bazHandler);

            "Then the removed handler list contains the handler for Bar"
                .x(() => removedHandlers.GetInvocationList().Should().Contain(barHandler));

            "And the removed handler list contains the handler for Baz"
                .x(() => removedHandlers.GetInvocationList().Should().Contain(bazHandler));
        }

        [Scenario]
        public void UnnaturalFakeAddSpecificEvent(Fake<IFoo> fake, EventHandler barHandler, EventHandler bazHandler, EventHandler handlers)
        {
            "Given a fake"
                .x(() => fake = new Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And subscription to the Bar event of the fake is configured to update a delegate"
                .x(() => fake.CallsTo(EventAction.Add(nameof(IFoo.Bar))).Invokes((EventHandler h) => handlers += h));

            "When the handler for Bar is subscribed to the Bar event"
                .x(() => fake.FakedObject.Bar += barHandler);

            "And the handler for Baz is subscribed to the Baz event"
                .x(() => fake.FakedObject.Baz += bazHandler);

            "Then the handler list contains the handler for Bar"
                .x(() => handlers.GetInvocationList().Should().Contain(barHandler));

            "And the handler list doesn't contain the handler for Baz"
                .x(() => handlers.GetInvocationList().Should().NotContain(bazHandler));
        }

        [Scenario]
        public void UnnaturalFakeAddAnyEvent(Fake<IFoo> fake, EventHandler barHandler, EventHandler bazHandler, EventHandler handlers)
        {
            "Given a fake"
                .x(() => fake = new Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And subscription to any event of the fake is configured to update a delegate"
                .x(() => fake.CallsTo(EventAction.Add()).Invokes((EventHandler h) => handlers += h));

            "When the handler for Bar is subscribed to the Bar event"
                .x(() => fake.FakedObject.Bar += barHandler);

            "And the handler for Baz is subscribed to the Baz event"
                .x(() => fake.FakedObject.Baz += bazHandler);

            "Then the handler list contains the handler for Bar"
                .x(() => handlers.GetInvocationList().Should().Contain(barHandler));

            "And the handler list contains the handler for Baz"
                .x(() => handlers.GetInvocationList().Should().Contain(bazHandler));
        }

        [Scenario]
        public void UnnaturalFakeRemoveSpecificEvent(Fake<IFoo> fake, EventHandler barHandler, EventHandler bazHandler, EventHandler removedHandlers)
        {
            "Given a fake"
                .x(() => fake = new Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And unsubscription from the Bar event of the fake is configured to update a delegate"
                .x(() => fake.CallsTo(EventAction.Remove(nameof(IFoo.Bar))).Invokes((EventHandler h) => removedHandlers += h));

            "When the handler for Bar is unsubscribed from the Bar event"
                .x(() => fake.FakedObject.Bar -= barHandler);

            "And the handler for Baz is unsubscribed from the Baz event"
                .x(() => fake.FakedObject.Baz -= bazHandler);

            "Then the removed handler list contains the handler for Bar"
                .x(() => removedHandlers.GetInvocationList().Should().Contain(barHandler));

            "And the removed handler list doesn't contain the handler for Baz"
                .x(() => removedHandlers.GetInvocationList().Should().NotContain(bazHandler));
        }

        [Scenario]
        public void UnnaturalFakeRemoveAnyEvent(Fake<IFoo> fake, EventHandler barHandler, EventHandler bazHandler, EventHandler removedHandlers)
        {
            "Given a fake"
                .x(() => fake = new Fake<IFoo>());

            "And event handlers for Bar and Baz"
                .x(() => (barHandler, bazHandler) = (A.Fake<EventHandler>(), A.Fake<EventHandler>()));

            "And unsubscription from any event of the fake is configured to update a delegate"
                .x(() => fake.CallsTo(EventAction.Remove()).Invokes((EventHandler h) => removedHandlers += h));

            "When the handler for Bar is unsubscribed from the Bar event"
                .x(() => fake.FakedObject.Bar -= barHandler);

            "And the handler for Baz is unsubscribed from the Baz event"
                .x(() => fake.FakedObject.Baz -= bazHandler);

            "Then the removed handler list contains the handler for Bar"
                .x(() => removedHandlers.GetInvocationList().Should().Contain(barHandler));

            "And the removed handler list contains the handler for Baz"
                .x(() => removedHandlers.GetInvocationList().Should().Contain(bazHandler));
        }
    }
}
