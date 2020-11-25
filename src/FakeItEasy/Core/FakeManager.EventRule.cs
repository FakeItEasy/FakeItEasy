namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <content>Event rule.</content>
    public partial class FakeManager
    {
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Would provide no benefit since there is no place from where to call the Dispose-method.")]
        private class EventRule
            : IFakeObjectCallRule
        {
            private readonly FakeManager fakeManager;

            private Dictionary<object, Delegate>? registeredEventHandlersField;

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

                return EventCall.TryGetEventCall(fakeObjectCall, out _);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                if (EventCall.TryGetEventCall(fakeObjectCall, out var eventCall))
                {
                    this.HandleEventCall(eventCall);
                }
            }

            private void HandleEventCall(EventCall eventCall)
            {
                if (eventCall.IsEventRegistration())
                {
                    if (eventCall.TryTakeEventRaiserArgumentProvider(out var argumentProvider))
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
                if (this.RegisteredEventHandlers.TryGetValue(key, out var result))
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
                if (this.RegisteredEventHandlers.TryGetValue(key, out var registration))
                {
                    registration = Delegate.Remove(registration, handler);

                    if (registration is not null)
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
                if (this.RegisteredEventHandlers.TryGetValue(call.Event, out var raiseMethod))
                {
                    var arguments = argumentProvider.GetEventArguments(this.fakeManager.Object!);

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
        }
    }
}
