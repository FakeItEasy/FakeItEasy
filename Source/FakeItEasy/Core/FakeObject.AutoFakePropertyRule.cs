namespace FakeItEasy.Core
{
    using System;

    public partial class FakeObject
    {
        [Serializable]
        private class AutoFakePropertyRule
            : IFakeObjectCallRule
        {
            public FakeObject FakeObject;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method) && TypeIsFakable(fakeObjectCall.Method.ReturnType);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                var newRule = new CallRuleMetadata
                {
                    Rule = new PropertyBehaviorRule(fakeObjectCall.Method, FakeObject) { Value = this.Factory.CreateFake(fakeObjectCall.Method.ReturnType, null, true) },
                    CalledNumberOfTimes = 1
                };

                this.FakeObject.allUserRulesField.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }

            private FakeObjectFactory Factory
            { 
                get
                {
                    return ServiceLocator.Current.Resolve<FakeObjectFactory>();
                }
            }

            private bool TypeIsFakable(Type type)
            {
                var command = this.Factory.CreateGenerationCommand(type, null, true);
                return command.GenerateFakeObject();
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
