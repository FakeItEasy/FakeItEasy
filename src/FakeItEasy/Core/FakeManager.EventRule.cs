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
#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Would provide no benefit since there is no place from where to call the Dispose-method.")]
        private class EventRule
            : IFakeObjectCallRule
        {
            private readonly FakeManager fakeManager;

#if FEATURE_BINARY_SERIALIZATION
            [NonSerialized]
#endif
            private readonly EventHandlerArgumentProviderMap eventHandlerArgumentProviderMap =
                ServiceLocator.Current.Resolve<EventHandlerArgumentProviderMap>();

#if FEATURE_BINARY_SERIALIZATION
            [NonSerialized]
#endif
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
                    if (this.registeredEventHandlersField == null)
                    {
                        this.registeredEventHandlersField = new Dictionary<object, Delegate>();
                    }

                    return this.registeredEventHandlersField;
                }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                return EventCall.GetEvent(fakeObjectCall.Method) != null;
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                var eventCall = EventCall.GetEventCall(fakeObjectCall);

                this.HandleEventCall(eventCall);
            }

            // Attempts to preserve the stack trace of an existing exception when re-throw via throw or throw ex.
            // Nicked from
            // http://weblogs.asp.net/fmarguerie/archive/2008/01/02/rethrowing-exceptions-and-preserving-the-full-call-stack-trace.aspx.
            // If reduced trust context (for example) precludes
            // invoking internal members on Exception, the stack trace will not be preserved.
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try method.")]
            private static void TryPreserveStackTrace(Exception exception)
            {
                try
                {
                    var preserveStackTrace = typeof(Exception).GetMethod(
                                                                    "InternalPreserveStackTrace",
                                                                    BindingFlags.Instance | BindingFlags.NonPublic);
                    preserveStackTrace.Invoke(exception, null);
                }
                catch
                {
                }
            }

            private void HandleEventCall(EventCall eventCall)
            {
                if (eventCall.IsEventRegistration())
                {
                    IEventRaiserArgumentProvider argumentProvider;
                    if (this.eventHandlerArgumentProviderMap.TryTakeArgumentProviderFor(
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

                    if (registration != null)
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
                        if (ex.InnerException != null)
                        {
                            // Exceptions thrown by event handlers should propagate outward as is, not
                            // be wrapped in a TargetInvocationException.
                            TryPreserveStackTrace(ex.InnerException);
                            throw ex.InnerException;
                        }

                        throw;
                    }
                }
            }

            private class EventCall
            {
                private EventCall()
                {
                }

                public EventInfo Event { get; private set; }

                public Delegate EventHandler { get; private set; }

                private MethodInfo CallingMethod { get; set; }

                public static EventCall GetEventCall(
                    IFakeObjectCall fakeObjectCall)
                {
                    var eventInfo = GetEvent(fakeObjectCall.Method);

                    return new EventCall
                               {
                                   Event = eventInfo,
                                   CallingMethod = fakeObjectCall.Method,
                                   EventHandler = (Delegate)fakeObjectCall.Arguments[0],
                               };
                }

                public static EventInfo GetEvent(MethodInfo eventAdderOrRemover)
                {
                    return
                        (from e in eventAdderOrRemover.DeclaringType.GetEvents()
                         where object.Equals(e.GetAddMethod().GetBaseDefinition(), eventAdderOrRemover.GetBaseDefinition())
                             || object.Equals(e.GetRemoveMethod().GetBaseDefinition(), eventAdderOrRemover.GetBaseDefinition())
                         select e).SingleOrDefault();
                }

                public bool IsEventRegistration()
                {
                    return this.Event.GetAddMethod().Equals(this.CallingMethod);
                }
            }
        }
    }
}
