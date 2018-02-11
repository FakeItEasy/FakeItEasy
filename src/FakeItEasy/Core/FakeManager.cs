namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// The central point in the API for proxied fake objects handles interception
    /// of fake object calls by using a set of rules. User defined rules can be inserted
    /// by using the AddRule-method.
    /// </summary>
#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    public partial class FakeManager : IFakeCallProcessor
    {
        private readonly CallRuleMetadata[] postUserRules;
        private readonly CallRuleMetadata[] preUserRules;
#pragma warning disable CA2235 // Mark all non-serializable fields
        private readonly LinkedList<IInterceptionListener> interceptionListeners;
        private readonly WeakReference objectReference;

        private ConcurrentQueue<ICompletedFakeObjectCall> recordedCalls;
#pragma warning restore CA2235 // Mark all non-serializable fields

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeManager"/> class.
        /// </summary>
        /// <param name="fakeObjectType">The faked type.</param>
        /// <param name="proxy">The faked proxy object.</param>
        /// <param name="fakeManagerAccessor">The fake manager accessor.</param>
        internal FakeManager(Type fakeObjectType, object proxy, IFakeManagerAccessor fakeManagerAccessor)
        {
            Guard.AgainstNull(fakeObjectType, nameof(fakeObjectType));
            Guard.AgainstNull(proxy, nameof(proxy));

            this.objectReference = new WeakReference(proxy);
            this.FakeObjectType = fakeObjectType;

            this.preUserRules = new[]
                                    {
                                        new CallRuleMetadata { Rule = new EventRule(this) }
                                    };
            this.AllUserRules = new LinkedList<CallRuleMetadata>();
            this.postUserRules = new[]
                                     {
                                         new CallRuleMetadata { Rule = new ObjectMemberRule(this, fakeManagerAccessor) },
                                         new CallRuleMetadata { Rule = new AutoFakePropertyRule(this) },
                                         new CallRuleMetadata { Rule = new PropertySetterRule(this) },
                                         new CallRuleMetadata { Rule = new CancellationRule() },
                                         new CallRuleMetadata { Rule = new DefaultReturnValueRule() }
                                     };

            this.recordedCalls = new ConcurrentQueue<ICompletedFakeObjectCall>();
            this.interceptionListeners = new LinkedList<IInterceptionListener>();
        }

        /// <summary>
        /// A delegate responsible for creating FakeObject instances.
        /// </summary>
        /// <param name="fakeObjectType">The faked type.</param>
        /// <param name="proxy">The faked proxy object.</param>
        /// <returns>An instance of <see cref="FakeManager"/>.</returns>
        internal delegate FakeManager Factory(Type fakeObjectType, object proxy);

        /// <summary>
        /// Gets the faked proxy object.
        /// </summary>
#pragma warning disable CA1716, CA1720 // Identifier contains keyword, Identifier contains type name
        public virtual object Object => this.objectReference.Target;
#pragma warning restore CA1716, CA1720 // Identifier contains keyword, Identifier contains type name

        /// <summary>
        /// Gets the faked type.
        /// </summary>
#pragma warning disable CA2235 // Mark all non-serializable fields
        public virtual Type FakeObjectType { get; }
#pragma warning restore CA2235 // Mark all non-serializable fields

        /// <summary>
        /// Gets the interceptions that are currently registered with the fake object.
        /// </summary>
        public virtual IEnumerable<IFakeObjectCallRule> Rules
        {
            get { return this.AllUserRules.Select(x => x.Rule); }
        }

#pragma warning disable CA2235 // Mark all non-serializable fields
        internal LinkedList<CallRuleMetadata> AllUserRules { get; }
#pragma warning restore CA2235 // Mark all non-serializable fields

        private IEnumerable<CallRuleMetadata> AllRules =>
            this.preUserRules.Concat(this.AllUserRules.Concat(this.postUserRules));

        /// <summary>
        /// Adds a call rule to the fake object.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public virtual void AddRuleFirst(IFakeObjectCallRule rule)
        {
            this.AllUserRules.AddFirst(new CallRuleMetadata { Rule = rule });
        }

        /// <summary>
        /// Adds a call rule last in the list of user rules, meaning it has the lowest priority possible.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public virtual void AddRuleLast(IFakeObjectCallRule rule)
        {
            this.AllUserRules.AddLast(new CallRuleMetadata { Rule = rule });
        }

        /// <summary>
        /// Removes the specified rule for the fake object.
        /// </summary>
        /// <param name="rule">The rule to remove.</param>
        public virtual void RemoveRule(IFakeObjectCallRule rule)
        {
            Guard.AgainstNull(rule, nameof(rule));

            var ruleToRemove = this.AllUserRules.FirstOrDefault(x => x.Rule.Equals(rule));
            this.AllUserRules.Remove(ruleToRemove);
        }

        /// <summary>
        /// Adds an interception listener to the manager.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddInterceptionListener(IInterceptionListener listener)
        {
            this.interceptionListeners.AddFirst(listener);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Explicit implementation to be able to make the IFakeCallProcessor interface internal.")]
        void IFakeCallProcessor.Process(IInterceptedFakeObjectCall fakeObjectCall)
        {
            foreach (var listener in this.interceptionListeners)
            {
                listener.OnBeforeCallIntercepted(fakeObjectCall);
            }

            var ruleToUse =
                (from rule in this.AllRules
                 where rule.Rule.IsApplicableTo(fakeObjectCall) && rule.HasNotBeenCalledSpecifiedNumberOfTimes()
                 select rule).First();

            try
            {
                ApplyRule(ruleToUse, fakeObjectCall);
            }
            finally
            {
                var readonlyCall = fakeObjectCall.AsReadOnly();

                this.RecordCall(readonlyCall);

                foreach (var listener in this.interceptionListeners.Reverse())
                {
                    listener.OnAfterCallIntercepted(readonlyCall, ruleToUse.Rule);
                }
            }
        }

        /// <summary>
        /// Returns a list of all calls on the managed object.
        /// </summary>
        /// <returns>A list of all calls on the managed object.</returns>
        internal IEnumerable<ICompletedFakeObjectCall> GetRecordedCalls()
        {
            return this.recordedCalls;
        }

        /// <summary>
        /// Records that a call has occurred on the managed object.
        /// </summary>
        /// <param name="call">The call to remember.</param>
        internal void RecordCall(ICompletedFakeObjectCall call)
        {
            SequenceNumberManager.RecordSequenceNumber(call);
            this.recordedCalls.Enqueue(call);
        }

        /// <summary>
        /// Removes any specified user rules.
        /// </summary>
        internal virtual void ClearUserRules()
        {
            this.AllUserRules.Clear();
        }

        /// <summary>
        /// Removes any recorded calls.
        /// </summary>
        internal virtual void ClearRecordedCalls()
        {
            this.recordedCalls = new ConcurrentQueue<ICompletedFakeObjectCall>();
        }

        /// <summary>
        /// Adds a call rule to the fake object after the specified rule.
        /// </summary>
        /// <param name="previousRule">The rule after which to add a rule.</param>
        /// <param name="newRule">The rule to add.</param>
        internal void AddRuleAfter(IFakeObjectCallRule previousRule, IFakeObjectCallRule newRule)
        {
            var previousNode = this.AllUserRules.Nodes().FirstOrDefault(n => n.Value.Rule == previousRule);
            if (previousNode == null)
            {
                throw new InvalidOperationException("The rule after which to add the new rule was not found in the list.");
            }

            this.AllUserRules.AddAfter(previousNode, new CallRuleMetadata { Rule = newRule });
        }

        private static void ApplyRule(CallRuleMetadata rule, IInterceptedFakeObjectCall fakeObjectCall)
        {
            rule.CalledNumberOfTimes++;
            rule.Rule.Apply(fakeObjectCall);
        }
    }
}
