namespace FakeItEasy
{
    using System;
    using System.Diagnostics;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    /// <summary>
    /// Provides extension methods for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtensions
    {
        [DebuggerStepThrough]
        public static object? GetDefaultValue(this Type type)
        {
            return type.GetTypeInfo().IsValueType && !type.Equals(typeof(void)) ? Activator.CreateInstance(type) : null;
        }

        [DebuggerStepThrough]
        public static bool CanBeInstantiatedAs(this Type type, Type targetType)
        {
            if (!targetType.IsAssignableFrom(type))
            {
                return false;
            }

            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsAbstract && !typeInfo.ContainsGenericParameters;
        }

        public static bool IsNullable(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsValueType
                || (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
