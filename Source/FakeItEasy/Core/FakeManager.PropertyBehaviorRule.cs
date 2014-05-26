namespace FakeItEasy.Core
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <content>Property behavior rule.</content>
    public partial class FakeManager
    {
        private class PropertyBehaviorRule
            : IFakeObjectCallRule
        {
            private readonly FakeManager fakeManager;
            private readonly MethodInfo propertyGetter;
            private readonly MethodInfo propertySetter;

            public PropertyBehaviorRule(MethodInfo propertyGetterOrSetter, FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
                var property = GetProperty(propertyGetterOrSetter);

                this.propertySetter = property.GetSetMethod();
                this.propertyGetter = property.GetGetMethod(true);
            }

            public object Value { get; set; }

            public object[] Indices { get; set; }

            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            public static bool IsPropertySetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
            }

            public static bool IsPropertyGetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return this.IsPropertySetter(fakeObjectCall) || this.IsPropertyGetter(fakeObjectCall);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, "fakeObjectCall");

                if (this.IsPropertyGetter(fakeObjectCall))
                {
                    fakeObjectCall.SetReturnValue(this.Value);
                }
                else
                {
                    this.Value = fakeObjectCall.Arguments.Last();
                }

                this.fakeManager.MoveRuleToFront(this);
            }

            private static PropertyInfo GetProperty(MethodInfo propertyGetterOrSetter)
            {
                return
                    (from property in propertyGetterOrSetter.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                     let getMethod = property.GetGetMethod(true)
                     let setMethod = property.GetSetMethod(true)
                     where (getMethod != null && getMethod.GetBaseDefinition().Equals(propertyGetterOrSetter.GetBaseDefinition()))
                         || (setMethod != null && setMethod.GetBaseDefinition().Equals(propertyGetterOrSetter.GetBaseDefinition()))
                     select property).Single();
            }

            private bool IsPropertySetter(IFakeObjectCall fakeObjectCall)
            {
                return this.propertySetter != null &&
                       this.propertySetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition()) &&
                       this.Indices.SequenceEqual(fakeObjectCall.Arguments.Take(fakeObjectCall.Arguments.Count - 1));
            }

            private bool IsPropertyGetter(IFakeObjectCall fakeObjectCall)
            {
                return this.propertyGetter != null &&
                       this.propertyGetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition()) &&
                       this.Indices.SequenceEqual(fakeObjectCall.Arguments);
            }
        }
    }
}