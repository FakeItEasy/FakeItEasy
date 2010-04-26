namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Represents a scope for fake objects, calls configured within a scope
    /// are only valid within that scope. Only calls made wihtin a scope
    /// are accessible from within a scope so for example asserts will only
    /// assert on those calls done within the scope.
    /// </summary>
    internal abstract class FakeScope
        : IDisposable
    {
        #region Construction
        static FakeScope()
        {
            FakeScope.Current = new RootScope();
        }

        private FakeScope()
        {
        }
        #endregion

        #region Properties
        internal static FakeScope Current { get; set; }

        internal abstract IFakeObjectContainer FakeObjectContainer { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new scope and sets it as the current scope.
        /// </summary>
        /// <returns>The created scope.</returns>
        public static FakeScope Create()
        {
            return Create(FakeScope.Current.FakeObjectContainer);
        }

        /// <summary>
        /// Creates a new scope and sets it as the current scope, using the specified
        /// container as the container for the new scope.
        /// </summary>
        /// <param name="container">The container to usee for the new scope.</param>
        /// <returns>The created scope.</returns>
        public static FakeScope Create(IFakeObjectContainer container)
        {
            var result = new ChildScope(FakeScope.Current, container);
            FakeScope.Current = result;
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

        /// <summary>
        /// Adds an intercepted call to the current scope.
        /// </summary>
        /// <param name="fakeObject">The fake object.</param>
        /// <param name="call">The call that is intercepted.</param>
        internal void AddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call)
        {
            fakeObject.AllRecordedCalls.Add(call);
            this.OnAddInterceptedCall(fakeObject, call);
        }

        /// <summary>
        /// Adds a fake object call to the current scope.
        /// </summary>
        /// <param name="fakeObject">The fake object.</param>
        /// <param name="rule">The rule to add.</param>
        internal void AddRuleFirst(FakeObject fakeObject, CallRuleMetadata rule)
        {
            fakeObject.AllUserRules.AddFirst(rule);
            this.OnAddRule(fakeObject, rule);
        }

        internal void AddRuleLast(FakeObject fakeObject, CallRuleMetadata rule)
        {
            fakeObject.AllUserRules.AddLast(rule);
            this.OnAddRule(fakeObject, rule);
        }

        internal abstract IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeObject fakeObject);

        protected abstract void OnDispose();

        protected abstract void OnAddRule(FakeObject fakeObject, CallRuleMetadata rule);

        protected abstract void OnAddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call);
        #endregion

        #region Nested types
        private class RootScope
            : FakeScope
        {
            private IFakeObjectContainer fakeObjectContainerField;

            public RootScope()
            {
                this.fakeObjectContainerField = new DynamicContainer();
            }

            internal override IFakeObjectContainer FakeObjectContainer
            {
                get
                {
                    return this.fakeObjectContainerField;
                }
            }

            internal override IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeObject fakeObject)
            {
                return fakeObject.AllRecordedCalls;
            }

            protected override void OnAddRule(FakeObject fakeObject, CallRuleMetadata rule)
            {
                // Do nothing
            }

            protected override void OnDispose()
            {
                // Do nothing
            }

            protected override void OnAddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call)
            {
                // Do nothing
            }
        }

        private class ChildScope
            : FakeScope
        {
            private FakeScope parentScope;
            private Dictionary<FakeObject, List<CallRuleMetadata>> rulesField;
            private Dictionary<FakeObject, List<ICompletedFakeObjectCall>> recordedCalls;
            private IFakeObjectContainer fakeObjectContainerField;

            public ChildScope(FakeScope parentScope, IFakeObjectContainer container)
            {
                this.parentScope = parentScope;
                this.rulesField = new Dictionary<FakeObject, List<CallRuleMetadata>>();
                this.recordedCalls = new Dictionary<FakeObject, List<ICompletedFakeObjectCall>>();
                this.fakeObjectContainerField = container;
            }

            internal override IFakeObjectContainer FakeObjectContainer
            {
                get { return this.fakeObjectContainerField; }
            }

            internal override IEnumerable<ICompletedFakeObjectCall> GetCallsWithinScope(FakeObject fakeObject)
            {
                List<ICompletedFakeObjectCall> calls;

                if (!this.recordedCalls.TryGetValue(fakeObject, out calls))
                {
                    calls = new List<ICompletedFakeObjectCall>();
                }

                return calls;
            }

            protected override void OnAddRule(FakeObject fakeObject, CallRuleMetadata rule)
            {
                List<CallRuleMetadata> rules;

                if (!this.rulesField.TryGetValue(fakeObject, out rules))
                {
                    rules = new List<CallRuleMetadata>();
                    this.rulesField.Add(fakeObject, rules);
                }

                rules.Add(rule);
            }

            protected override void OnDispose()
            {
                this.RemoveRulesConfiguredInScope();
                FakeScope.Current = this.parentScope;
            }

            protected override void OnAddInterceptedCall(FakeObject fakeObject, ICompletedFakeObjectCall call)
            {
                this.parentScope.OnAddInterceptedCall(fakeObject, call);

                List<ICompletedFakeObjectCall> calls;

                if (!this.recordedCalls.TryGetValue(fakeObject, out calls))
                {
                    calls = new List<ICompletedFakeObjectCall>();
                    this.recordedCalls.Add(fakeObject, calls);
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
        #endregion
    }   
}
