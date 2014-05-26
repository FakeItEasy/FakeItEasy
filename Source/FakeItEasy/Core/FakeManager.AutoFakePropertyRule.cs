namespace FakeItEasy.Core
{
    using System;
    using System.Linq;
    using FakeItEasy.Creation;

    /// <content>Auto fake property rule.</content>
    public partial class FakeManager
    {
        [Serializable]
        private class AutoFakePropertyRule
            : IFakeObjectCallRule
        {
            public FakeManager FakeManager { get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            private static IFakeAndDummyManager FakeAndDummyManager
            {
                get { return ServiceLocator.Current.Resolve<IFakeAndDummyManager>(); }
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method) && TypeIsFakable(fakeObjectCall.Method.ReturnType);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                var newRule = new CallRuleMetadata
                                  {
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method, FakeManager)
                                      {
                                          Value = CreateFake(fakeObjectCall.Method.ReturnType),
                                          Indices = fakeObjectCall.Arguments.ToArray(),
                                      },
                                      CalledNumberOfTimes = 1
                                  };

                this.FakeManager.allUserRulesField.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }

            private static object CreateFake(Type type)
            {
                return FakeAndDummyManager.CreateFake(type, FakeOptions.Empty);
            }

            private static bool TypeIsFakable(Type type)
            {
                object result = null;
                return FakeAndDummyManager.TryCreateFake(type, FakeOptions.Empty, out result);
            }
        }
    }
}