namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Core.Creation;

    public partial class FakeManager
    {
        [Serializable]
        private class AutoFakePropertyRule
            : IFakeObjectCallRule
        {
            public FakeManager FakeManager;

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method) && TypeIsFakable(fakeObjectCall.Method.ReturnType);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                var newRule = new CallRuleMetadata
                {
                    Rule = new PropertyBehaviorRule(fakeObjectCall.Method, FakeManager) { Value = CreateFake(fakeObjectCall.Method.ReturnType) },
                    CalledNumberOfTimes = 1
                };

                this.FakeManager.allUserRulesField.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }

            private static IFakeAndDummyManager FakeManager
            {
                get
                {
                    return ServiceLocator.Current.Resolve<IFakeAndDummyManager>();
                }
            }

            private static object CreateFake(Type type)
            {
                return FakeManager.CreateFake(type, FakeOptions.Empty);
            }

            private static bool TypeIsFakable(Type type)
            {
                object result = null;
                return FakeManager.TryCreateFake(type, FakeOptions.Empty, out result);
            }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }
        }
    }
}
