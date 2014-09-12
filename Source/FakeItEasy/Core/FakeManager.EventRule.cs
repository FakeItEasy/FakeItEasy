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
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Would provide no benefit since there is no place from where to call the Dispose-method.")]
        private class EventRule
            : IFakeObjectCallRule
        {
            [NonSerialized]
            private readonly EventHandlerArgumentProviderMap eventHandlerArgumentProviderMap =
                ServiceLocator.Current.Resolve<EventHandlerArgumentProviderMap>();

            [NonSerialized]
            private Dictionary<object, Delegate> registeredEventHandlersField;

            public FakeManager FakeManager { get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

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
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                return EventCall.GetEvent(fakeObjectCall.Method) != null;
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                var eventCall = EventCall.GetEventCall(fakeObjectCall);

                this.HandleEventCall(eventCall);
            }

            /// <summary>
            /// Attempts to preserve the stack trace of an existing exception when rethrown via <c>throw</c> or <c>throw ex</c>.
            /// </summary>
            /// <remarks>Nicked from
            /// http://weblogs.asp.net/fmarguerie/archive/2008/01/02/rethrowing-exceptions-and-preserving-the-full-call-stack-trace.aspx.
            /// If reduced trust context (for example) precludes
            /// invoking internal members on <see cref="Exception"/>, the stack trace will not be preserved.
            /// </remarks>
            /// <param name="exception">The exception whose stack trace needs preserving.</param>
            [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "URL in remarks.")]
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
                    if (eventCall.IsEventRaiser())
                    {
                        this.RaiseEvent(eventCall);
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
                Delegate result = null;

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
                Delegate registration = null;

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

            private void RaiseEvent(EventCall call)
            {
                Delegate raiseMethod = null;

                if (this.RegisteredEventHandlers.TryGetValue(call.Event, out raiseMethod))
                {
                    var arguments = new List<object>();

                    if (!this.CanFillArgumentsFromImplicitEventRaiser(call, arguments))
                    {
                        this.FillArgumentsFromExplicitEventRaiser(call, arguments);
                    }

                    try
                    {
                        raiseMethod.DynamicInvoke(arguments.ToArray());
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

            private bool CanFillArgumentsFromImplicitEventRaiser(EventCall call, List<object> arguments)
            {
                Func<object, object[]> argumentListBuilder;

                if (this.eventHandlerArgumentProviderMap.TryGetArgumentProviderFor(call.EventHandler, out argumentListBuilder))
                {
                    arguments.AddRange(argumentListBuilder(this.FakeManager.Object));
                    return true;
                }

                return false;
            }

            private void FillArgumentsFromExplicitEventRaiser(EventCall call, List<object> arguments)
            {
                var eventRaiserArguments = call.EventHandler.Target as IEventRaiserArguments;

                if (eventRaiserArguments == null)
                {
                    return;
                }

                var sender = eventRaiserArguments.Sender ?? this.FakeManager.Object;
                arguments.Add(sender);
                arguments.Add(eventRaiserArguments.EventArguments);
            }

            private class EventCall
            {
                private EventHandlerArgumentProviderMap eventHandlerArgumentProviderMap =
                               ServiceLocator.Current.Resolve<EventHandlerArgumentProviderMap>();

                private EventCall()
                {
                }

                public EventInfo Event { get; set; }

                public MethodInfo CallingMethod { get; set; }

                public Delegate EventHandler { get; set; }

                public static EventCall GetEventCall(IFakeObjectCall fakeObjectCall)
                {
                    var eventInfo = GetEvent(fakeObjectCall.Method);

                    return new EventCall
                               {
                                   Event = eventInfo, 
                                   CallingMethod = fakeObjectCall.Method, 
                                   EventHandler = (Delegate)fakeObjectCall.Arguments[0]
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

                public bool IsEventRaiser()
                {
                    return this.IsImplicitEventRaiser() || this.IsExplicitEventRaiserUsingNow();
                }

                public bool IsEventRegistration()
                {
                    return this.Event.GetAddMethod().Equals(this.CallingMethod);
                }

                private bool IsExplicitEventRaiserUsingNow()
                {
                    var declaringType = this.EventHandler.Method.DeclaringType;

                    return declaringType != null
                           && declaringType.IsGenericType
                           && declaringType.GetGenericTypeDefinition() == typeof(Raise<>)
                           && this.EventHandler.Method.Name.Equals("Now");
                }

                private bool IsImplicitEventRaiser()
                {
                    return this.eventHandlerArgumentProviderMap.Contains(this.EventHandler);
                }
            }
        }
    }
}