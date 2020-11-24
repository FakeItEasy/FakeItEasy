namespace FakeItEasy
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides extension methods for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtensions
    {
        [DebuggerStepThrough]
        public static object? GetDefaultValue(this Type type)
        {
            return type.IsValueType && !type.Equals(typeof(void)) ? Activator.CreateInstance(type) : null;
        }

        [DebuggerStepThrough]
        public static bool CanBeInstantiatedAs(this Type type, Type targetType)
        {
            if (!targetType.IsAssignableFrom(type))
            {
                return false;
            }

            return !type.IsAbstract && !type.ContainsGenericParameters && type.HasDefaultConstructor();
        }

        public static bool IsNullable(this Type type)
        {
            return !type.IsValueType
                || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        [DebuggerStepThrough]
        private static bool HasDefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) is not null;
        }
    }
}
