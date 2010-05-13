namespace FakeItEasy.Core
{
    using System.Linq;
    using System.Reflection;

    public partial class FakeManager
    {
        private class PropertyBehaviorRule
            : IFakeObjectCallRule
        {
            private MethodInfo propertySetter;
            private MethodInfo propertyGetter;
            private FakeManager fakeManager;

            public PropertyBehaviorRule(MethodInfo propertyGetterOrSetter, FakeManager fakeManager)
            {
                this.fakeManager = fakeManager;
                var property = GetProperty(propertyGetterOrSetter);

                this.propertySetter = property.GetSetMethod();
                this.propertyGetter = property.GetGetMethod(true);
            }

            public object Value { get; set; }
            
            public int? NumberOfTimesToCall
            {
                get { return null; }
            }

            public static bool IsPropertySetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("set_", System.StringComparison.Ordinal);
            }

            public static bool IsPropertyGetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("get_", System.StringComparison.Ordinal);
            }

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return this.IsPropertySetter(fakeObjectCall) || this.IsPropertyGetter(fakeObjectCall);
            }

            public void Apply(IWritableFakeObjectCall fakeObjectCall)
            {
                if (this.IsPropertyGetter(fakeObjectCall))
                {
                    fakeObjectCall.SetReturnValue(this.Value);
                }
                else
                {
                    this.Value = fakeObjectCall.Arguments[0];
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
                return this.propertySetter != null && this.propertySetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition());
            }

            private bool IsPropertyGetter(IFakeObjectCall fakeObjectCall)
            {
                return this.propertyGetter != null && this.propertyGetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition());
            }
        }
    }
}
