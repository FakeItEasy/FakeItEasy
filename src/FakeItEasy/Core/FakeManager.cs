namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// The central point in the API for proxied fake objects handles interception
    /// of fake object calls by using a set of rules. User defined rules can be inserted
    /// by using the AddRule-method.
    /// </summary>
    public partial class FakeManager : IFakeCallProcessor
    {
        private static readonly SharedFakeObjectCallRule[] SharedPostUserRules =
        {
            new ObjectMemberRule(),
            new AutoFakePropertyRule(),
            new PropertySetterRule(),
            new CancellationRule(),
            new DefaultReturnValueRule(),
        };

        private readonly EventRule eventRule;
        private readonly LinkedList<IInterceptionListener> interceptionListeners;
        private readonly WeakReference objectReference;

        private readonly LinkedList<CallRuleMetadata> allUserRules;

        private EventCallHandler? eventCallHandler;

        private CallRuleMetadata[]? initialUserRules;

        private ConcurrentQueue<CompletedFakeObjectCall> recordedCalls;

        private int lastSequenceNumber = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeManager"/> class.
        /// </summary>
        /// <param name="fakeObjectType">The faked type.</param>
        /// <param name="proxy">The faked proxy object.</param>
        /// <param name="fakeObjectName">The name of the fake object.</param>
        internal FakeManager(Type fakeObjectType, object proxy, string? fakeObjectName)
        {
            Guard.AgainstNull(fakeObjectType);
            Guard.AgainstNull(proxy);

            this.objectReference = new WeakReference(proxy);
            this.FakeObjectType = fakeObjectType;
            this.FakeObjectName = fakeObjectName;

            this.eventRule = new EventRule(this);
            this.allUserRules = new LinkedList<CallRuleMetadata>();

            this.recordedCalls = new ConcurrentQueue<CompletedFakeObjectCall>();
            this.interceptionListeners = new LinkedList<IInterceptionListener>();
        }

        /// <summary>
        /// A delegate responsible for creating FakeObject instances.
        /// </summary>
        /// <param name="fakeObjectType">The faked type.</param>
        /// <param name="proxy">The faked proxy object.</param>
        /// <param name="fakeObjectName">The name of the fake object.</param>
        /// <returns>An instance of <see cref="FakeManager"/>.</returns>
        internal delegate FakeManager Factory(Type fakeObjectType, object proxy, string? fakeObjectName);

        /// <summary>
        /// Gets the faked proxy object.
        /// </summary>
        /// <remarks>Can be null if the proxy object has been collected by the garbage collector.</remarks>
#pragma warning disable CA1716, CA1720 // Identifier contains keyword, Identifier contains type name
        public virtual object? Object => this.objectReference.Target;
#pragma warning restore CA1716, CA1720 // Identifier contains keyword, Identifier contains type name

        /// <summary>
        /// Gets the faked type.
        /// </summary>
        public virtual Type FakeObjectType { get; }

        /// <summary>
        /// Gets the name of the fake.
        /// </summary>
        public string? FakeObjectName { get; }

        /// <summary>
        /// Gets the interceptions that are currently registered with the fake object.
        /// </summary>
        public virtual IEnumerable<IFakeObjectCallRule> Rules
        {
            get
            {
                lock (this.allUserRules)
                {
                    return this.allUserRules.Select(x => x.Rule).ToList();
                }
            }
        }

        internal string FakeObjectDisplayName =>
            string.IsNullOrEmpty(this.FakeObjectName)
                ? "Faked " + this.FakeObjectType
                : this.FakeObjectName!;

        internal EventCallHandler EventCallHandler => this.eventCallHandler ??= new EventCallHandler(this);

        /// <summary>
        /// Adds a call rule to the fake object.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public virtual void AddRuleFirst(IFakeObjectCallRule rule)
        {
            Guard.AgainstNull(rule);

            lock (this.allUserRules)
            {
                this.allUserRules.AddFirst(CallRuleMetadata.NeverCalled(rule));
            }
        }

        /// <summary>
        /// Adds a call rule last in the list of user rules, meaning it has the lowest priority possible.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public virtual void AddRuleLast(IFakeObjectCallRule rule)
        {
            Guard.AgainstNull(rule);

            lock (this.allUserRules)
            {
                this.allUserRules.AddLast(CallRuleMetadata.NeverCalled(rule));
            }
        }

        /// <summary>
        /// Removes the specified rule for the fake object.
        /// </summary>
        /// <param name="rule">The rule to remove.</param>
        public virtual void RemoveRule(IFakeObjectCallRule rule)
        {
            Guard.AgainstNull(rule);

            lock (this.allUserRules)
            {
                var ruleToRemove = this.allUserRules.FirstOrDefault(x => x.Rule.Equals(rule));
                if (ruleToRemove is not null)
                {
                    this.allUserRules.Remove(ruleToRemove);
                }
            }
        }

        /// <summary>
        /// Adds an interception listener to the manager.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        public void AddInterceptionListener(IInterceptionListener listener)
        {
            Guard.AgainstNull(listener);

            this.interceptionListeners.AddFirst(listener);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Explicit implementation to be able to make the IFakeCallProcessor interface internal.")]
        void IFakeCallProcessor.Process(InterceptedFakeObjectCall fakeObjectCall)
        {
            for (var listenerNode = this.interceptionListeners.First; listenerNode is not null; listenerNode = listenerNode.Next)
            {
                listenerNode.Value.OnBeforeCallIntercepted(fakeObjectCall);
            }

            var callToRecord = fakeObjectCall.ToCompletedCall();
            this.RecordCall(callToRecord);

            try
            {
                this.ApplyBestRule(fakeObjectCall);
            }
            finally
            {
                callToRecord.ReturnValue = fakeObjectCall.ReturnValue;
                for (var listenerNode = this.interceptionListeners.Last; listenerNode is not null; listenerNode = listenerNode.Previous)
                {
                    listenerNode.Value.OnAfterCallIntercepted(callToRecord);
                }
            }
        }

        internal int GetLastRecordedSequenceNumber() => this.lastSequenceNumber;

        /// <summary>
        /// Returns a list of all calls on the managed object.
        /// </summary>
        /// <returns>A list of all calls on the managed object.</returns>
        internal IEnumerable<CompletedFakeObjectCall> GetRecordedCalls()
        {
            return this.recordedCalls;
        }

        /// <summary>
        /// Removes any specified user rules.
        /// </summary>
        internal virtual void ClearUserRules()
        {
            lock (this.allUserRules)
            {
                this.allUserRules.Clear();
            }
        }

        /// <summary>
        /// Restore user rules to those present when the fake was first created.
        /// </summary>
        internal virtual void ResetToInitialUserRules()
        {
            lock (this.allUserRules)
            {
                this.allUserRules.Clear();
                foreach (var rule in this.initialUserRules!)
                {
                    this.allUserRules.AddLast(rule);
                }
            }
        }

        /// <summary>
        /// Removes any recorded calls.
        /// </summary>
        internal virtual void ClearRecordedCalls()
        {
            this.recordedCalls = new ConcurrentQueue<CompletedFakeObjectCall>();
        }

        /// <summary>
        /// Adds a call rule to the fake object after the specified rule.
        /// </summary>
        /// <param name="previousRule">The rule after which to add a rule.</param>
        /// <param name="newRule">The rule to add.</param>
        internal void AddRuleAfter(IFakeObjectCallRule previousRule, IFakeObjectCallRule newRule)
        {
            lock (this.allUserRules)
            {
                var previousNode = this.allUserRules.Nodes().FirstOrDefault(n => n.Value.Rule == previousRule);
                if (previousNode is null)
                {
                    throw new InvalidOperationException(ExceptionMessages.CannotFindPreviousRule);
                }

                this.allUserRules.AddAfter(previousNode, CallRuleMetadata.NeverCalled(newRule));
            }
        }

        /// <summary>
        /// Take a snapshot of the current user rules, recording them as the "initial" user rules.
        /// </summary>
        /// <remarks>
        /// Should only be called once, immediately after the FakeManager has been initialized.
        /// Provides the rules that will be used by <see cref="ResetToInitialUserRules" />.
        /// </remarks>
        internal void SnapshotInitialUserRules()
        {
            lock (this.allUserRules)
            {
                this.initialUserRules = new CallRuleMetadata[this.allUserRules.Count];
                this.allUserRules.CopyTo(this.initialUserRules, 0);
            }
        }

        // Apply the best rule to the call. There will always be at least one applicable rule.
        private void ApplyBestRule(IInterceptedFakeObjectCall fakeObjectCall)
        {
            CallRuleMetadata? bestUserRule = null;
            lock (this.allUserRules)
            {
                foreach (var rule in this.allUserRules)
                {
                    if (rule.Rule.IsApplicableTo(fakeObjectCall) && rule.HasNotBeenCalledSpecifiedNumberOfTimes())
                    {
                        bestUserRule = rule;
                        break;
                    }
                }
            }

            if (bestUserRule is not null)
            {
                bestUserRule.RecordCall();
                bestUserRule.Rule.Apply(fakeObjectCall);
                return;
            }

            if (this.eventRule.IsApplicableTo(fakeObjectCall))
            {
                this.eventRule.Apply(fakeObjectCall);
                return;
            }

            foreach (var postUserRule in SharedPostUserRules)
            {
                if (postUserRule.IsApplicableTo(fakeObjectCall))
                {
                    postUserRule.Apply(fakeObjectCall);
                    return;
                }
            }
        }

        /// <summary>
        /// Provides exclusive access the list of defined user rules so a client can
        /// inspect and optionally modify the list without interfering with concurrent
        /// actions on other threads.
        /// </summary>
        /// <param name="action">An action that can inspect and update the user rules without fear of conflict.</param>
        private void MutateUserRules(Action<LinkedList<CallRuleMetadata>> action)
        {
            lock (this.allUserRules)
            {
                action.Invoke(this.allUserRules);
            }
        }

        /// <summary>
        /// Records that a call has occurred on the managed object.
        /// </summary>
        /// <param name="call">The call to remember.</param>
        private void RecordCall(CompletedFakeObjectCall call)
        {
            this.UpdateLastSequenceNumber(call.SequenceNumber);
            this.recordedCalls.Enqueue(call);
        }

        private void UpdateLastSequenceNumber(int sequenceNumber)
        {
            //// Set the specified sequence number as the last sequence number if it's greater than the current last sequence number.
            //// We use this number in FakeAsserter to separate calls made before the assertion starts from those made during the
            //// assertion.
            //// Because lastSequenceNumber might be changed by another thread after the comparison, we use CompareExchange to
            //// only assign it if it has the same value as the one we compared with. If it's not the case, we retry.

            int last;
            do
            {
                last = this.lastSequenceNumber;
            }
            while (sequenceNumber > last &&
                   sequenceNumber != Interlocked.CompareExchange(ref this.lastSequenceNumber, sequenceNumber, last));
        }

        private abstract class SharedFakeObjectCallRule : IFakeObjectCallRule
        {
            int? IFakeObjectCallRule.NumberOfTimesToCall => null;

            public abstract bool IsApplicableTo(IFakeObjectCall fakeObjectCall);

            public abstract void Apply(IInterceptedFakeObjectCall fakeObjectCall);
        }
    }
}
