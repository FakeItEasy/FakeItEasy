namespace FakeItEasy.Core
{
    using System;
    using System.Linq;

    /// <content>Property setter rule.</content>
    public partial class FakeManager
    {
#if FEATURE_BINARY_SERIALIZATION
        [Serializable]
#endif
        private class PropertySetterRule
            : SharedFakeObjectCallRule
        {
            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                return PropertyBehaviorRule.IsPropertySetter(fakeObjectCall.Method);
            }

            public override void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                var fakeManager = Fake.GetFakeManager(fakeObjectCall.FakedObject);

                // Setting the property adds a PropertyBehaviorRule to store the assigned value.
                // The PropertyBehaviorRule is added at the end of user rules to avoid overriding
                // explicit configurations of the property made with CallTo.
                var existingRule = fakeManager
                    .AllUserRules
                    .Select(metadata => metadata.Rule)
                    .OfType<PropertyBehaviorRule>()
                    .LastOrDefault(rule => rule.IsMatchForSetter(fakeObjectCall));

                if (existingRule != null)
                {
                    existingRule.Value = fakeObjectCall.Arguments.Last();
                }
                else
                {
                    var newRule = new CallRuleMetadata
                    {
                        CalledNumberOfTimes = 1,
                        Rule = new PropertyBehaviorRule(fakeObjectCall.Method)
                        {
                            Indices = fakeObjectCall.Arguments.Take(fakeObjectCall.Arguments.Count - 1).ToArray(),
                            Value = fakeObjectCall.Arguments.Last()
                        }
                    };

                    fakeManager.AllUserRules.AddLast(newRule);
                }
            }
        }
    }
}
