namespace FakeItEasy.VisualBasic
{
    using System.Linq;
    using FakeItEasy.Core;

    /// <summary>
    /// A call rule that "sits and waits" for the next call, when
    /// that call occurs the recorded rule is added for that call.
    /// </summary>
    internal class RecordingCallRule<TFake>
        : IFakeObjectCallRule
    {
        private FakeManager fakeManager;
        private RecordedCallRule recordedRule;
        private FakeAsserter.Factory asserterFactory;

        public RecordingCallRule(FakeManager fakeManager, RecordedCallRule recordedRule, FakeAsserter.Factory asserterFactory)
        {
            this.fakeManager = fakeManager;
            this.recordedRule = recordedRule;
            this.asserterFactory = asserterFactory;
        }

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IWritableFakeObjectCall fakeObjectCall)
        {
            this.recordedRule.ApplicableToMethod = fakeObjectCall.Method;

            if (this.recordedRule.IsApplicableToArguments == null)
            {
                this.CreateArgumentsPredicateFromArguments(fakeObjectCall);
            }

            if (this.recordedRule.IsAssertion)
            {
                this.DoAssertion(fakeObjectCall);
            }
            
            this.fakeManager.AddRuleFirst(this.recordedRule);

            fakeObjectCall.SetReturnValue(Helpers.GetDefaultValueOfType(fakeObjectCall.Method.ReturnType));
        }

        private void DoAssertion(IFakeObjectCall fakeObjectCall)
        {
            var asserter = this.asserterFactory.Invoke(this.fakeManager.RecordedCallsInScope.Cast<IFakeObjectCall>());
            asserter.AssertWasCalled(this.recordedRule.IsApplicableTo, fakeObjectCall.ToString(), x => this.recordedRule.RepeatConstraint.Matches(x), this.recordedRule.RepeatConstraint.ToString());
        }

        private void CreateArgumentsPredicateFromArguments(IFakeObjectCall fakeObjectCall)
        {
            this.recordedRule.IsApplicableToArguments = x => x.AsEnumerable().SequenceEqual(fakeObjectCall.Arguments.AsEnumerable());
        }

        public int? NumberOfTimesToCall
        {
            get { return 1; }
        }
    }
}
