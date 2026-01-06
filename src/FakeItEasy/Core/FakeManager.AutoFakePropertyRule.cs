namespace FakeItEasy.Core;

using System.Linq;

/// <content>Auto fake property rule.</content>
public partial class FakeManager
{
    private class AutoFakePropertyRule
        : SharedFakeObjectCallRule
    {
        public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall);

            return PropertyBehaviorRule.IsPropertyGetter(fakeObjectCall.Method);
        }

        public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
        {
            Guard.AgainstNull(fakeObjectCall);

            var newRule = CallRuleMetadata.CalledOnce(
                new PropertyBehaviorRule(fakeObjectCall.Method)
                {
                    Value = fakeObjectCall.GetDefaultReturnValue(),
                    Indices = fakeObjectCall.Arguments.ToArray(),
                });

            Fake.GetFakeManager(fakeObjectCall.FakedObject).MutateUserRules(
                allUserRules => allUserRules.AddFirst(newRule));

            newRule.Rule.Apply(fakeObjectCall);
        }
    }
}
