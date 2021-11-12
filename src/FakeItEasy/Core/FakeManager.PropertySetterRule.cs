namespace FakeItEasy.Core
{
    using System.Linq;

    /// <content>Property setter rule.</content>
    public partial class FakeManager
    {
        private class PropertySetterRule
            : SharedFakeObjectCallRule
        {
            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall);

                return PropertyBehaviorRule.IsPropertySetter(fakeObjectCall.Method);
            }

            public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall);

                Fake.GetFakeManager(fakeObjectCall.FakedObject).MutateUserRules(allUserRules =>
                {
                    // Setting the property adds a PropertyBehaviorRule to store the assigned value.
                    // The PropertyBehaviorRule is added at the end of user rules to avoid overriding
                    // explicit configurations of the property made with CallTo.
                    var existingRule = allUserRules
                        .Select(metadata => metadata.Rule)
                        .OfType<PropertyBehaviorRule>()
                        .LastOrDefault(rule => rule.IsMatchForSetter(fakeObjectCall));

                    if (existingRule is not null)
                    {
                        existingRule.Value = fakeObjectCall.Arguments.Last();
                    }
                    else
                    {
                        var newRule = CallRuleMetadata.CalledOnce(
                            new PropertyBehaviorRule(fakeObjectCall.Method)
                            {
                                Indices = fakeObjectCall.Arguments.Take(fakeObjectCall.Arguments.Count - 1).ToArray(),
                                Value = fakeObjectCall.Arguments.Last()
                            });

                        allUserRules.AddLast(newRule);
                    }
                });
            }
        }
    }
}
