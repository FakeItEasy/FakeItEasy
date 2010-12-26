namespace FakeItEasy.Core
{
    using System;

    public partial class FakeManager
    {
        [Serializable]
        private class PropertySetterRule
            : IFakeObjectCallRule
        {
            public FakeManager FakeManager { get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return PropertyBehaviorRule.IsPropertySetter(fakeObjectCall.Method);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                var newRule = new CallRuleMetadata
                                  {
                                      CalledNumberOfTimes = 1, 
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method, this.FakeManager) { Value = fakeObjectCall.Arguments[0] }
                                  };

                this.FakeManager.allUserRulesField.AddFirst(newRule);
            }
        }
    }
}