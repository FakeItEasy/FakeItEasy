namespace FakeItEasy.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal class EventCallHandler
{
    private readonly FakeManager fakeManager;

    private readonly Dictionary<object, Delegate> registeredEventHandlers;

    public EventCallHandler(FakeManager fakeManager)
        : this(fakeManager, new Dictionary<object, Delegate>())
    {
    }

    private EventCallHandler(FakeManager fakeManager, Dictionary<object, Delegate> registeredEventHandlers)
    {
        this.fakeManager = fakeManager;
        this.registeredEventHandlers = registeredEventHandlers;
    }

    public void HandleEventCall(EventCall eventCall)
    {
        if (eventCall.IsEventSubscription())
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

    public EventCallHandler GetSnapshot()
    {
        var registeredHandlers = new Dictionary<object, Delegate>(this.registeredEventHandlers);
        return new EventCallHandler(this.fakeManager, registeredHandlers);
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
        if (this.registeredEventHandlers.TryGetValue(key, out var result))
        {
            result = Delegate.Combine(result, handler);
        }
        else
        {
            result = handler;
        }

        this.registeredEventHandlers[key] = result;
    }

    private void RemoveHandler(object key, Delegate handler)
    {
        if (this.registeredEventHandlers.TryGetValue(key, out var registration))
        {
            registration = Delegate.Remove(registration, handler);

            if (registration is not null)
            {
                this.registeredEventHandlers[key] = registration;
            }
            else
            {
                this.registeredEventHandlers.Remove(key);
            }
        }
    }

    private void RaiseEvent(EventCall call, IEventRaiserArgumentProvider argumentProvider)
    {
        if (this.registeredEventHandlers.TryGetValue(call.Event, out var raiseMethod))
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
