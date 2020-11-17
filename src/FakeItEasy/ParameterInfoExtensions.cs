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

            var parameterAttributes = parameterInfo.CustomAttributes;
            return parameterAttributes is null ||
                   parameterAttributes.All(customAttributeData => customAttributeData.AttributeType.FullName != IsReadOnlyAttributeFullName);
        }
    }
}
