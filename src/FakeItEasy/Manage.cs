namespace FakeItEasy
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// Provides methods for managing fake events automatically.
    /// </summary>
    public static class Manage
    {
        /// <summary>
        /// Specifies all events of the fake.
        /// </summary>
        /// <returns>A fluent configuration object to specify the fake.</returns>
#pragma warning disable SA1623 // PropertySummaryDocumentationMustMatchAccessors
        public static IManageEventConfiguration AllEvents => new ManageAllEventsConfiguration();
#pragma warning restore SA1623 // PropertySummaryDocumentationMustMatchAccessors

        /// <summary>
        /// Specifies a named event of the fake.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <returns>A fluent configuration object to specify the fake.</returns>
        public static IManageEventConfiguration Event(string eventName) => new ManageNamedEventConfiguration(eventName);

        private static void ManageEvents(object fake, Func<EventCall, bool> eventCallPredicate)
        {
            Guard.AgainstNull(fake, nameof(fake));

            var handler = new EventCallHandler(Fake.GetFakeManager(fake));
            A.CallTo(fake)
                .WithVoidReturnType()
                .Where(
                    call => EventCall.TryGetEventCall(call, out var eventCall) && eventCallPredicate(eventCall),
                    writer => { }) // This call spec will never be asserted, so we don't need to write a description
                .Invokes(call =>
                {
                    if (EventCall.TryGetEventCall(call, out var eventCall))
                    {
                        handler.HandleEventCall(eventCall);
                    }
                });
        }

        private class ManageNamedEventConfiguration : IManageEventConfiguration
        {
            private readonly string eventName;

            public ManageNamedEventConfiguration(string eventName)
            {
                Guard.AgainstNull(eventName, nameof(eventName));
                this.eventName = eventName;
            }

            public void Of(object fake) => ManageEvents(fake, e => e.Event.Name == this.eventName);
        }

        private class ManageAllEventsConfiguration : IManageEventConfiguration
        {
            public void Of(object fake) => ManageEvents(fake, _ => true);
        }
    }
}
