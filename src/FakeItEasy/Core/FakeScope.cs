namespace FakeItEasy.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a scope for fake objects, calls configured within a scope
    /// are only valid within that scope. Only calls made within a scope
    /// are accessible from within a scope so for example asserts will only
    /// assert on those calls done within the scope.
    /// </summary>
    internal abstract class FakeScope
        : IFakeScope
    {
        static FakeScope()
        {
            Current = new RootScope();
        }

        private FakeScope()
        {
        }

        internal static FakeScope Current { get; set; }

        /// <summary>
        /// Creates a new scope and sets it as the current scope.
        /// </summary>
        /// <returns>The created scope.</returns>
        public static IFakeScope Create()
        {
            var result = new ChildScope(Current);
            Current = result;
            return result;
        }

        /// <summary>
        /// Closes the scope.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.OnDispose();
        }

        public abstract IEnumerator<ICompletedFakeObjectCall> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds an intercepted call to the current scope.
        /// </summary>
        /// <param name="fakeManager">The fake object.</param>
        /// <param name="call">The call that is intercepted.</param>
        internal void AddInterceptedCall(FakeManager fakeManager, ICompletedFakeObjectCall call)
        {
            fakeManager.AllRecordedCalls.Add(call);
            this.OnAddInterceptedCall(fakeManager, call);
        }

        /// <summary>
        /// Adds a fake object call to the current scope.
        /// </summary>
        /// <param name="fakeManager">The fake object.</param>
        /// <param name="rule">The rule to add.</param>
        internal void AddRuleFirst(FakeManager fakeManager, CallRuleMetadata rule)
        {
            fakeManager.AllUserRules.AddFirst(rule);
            this.OnAddRule(fakeManager, rule);
        }

        internal void AddRuleLast(FakeManager fakeManager, CallRuleMetadata rule)
        {
            fakeManager.AllUserRules.AddLast(rule);
            this.OnAddRule(fakeManager, rule);
        }

        internal abstract IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeManager fakeObject);

        protected abstract void OnDispose();

        protected abstract void OnAddRule(FakeManager fakeObject, CallRuleMetadata rule);

        protected abstract void OnAddInterceptedCall(FakeManager fakeObject, ICompletedFakeObjectCall call);

        private class ChildScope
            : FakeScope
        {
            private readonly FakeScope parentScope;
            private readonly LinkedList<ICompletedFakeObjectCall> recordedCalls;
            private readonly Dictionary<FakeManager, IList<ICompletedFakeObjectCall>> recordedCallsGroupedByFakeManager;
            private readonly Dictionary<FakeManager, IList<CallRuleMetadata>> rulesField;

            public ChildScope(FakeScope parentScope)
            {
                this.parentScope = parentScope;
                this.rulesField = new Dictionary<FakeManager, IList<CallRuleMetadata>>();
                this.recordedCallsGroupedByFakeManager = new Dictionary<FakeManager, IList<ICompletedFakeObjectCall>>();
                this.recordedCalls = new LinkedList<ICompletedFakeObjectCall>();
            }

            public override IEnumerator<ICompletedFakeObjectCall> GetEnumerator()
            {
                return this.recordedCalls.GetEnumerator();
            }

            internal override IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeManager fakeObject)
            {
                IList<ICompletedFakeObjectCall> calls;

                if (!this.recordedCallsGroupedByFakeManager.TryGetValue(fakeObject, out calls))
                {
                    calls = new SynchronizedCollection<ICompletedFakeObjectCall>();
                }

                return calls;
            }

            protected override void OnAddRule(FakeManager fakeObject, CallRuleMetadata rule)
            {
                IList<CallRuleMetadata> rules;

                if (!this.rulesField.TryGetValue(fakeObject, out rules))
                {
                    rules = new SynchronizedCollection<CallRuleMetadata>();
                    this.rulesField.Add(fakeObject, rules);
                }

                rules.Add(rule);
            }

            protected override void OnDispose()
            {
                this.RemoveRulesConfiguredInScope();
                FakeScope.Current = this.parentScope;
            }

            protected override void OnAddInterceptedCall(FakeManager fakeManager, ICompletedFakeObjectCall call)
            {
                this.parentScope.OnAddInterceptedCall(fakeManager, call);

                this.recordedCalls.AddLast(call);

                IList<ICompletedFakeObjectCall> calls;

                if (!this.recordedCallsGroupedByFakeManager.TryGetValue(fakeManager, out calls))
                {
                    calls = new SynchronizedCollection<ICompletedFakeObjectCall>();
                    this.recordedCallsGroupedByFakeManager.Add(fakeManager, calls);
                }

                calls.Add(call);
            }

            private void RemoveRulesConfiguredInScope()
            {
                foreach (var objectRules in this.rulesField)
                {
                    foreach (var rule in objectRules.Value)
                    {
                        objectRules.Key.AllUserRules.Remove(rule);
                    }
                }
            }
        }

        private class RootScope
            : FakeScope
        {
            public override IEnumerator<ICompletedFakeObjectCall> GetEnumerator()
            {
                throw new NotSupportedException();
            }

            internal override IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeManager fakeObject)
            {
                return fakeObject.AllRecordedCalls;
            }

            protected override void OnAddRule(FakeManager fakeObject, CallRuleMetadata rule)
            {
            }

            protected override void OnDispose()
            {
            }

            protected override void OnAddInterceptedCall(FakeManager fakeObject, ICompletedFakeObjectCall call)
            {
            }
        }
    }
}
