namespace FakeItEasy.Core
{
    using System.Linq;

    /// <content>Auto fake property rule.</content>
    public partial class FakeManager
    {
        private class AutoFakePropertyRule
            : SharedFakeObjectCallRule
        {
            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method);
            }

            public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                var newRule = new CallRuleMetadata
                                  {
                                      Rule = new PropertyBehaviorRule(fakeObjectCall.Method)
                                      {
                                          Value = fakeObjectCall.GetDefaultReturnValue(),
                                          Indices = fakeObjectCall.Arguments.ToArray(),
                                      },
                                      CalledNumberOfTimes = 1
                                  };

                Fake.GetFakeManager(fakeObjectCall.FakedObject).AllUserRules.AddFirst(newRule);
                newRule.Rule.Apply(fakeObjectCall);
            }
        }
    }
}
