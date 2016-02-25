namespace FakeItEasy.Core
{
    using System;
    using System.Linq;

    /// <content>Auto fake property rule.</content>
    public partial class FakeManager
    {
#if FEATURE_SERIALIZATION
        [Serializable]
#endif
        private class AutoFakePropertyRule
            : IFakeObjectCallRule
        {
            public FakeManager FakeManager { private get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                var newRule = new CallRuleMetadata
                                  {
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method, FakeManager)
                                      {
                                          Value = DefaultReturnValue(fakeObjectCall),
                                          Indices = fakeObjectCall.Arguments.ToArray(),
                                      },
                                      CalledNumberOfTimes = 1
                                  };

                this.FakeManager.allUserRulesField.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }

            private static object DefaultReturnValue(IInterceptedFakeObjectCall fakeObjectCall)
            {
                return DefaultReturnValueRule.ResolveReturnValue(fakeObjectCall);
            }
        }
    }
}
