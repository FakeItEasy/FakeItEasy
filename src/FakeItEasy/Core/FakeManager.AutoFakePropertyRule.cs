namespace FakeItEasy.Core
{
    using System;
    using System.Linq;

    /// <content>Auto fake property rule.</content>
    public partial class FakeManager
    {
        [Serializable]
        private class AutoFakePropertyRule
            : IFakeObjectCallRule
        {
            private readonly FakeManager fakeManager;

            public AutoFakePropertyRule(FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
            }

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
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method, this.fakeManager)
                                      {
                                          Value = DefaultReturnValue(fakeObjectCall),
                                          Indices = fakeObjectCall.Arguments.ToArray(),
                                      },
                                      CalledNumberOfTimes = 1
                                  };

                this.fakeManager.allUserRulesField.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }

            private static object DefaultReturnValue(IInterceptedFakeObjectCall fakeObjectCall)
            {
                return DefaultReturnValueRule.ResolveReturnValue(fakeObjectCall);
            }
        }
    }
}
