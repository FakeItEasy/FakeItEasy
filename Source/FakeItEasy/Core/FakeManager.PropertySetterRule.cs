namespace FakeItEasy.Core
{
    using System;

    /// <content>Property setter rule.</content>
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
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                return PropertyBehaviorRule.IsPropertySetter(fakeObjectCall.Method);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                var newRule = new CallRuleMetadata
                                  {
                                      CalledNumberOfTimes = 1,
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method, this.FakeManager)
                                                 {
                                                     Value = fakeObjectCall.Arguments[fakeObjectCall.Arguments.Count - 1]
                                                 }
                                  };

                this.FakeManager.allUserRulesField.AddFirst(newRule);
            }
        }
    }
}