namespace FakeItEasy
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    /// <summary>
    /// Provides extension methods for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtensions
    {
        public static string FullNameCSharp(this Type type)
        {
            Guard.AgainstNull(type, "type");

            if (!type.GetTypeInfo().IsGenericType)
            {
                return type.FullName;
            }

            var partName = type.FullName.Split('`')[0];
            var genericArgNames = type.GetGenericArguments().Select(arg => arg.FullNameCSharp()).ToArray();
            return string.Format(CultureInfo.InvariantCulture, "{0}<{1}>", partName, string.Join(", ", genericArgNames));
        }

        [DebuggerStepThrough]
        public static object GetDefaultValue(this Type type)
        {
            return type.GetTypeInfo().IsValueType && !type.Equals(typeof(void)) ? Activator.CreateInstance(type) : null;
        }

        [DebuggerStepThrough]
        public static bool CanBeInstantiatedAs(this Type type, Type targetType)
        {
            return targetType.IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract;
        }
    }
}
