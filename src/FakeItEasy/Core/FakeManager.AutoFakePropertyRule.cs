namespace FakeItEasy.Core
{
    using System;
    using System.Linq;
    using FakeItEasy.Creation;

    /// <content>Auto fake property rule.</content>
    public partial class FakeManager
    {
#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class AutoFakePropertyRule
            : IFakeObjectCallRule
        {
            private readonly FakeManager fakeManager;

            public AutoFakePropertyRule(FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
            }

            public int? NumberOfTimesToCall => null;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                var newRule = new CallRuleMetadata
                                  {
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method)
                                      {
                                          Value = fakeObjectCall.GetDefaultReturnValue(),
                                          Indices = fakeObjectCall.Arguments.ToArray(),
                                      },
                                      CalledNumberOfTimes = 1
                                  };

                this.fakeManager.AllUserRules.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }
        }
    }
}
