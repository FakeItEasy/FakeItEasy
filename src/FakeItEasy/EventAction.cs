namespace FakeItEasy;

using FakeItEasy.Core;

/// <summary>
/// Represents subscription to or unsubscription from an event of a fake.
/// </summary>
/// <remarks>An instance of this class can't be explicitly created. Use the static <c>Add</c> and <c>Remove</c> methods to obtain an instance.</remarks>
public abstract class EventAction
{
    private EventAction()
    {
    }

    /// <summary>
    /// Returns an <see cref="EventAction"/> that represents subscription to the specified event of a fake.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <returns>An <see cref="EventAction"/> that represents the action.</returns>
    public static EventAction Add(string eventName) => new AddSpecificEventAction(eventName);

    /// <summary>
    /// Returns an <see cref="EventAction"/> that represents unsubscription from the specified event of a fake.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <returns>An <see cref="EventAction"/> that represents the action.</returns>
    public static EventAction Remove(string eventName) => new RemoveSpecificEventAction(eventName);

    /// <summary>
    /// Returns an <see cref="EventAction"/> that represents subscription to any event of a fake.
    /// </summary>
    /// <returns>An <see cref="EventAction"/> that represents the action.</returns>
    public static EventAction Add() => new AddAnyEventAction();

    /// <summary>
    /// Returns an <see cref="EventAction"/> that represents unsubscription from any event of a fake.
    /// </summary>
    /// <returns>An <see cref="EventAction"/> that represents the action.</returns>
    public static EventAction Remove() => new RemoveAnyEventAction();

    internal bool Matches(IFakeObjectCall call) => EventCall.TryGetEventCall(call, out var eventCall) && this.Matches(eventCall);

    internal void WriteDescription(object fake, IOutputWriter writer) => this.WriteDescription(Fake.GetFakeManager(fake).FakeObjectDisplayName, writer);

    private protected abstract bool Matches(EventCall eventCall);

    private protected abstract void WriteDescription(string fakeDisplayName, IOutputWriter writer);

    private class AddAnyEventAction : EventAction
    {
        private protected override bool Matches(EventCall eventCall) => eventCall.IsEventSubscription() && !eventCall.IsEventRaiser();

        private protected override void WriteDescription(string fakeDisplayName, IOutputWriter writer) => writer.Write($"Subscription to any event of {fakeDisplayName}");
    }

    private class RemoveAnyEventAction : EventAction
    {
        private protected override bool Matches(EventCall eventCall) => eventCall.IsEventUnsubscription();

        private protected override void WriteDescription(string fakeDisplayName, IOutputWriter writer) => writer.Write($"Unsubscription from any event of {fakeDisplayName}");
    }

    private class AddSpecificEventAction : EventAction
    {
        private readonly string eventName;

        public AddSpecificEventAction(string eventName)
        {
            this.eventName = eventName;
        }

        private protected override bool Matches(EventCall eventCall) => eventCall.Event.Name == this.eventName && eventCall.IsEventSubscription() && !eventCall.IsEventRaiser();

        private protected override void WriteDescription(string fakeDisplayName, IOutputWriter writer) => writer.Write($"Subscription to event '{this.eventName}' of {fakeDisplayName}");
    }

    private class RemoveSpecificEventAction : EventAction
    {
        private readonly string eventName;

        public RemoveSpecificEventAction(string eventName)
        {
            this.eventName = eventName;
        }

        private protected override bool Matches(EventCall eventCall) => eventCall.Event.Name == this.eventName && eventCall.IsEventUnsubscription();

        private protected override void WriteDescription(string fakeDisplayName, IOutputWriter writer) => writer.Write($"Unsubscription from event '{this.eventName}' of {fakeDisplayName}");
    }
}
