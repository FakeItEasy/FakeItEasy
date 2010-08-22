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
        private IFakeObjectCallFormatter callFormatter;

        public RecordingCallRule(FakeManager fakeManager, RecordedCallRule recordedRule, FakeAsserter.Factory asserterFactory, IFakeObjectCallFormatter callFormatter)
        {
            this.fakeManager = fakeManager;
            this.recordedRule = recordedRule;
            this.asserterFactory = asserterFactory;
            this.callFormatter = callFormatter;
        }

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            fakeObjectCall.DoNotRecordCall();

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

            var callDescription = this.callFormatter.GetDescription(fakeObjectCall);
            var repeatDescription = this.recordedRule.RepeatConstraint.ToString();

            asserter.AssertWasCalled(this.recordedRule.IsApplicableTo, callDescription, x => this.recordedRule.RepeatConstraint.Matches(x), repeatDescription);
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
