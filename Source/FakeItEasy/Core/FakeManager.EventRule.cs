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
        private class EventRule : IFakeObjectCallRule
        {
            [NonSerialized]
            private readonly EventHandlerArgumentProviders argumentProviders =
                ServiceLocator.Current.Resolve<EventHandlerArgumentProviders>();

            private readonly FakeManager fakeManager;

            [NonSerialized]
            private readonly Dictionary<object, Delegate> eventHandlers = new Dictionary<object, Delegate>();

            public EventRule(FakeManager fakeManager)
            {
                Guard.AgainstNull(fakeManager, "fakeManager");

                this.fakeManager = fakeManager;
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                return GetEvent(fakeObjectCall.Method) != null;
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                var eventInfo = GetEvent(fakeObjectCall.Method);
                var eventHandler = (Delegate)fakeObjectCall.Arguments[0];
                if (eventInfo.GetAddMethod() == fakeObjectCall.Method)
                {
                    if (this.argumentProviders.ContainsKey(eventHandler))
                    {
                        this.RaiseEvent(eventInfo, eventHandler);
                    }
                    else
                    {
                        this.AddHandler(eventInfo, eventHandler);
                    }
                }
                else
                {
                    this.RemoveHandler(eventInfo, eventHandler);
                }
            }

            private static EventInfo GetEvent(MethodInfo method)
            {
                var baseDefinition = method.GetBaseDefinition();
                return
                    method.DeclaringType.GetEvents().SingleOrDefault(e =>
                        e.GetAddMethod().GetBaseDefinition() == baseDefinition ||
                        e.GetRemoveMethod().GetBaseDefinition() == baseDefinition);
            }

            private void AddHandler(object key, Delegate handler)
            {
                Delegate result = this.eventHandlers.TryGetValue(key, out result)
                    ? Delegate.Combine(result, handler)
                    : handler;

                this.eventHandlers[key] = result;
            }

            private void RemoveHandler(object key, Delegate handler)
            {
                Delegate registration;
                if (this.eventHandlers.TryGetValue(key, out registration))
                {
                    registration = Delegate.Remove(registration, handler);

                    if (registration != null)
                    {
                        this.eventHandlers[key] = registration;
                    }
                    else
                    {
                        this.eventHandlers.Remove(key);
                    }
                }
            }

            private void RaiseEvent(EventInfo eventInfo, Delegate eventHandler)
            {
                Delegate raiseMethod;
                if (this.eventHandlers.TryGetValue(eventInfo, out raiseMethod))
                {
                    IArgumentProvider provider;
                    this.argumentProviders.TryRemove(eventHandler, out provider);
                    var arguments = provider.GetArguments(this.fakeManager.Object);

                    try
                    {
                        raiseMethod.DynamicInvoke(arguments);
                    }
                    catch (TargetInvocationException ex)
                    {
                        // Exceptions thrown by event handlers should propagate outward as is, not
                        // be wrapped in a TargetInvocationException.
                        if (ex.InnerException != null)
                        {
                            ex.InnerException.TryPreserveStackTrace();
                            throw ex.InnerException;
                        }

                        throw;
                    }
                }
            }
        }
    }
}