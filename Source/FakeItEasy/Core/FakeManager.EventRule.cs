namespace FakeItEasy.Core
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public partial class FakeManager
    {
        [Serializable]
        private class EventRule
            : IFakeObjectCallRule
        {
            [NonSerialized]
            private EventHandlerList registeredEventHandlersField;

            public FakeManager FakeManager { get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            private EventHandlerList RegisteredEventHandlers
            {
                get
                {
                    if (this.registeredEventHandlersField == null)
                    {
                        this.registeredEventHandlersField = new EventHandlerList();
                    }

                    return this.registeredEventHandlersField;
                }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return EventCall.GetEvent(fakeObjectCall.Method) != null;
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                EventCall eventCall = EventCall.GetEventCall(fakeObjectCall);

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
                this.RegisteredEventHandlers.RemoveHandler(call.Event, call.EventHandler);
            }

            private void AddEventListener(EventCall call)
            {
                this.RegisteredEventHandlers.AddHandler(call.Event, call.EventHandler);
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
                         where object.Equals(e.GetAddMethod().GetBaseDefinition(), eventAdderOrRemover.GetBaseDefinition())
                              || object.Equals(e.GetRemoveMethod().GetBaseDefinition(), eventAdderOrRemover.GetBaseDefinition())
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
