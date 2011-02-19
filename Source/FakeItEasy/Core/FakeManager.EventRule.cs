namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    public partial class FakeManager
    {
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Would provide no benefit since there is no place from where to call the Dispose-method.")]
        private class EventRule
            : IFakeObjectCallRule
        {
            [NonSerialized] private Dictionary<object, Delegate> registeredEventHandlersField;

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
                return EventCall.GetEvent(fakeObjectCall.Method) != null;
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
                    result = Delegate.Combine(handler, handler);
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
                    this.RegisteredEventHandlers[key] = registration;
                }
            }

            private void RaiseEvent(EventCall call)
            {
                var raiseMethod = this.RegisteredEventHandlers[call.Event];

                if (raiseMethod != null)
                {
                    var arguments = call.EventHandler.Target as IEventRaiserArguments;

                    var sender = arguments.Sender ?? this.FakeManager.Object;

                    raiseMethod.DynamicInvoke(sender, arguments.EventArguments);
                }
            }

            private class EventCall
            {
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
                         where Equals(e.GetAddMethod().GetBaseDefinition(), eventAdderOrRemover.GetBaseDefinition())
                             || Equals(e.GetRemoveMethod().GetBaseDefinition(), eventAdderOrRemover.GetBaseDefinition())
                         select e).SingleOrDefault();
                }

                public bool IsEventRaiser()
                {
                    var declaringType = this.EventHandler.Method.DeclaringType;

                    return declaringType.IsGenericType
                        && declaringType.GetGenericTypeDefinition() == typeof(Raise<>)
                            && this.EventHandler.Method.Name.Equals("Now");
                }

                public bool IsEventRegistration()
                {
                    return this.Event.GetAddMethod().Equals(this.CallingMethod);
                }
            }
        }
    }
}