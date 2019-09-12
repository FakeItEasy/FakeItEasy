namespace FakeItEasy
{
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="ParameterInfo"/>.
    /// </summary>
    internal static class ParameterInfoExtensions
    {
        private const string IsReadOnlyAttributeFullName = "System.Runtime.CompilerServices.IsReadOnlyAttribute";

        public static bool IsOutOrRef(this ParameterInfo parameterInfo)
        {
            if (!parameterInfo.ParameterType.IsByRef)
            {
                return false;
            }

#if FEATURE_PARAMETERINFO_CUSTOMATTRIBUTES_PROPERTY
            var parameterAttributes = parameterInfo.CustomAttributes;
            return parameterAttributes is null ||
                   parameterAttributes.All(customAttributeData => customAttributeData.AttributeType.FullName != IsReadOnlyAttributeFullName);
#else
            var parameterAttributes = parameterInfo.GetCustomAttributesData();
            return parameterAttributes is null ||
                   parameterAttributes.All(customAttributeData => customAttributeData.Constructor.DeclaringType?.FullName != IsReadOnlyAttributeFullName);
#endif
        }
    }
}
