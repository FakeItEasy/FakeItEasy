namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core.Creation;

    /// <summary>
    /// The central point in the API for proxied fake objects handles interception
    /// of fake object calls by using a set of rules. User defined rules can be inserted
    /// by using the AddRule-method.
    /// </summary>
    [Serializable]
    public partial class FakeObject
    {
        private IEnumerable<CallRuleMetadata> preUserRules;
        private LinkedList<CallRuleMetadata> allUserRulesField;
        private IEnumerable<CallRuleMetadata> postUserRules;
        private List<ICompletedFakeObjectCall> recordedCallsField;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeObject"/> class.
        /// </summary>
        public FakeObject()
        {
            this.preUserRules = new[] 
            {
                new CallRuleMetadata { Rule = new EventRule { FakeObject = this } } 
            };
            this.allUserRulesField = new LinkedList<CallRuleMetadata>();
            this.postUserRules = new[] 
            { 
                new CallRuleMetadata { Rule = new ObjectMemberRule { FakeObject = this } },
                new CallRuleMetadata { Rule = new AutoFakePropertyRule { FakeObject = this } },
                new CallRuleMetadata { Rule = new PropertySetterRule { FakeObject = this } },
                new CallRuleMetadata { Rule = new DefaultReturnValueRule() }
            };

            this.recordedCallsField = new List<ICompletedFakeObjectCall>();
        }

        /// <summary>
        /// A delegate responsible for creating FakeObject instances.
        /// </summary>
        /// <returns></returns>
        public delegate FakeObject Factory();

        /// <summary>
        /// Gets the faked object.
        /// </summary>
        public virtual object Object
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the faked type.
        /// </summary>
        public virtual Type FakeObjectType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the interceptions that are currently registered with the fake object.
        /// </summary>
        public virtual IEnumerable<IFakeObjectCallRule> Rules
        {
            get
            {
                return this.allUserRulesField.Select(x => x.Rule);
            }
        }

        /// <summary>
        /// Gets a collection of all the calls made to the fake object within the current scope.
        /// </summary>
        public virtual IEnumerable<ICompletedFakeObjectCall> RecordedCallsInScope
        {
            get
            {
                return FakeScope.Current.GetCallsWithinScope(this);
            }
        }

        internal List<ICompletedFakeObjectCall> AllRecordedCalls
        {
            get
            {
                return this.recordedCallsField;
            }
        }

        internal LinkedList<CallRuleMetadata> AllUserRules
        {
            get
            {
                return this.allUserRulesField;
            }
        }

        private IEnumerable<CallRuleMetadata> AllRules
        { 
            get
            {
                return this.preUserRules.Concat(this.AllUserRules.Concat(this.postUserRules));
            }
        }

        /// <summary>
        /// Adds a call rule to the fake object.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public virtual void AddRuleFirst(IFakeObjectCallRule rule)
        {
            var newRule = new CallRuleMetadata { Rule = rule };
            FakeScope.Current.AddRuleFirst(this, newRule);
        }

        /// <summary>
        /// Adds a call rule last in the list of user rules, meaning it has the lowest priority possible.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public virtual void AddRuleLast(IFakeObjectCallRule rule)
        {
            var newRule = new CallRuleMetadata { Rule = rule };
            FakeScope.Current.AddRuleLast(this, newRule);
        }

        /// <summary>
        /// Removes the specified rule for the fake object.
        /// </summary>
        /// <param name="rule">The rule to remove.</param>
        public virtual void RemoveRule(IFakeObjectCallRule rule)
        {
            Guard.AgainstNull(rule, "rule");

            var ruleToRemove = this.AllUserRules.Where(x => x.Rule.Equals(rule)).FirstOrDefault();
            this.AllUserRules.Remove(ruleToRemove);
        }
        
        internal virtual void SetProxy(ProxyResult proxy)
        {
            this.Object = proxy.Proxy;
            this.FakeObjectType = proxy.ProxiedType;
            proxy.CallWasIntercepted += this.Proxy_CallWasIntercepted;
        }

        private static void ApplyRule(CallRuleMetadata rule, IInterceptedFakeObjectCall fakeObjectCall)
        {
            rule.CalledNumberOfTimes++;
            rule.Rule.Apply(fakeObjectCall);
        }
        
        private void Intercept(IWritableFakeObjectCall fakeObjectCall)
        {
            FakeScope.Current.AddInterceptedCall(this, fakeObjectCall.AsReadOnly());

            var ruleToUse =
                (from rule in this.AllRules
                 where rule.Rule.IsApplicableTo(fakeObjectCall) && rule.HasNotBeenCalledSpecifiedNumberOfTimes()
                 select rule).First();

            ApplyRule(ruleToUse, new InterceptedCallAdapter(fakeObjectCall));
        }

        private void MoveRuleToFront(CallRuleMetadata rule)
        {
            if (this.allUserRulesField.Remove(rule))
            {
                this.allUserRulesField.AddFirst(rule);
            }
        }

        private void MoveRuleToFront(IFakeObjectCallRule rule)
        {
            var metadata = this.AllRules.Where(x => x.Rule.Equals(rule)).Single();
            this.MoveRuleToFront(metadata);
        }

        private void Proxy_CallWasIntercepted(object sender, CallInterceptedEventArgs e)
        {
            this.Intercept(e.Call);
        }

        private class InterceptedCallAdapter
            : IInterceptedFakeObjectCall
        {
            private IWritableFakeObjectCall call;

            public InterceptedCallAdapter(IWritableFakeObjectCall call)
            {
                this.call = call;
            }

            public void SetReturnValue(object value)
            {
                this.call.SetReturnValue(value);
            }

            public void CallBaseMethod()
            {
                this.call.CallBaseMethod();
            }

            public void SetArgumentValue(int index, object value)
            {
                this.call.SetArgumentValue(index, value);
            }

            public ICompletedFakeObjectCall AsReadOnly()
            {
                return this.call.AsReadOnly();
            }

            public System.Reflection.MethodInfo Method
            {
                get { return this.call.Method; }
            }

            public ArgumentCollection Arguments
            {
                get { return this.call.Arguments; }
            }

            public object FakedObject
            {
                get { return this.call.FakedObject; }
            }
        }

    }
}