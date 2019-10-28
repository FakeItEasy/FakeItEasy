namespace FakeItEasy.Core
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Compatibility;

    /// <content>Property behavior rule.</content>
    public partial class FakeManager
    {
        private class PropertyBehaviorRule
            : IFakeObjectCallRule
        {
            private readonly MethodInfo propertyGetter;
            private readonly MethodInfo propertySetter;

            public PropertyBehaviorRule(MethodInfo propertyGetterOrSetter)
            {
                var property = GetProperty(propertyGetterOrSetter);

                this.propertySetter = property.GetSetMethod();
                this.propertyGetter = property.GetGetMethod(true);
            }

            public object? Value { get; set; }

            public object?[] Indices { get; set; } = ArrayHelper.Empty<object?>();

            public int? NumberOfTimesToCall => null;

            public static bool IsPropertySetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
            }

            public static bool IsPropertyGetter(MethodInfo method)
            {
                return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
            }

            public bool IsMatchForSetter(IFakeObjectCall fakeObjectCall) => this.IsPropertySetter(fakeObjectCall);

            public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return this.IsPropertyGetter(fakeObjectCall);
            }

            public void Apply(IInterceptedFakeObjectCall fakeObjectCall)
            {
                Guard.AgainstNull(fakeObjectCall, nameof(fakeObjectCall));

                fakeObjectCall.SetReturnValue(this.Value);
            }

            private static PropertyInfo GetProperty(MethodInfo propertyGetterOrSetter)
            {
                return
                    (from property in propertyGetterOrSetter.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                     let getMethod = property.GetGetMethod(true)
                     let setMethod = property.GetSetMethod(true)
                     where (getMethod is object && getMethod.GetBaseDefinition().Equals(propertyGetterOrSetter.GetBaseDefinition()))
                         || (setMethod is object && setMethod.GetBaseDefinition().Equals(propertyGetterOrSetter.GetBaseDefinition()))
                     select property).Single();
            }

            private bool IsPropertySetter(IFakeObjectCall fakeObjectCall)
            {
                return this.propertySetter is object &&
                       this.propertySetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition()) &&
                       this.Indices.SequenceEqual(fakeObjectCall.Arguments.Take(fakeObjectCall.Arguments.Count - 1));
            }

            private bool IsPropertyGetter(IFakeObjectCall fakeObjectCall)
            {
                return this.propertyGetter is object &&
                       this.propertyGetter.GetBaseDefinition().Equals(fakeObjectCall.Method.GetBaseDefinition()) &&
                       this.Indices.SequenceEqual(fakeObjectCall.Arguments);
            }
        }
    }
}
