namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    internal class EventCall
    {
        private static readonly Func<EventInfo, MethodInfo> GetAddMethod = e => e.GetAddMethod(true)!;
        private static readonly Func<EventInfo, MethodInfo> GetRemoveMethod = e => e.GetRemoveMethod(true)!;
        private static readonly EventHandlerArgumentProviderMap EventHandlerArgumentProviderMap =
            ServiceLocator.Resolve<EventHandlerArgumentProviderMap>();

        private EventCall(EventInfo @event, MethodInfo callingMethod, Delegate eventHandler)
        {
            this.Event = @event;
            this.CallingMethod = callingMethod;
            this.EventHandler = eventHandler;
        }

        public EventInfo Event { get; }

        public Delegate EventHandler { get; }

        private MethodInfo CallingMethod { get; }

        public static bool TryGetEventCall(IFakeObjectCall fakeObjectCall, [NotNullWhen(true)] out EventCall? eventCall)
        {
            eventCall = null;
            var eventInfo = GetEvent(fakeObjectCall.Method);
            if (eventInfo is null)
            {
                return false;
            }

            var handler = fakeObjectCall.Arguments.Get<Delegate>(0);
            if (handler is null)
            {
                return false;
            }

            eventCall = new EventCall(eventInfo, fakeObjectCall.Method, handler);
            return true;
        }

        public bool IsEventSubscription()
        {
            return GetAddMethod(this.Event).Equals(this.CallingMethod);
        }

        public bool IsEventUnsubscription()
        {
            return GetRemoveMethod(this.Event).Equals(this.CallingMethod);
        }

        public bool TryTakeEventRaiserArgumentProvider([NotNullWhen(true)] out IEventRaiserArgumentProvider? argumentProvider)
        {
            return EventHandlerArgumentProviderMap.TryTakeArgumentProviderFor(this.EventHandler, out argumentProvider);
        }

        public bool IsEventRaiser() => EventHandlerArgumentProviderMap.HasArgumentProvider(this.EventHandler);

        private static EventInfo? GetEvent(MethodInfo eventAdderOrRemover)
        {
            if (!eventAdderOrRemover.IsSpecialName)
            {
                return null;
            }

            Func<EventInfo, MethodInfo> getMethod;
            if (eventAdderOrRemover.Name.StartsWith("add_", StringComparison.Ordinal))
            {
                getMethod = GetAddMethod;
            }
            else if (eventAdderOrRemover.Name.StartsWith("remove_", StringComparison.Ordinal))
            {
                getMethod = GetRemoveMethod;
            }
            else
            {
                return null;
            }

            var eventInfos = eventAdderOrRemover.DeclaringType!.GetEvents(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);
            if (eventInfos.Length == 0)
            {
                return null;
            }

            var adderOrRemoverDefinition = eventAdderOrRemover.GetBaseDefinition();
            return
                eventInfos.SingleOrDefault(e =>
                    Equals(getMethod(e).GetBaseDefinition(), adderOrRemoverDefinition));
        }
    }
}
