namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <content>Event rule.</content>
    public partial class FakeManager
    {
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Would provide no benefit since there is no place from where to call the Dispose-method.")]
        private class EventRule
            : IFakeObjectCallRule
        {
            private static readonly EventHandlerArgumentProviderMap EventHandlerArgumentProviderMap =
                ServiceLocator.Resolve<EventHandlerArgumentProviderMap>();

            private readonly FakeManager fakeManager;

            private Dictionary<object, Delegate> registeredEventHandlersField;

            public EventRule(FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
            }

            public int? NumberOfTimesToCall => null;

            private Dictionary<object, Delegate> RegisteredEventHandlers
            {
                get
                {
                    if (this.registeredEventHandlersField is null)
                    {
                        this.registeredEventHandlersField = new Dictionary<object, Delegate>();
                    }

                    return this.registeredEventHandlersField;
                }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                return EventCall.GetEvent(fakeObjectCall.Method) is object;
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                var eventCall = EventCall.GetEventCall(fakeObjectCall);

                this.HandleEventCall(eventCall);
            }

            private void HandleEventCall(EventCall eventCall)
            {
                if (eventCall.IsEventRegistration())
                {
                    IEventRaiserArgumentProvider argumentProvider;
                    if (EventHandlerArgumentProviderMap.TryTakeArgumentProviderFor(
                        eventCall.EventHandler,
                        out argumentProvider))
                    {
                        this.RaiseEvent(eventCall, argumentProvider);
                    }
                    else
                    {
                        this.AddEventListener(eventCall);
                    }
                }
                else
                {
                    this.RemoveEventListener(eventCall);
                }
            }

            private void RemoveEventListener(EventCall call)
            {
                this.RemoveHandler(call.Event, call.EventHandler);
            }

            private void AddEventListener(EventCall call)
            {
                this.AddHandler(call.Event, call.EventHandler);
            }

            private void AddHandler(object key, Delegate handler)
            {
                Delegate result;

                if (this.RegisteredEventHandlers.TryGetValue(key, out result))
                {
                    result = Delegate.Combine(result, handler);
                }
                else
                {
                    result = handler;
                }

                this.RegisteredEventHandlers[key] = result;
            }

            private void RemoveHandler(object key, Delegate handler)
            {
                Delegate registration;

                if (this.RegisteredEventHandlers.TryGetValue(key, out registration))
                {
                    registration = Delegate.Remove(registration, handler);

                    if (registration is object)
                    {
                        this.RegisteredEventHandlers[key] = registration;
                    }
                    else
                    {
                        this.RegisteredEventHandlers.Remove(key);
                    }
                }
            }

            private void RaiseEvent(EventCall call, IEventRaiserArgumentProvider argumentProvider)
            {
                Delegate raiseMethod;

                if (this.RegisteredEventHandlers.TryGetValue(call.Event, out raiseMethod))
                {
                    var arguments = argumentProvider.GetEventArguments(this.fakeManager.Object);

                    try
                    {
                        raiseMethod.DynamicInvoke(arguments);
                    }
                    catch (TargetInvocationException ex)
                    {
                        // Exceptions thrown by event handlers should propagate outward as is, not
                        // be wrapped in a TargetInvocationException.
                        ex.InnerException?.Rethrow();
                        throw;
                    }
                }
            }

            private class EventCall
            {
                private static readonly Func<EventInfo, MethodInfo> GetAddMethod = e => e.GetAddMethod(true);
                private static readonly Func<EventInfo, MethodInfo> GetRemoveMethod = e => e.GetRemoveMethod(true);

                private EventCall(EventInfo @event, MethodInfo callingMethod, Delegate eventHandler)
                {
                    this.Event = @event;
                    this.CallingMethod = callingMethod;
                    this.EventHandler = eventHandler;
                }

                public EventInfo Event { get; }

                public Delegate EventHandler { get; }

                private MethodInfo CallingMethod { get; }

                public static EventCall GetEventCall(
                    IFakeObjectCall fakeObjectCall)
                {
                    var eventInfo = GetEvent(fakeObjectCall.Method);

                    return new EventCall(eventInfo, fakeObjectCall.Method, (Delegate)fakeObjectCall.Arguments[0] !);
                }

                public static EventInfo GetEvent(MethodInfo eventAdderOrRemover)
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

                    var eventInfos = eventAdderOrRemover.DeclaringType.GetEvents(
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

                public bool IsEventRegistration()
                {
                    return GetAddMethod(this.Event).Equals(this.CallingMethod);
                }
            }
        }
    }
}
