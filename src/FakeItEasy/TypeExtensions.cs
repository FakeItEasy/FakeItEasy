namespace FakeItEasy;

using System;
using System.Diagnostics;
using System.Linq;

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

    public static bool IsByRefLike(this Type type)
    {
#if LACKS_ISBYREFLIKE
            return type.GetCustomAttributesData().Any(att => att.AttributeType.FullName == "System.Runtime.CompilerServices.IsByRefLikeAttribute");
#else
        return type.IsByRefLike;
#endif
    }

    [DebuggerStepThrough]
    private static bool HasDefaultConstructor(this Type type)
    {
        return type.GetConstructor(Type.EmptyTypes) is not null;
    }
}
