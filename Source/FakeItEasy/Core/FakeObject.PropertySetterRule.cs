namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class FakeObject
    {
        [Serializable]
        private class PropertySetterRule
            : IFakeObjectCallRule
        {
            public FakeObject FakeObject;

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

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }

    }
}
