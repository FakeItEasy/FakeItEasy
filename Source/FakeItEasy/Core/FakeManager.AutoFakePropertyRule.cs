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

                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                object theValue;
                if (!TryCreateFake(fakeObjectCall.Method.ReturnType, out theValue))
                {
                    theValue = DefaultReturnValue(fakeObjectCall);
                }

                var newRule = new CallRuleMetadata
                                  {
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method, FakeManager)
                                      {
                                          Value = theValue,
                                          Indices = fakeObjectCall.Arguments.ToArray(),
                                      },
                                      CalledNumberOfTimes = 1
                                  };

                this.FakeManager.allUserRulesField.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }

            private static bool TryCreateFake(Type type, out object fake)
            {
                return FakeAndDummyManager.TryCreateFake(type, FakeOptions.Empty, out fake);
            }

            private static object DefaultReturnValue(IInterceptedFakeObjectCall fakeObjectCall)
            {
                return DefaultReturnValueRule.ResolveReturnValue(fakeObjectCall);
            }
        }
    }
}