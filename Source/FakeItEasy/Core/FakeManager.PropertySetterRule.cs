namespace FakeItEasy.Core
{
    using System;

    public partial class FakeManager
    {
        [Serializable]
        private class PropertySetterRule
            : IFakeObjectCallRule
        {
            public FakeManager FakeObject { get; set; }
            
            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return PropertyBehaviorRule.IsPropertySetter(fakeObjectCall.Method);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                var newRule = new CallRuleMetadata
                {
                    CalledNumberOfTimes = 1,
                    Rule = new PropertyBehaviorRule(fakeObjectCall.Method, this.FakeObject) { Value = fakeObjectCall.Arguments[0] }
                };

                this.FakeObject.allUserRulesField.AddFirst(newRule);
            }
        }
    }
}
