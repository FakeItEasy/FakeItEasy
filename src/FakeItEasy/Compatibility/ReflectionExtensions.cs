#if !FEATURE_NETCORE_REFLECTION
namespace FakeItEasy
{
    using System;
    using System.Reflection;

    internal static class ReflectionExtensions
    {
        /// <summary>
        /// A pass through extension for legacy platforms that lacks GetTypeInfo() extension method.
        /// </summary>
        /// <param name="type">The type argument.</param>
        /// <returns>Type info of the type argument.</returns>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        /// <summary>
        /// This allows the usage of new Reflection API Delegate.GetMethodInfo() on .NET 3.5 and 4.0.
        /// It delegates to the old Delegate.Method property.
        /// </summary>
        /// <param name="delegate">The delegate argument.</param>
        /// <returns>MethodInfo of the delegate argument.</returns>
        public static MethodInfo GetMethodInfo(this Delegate @delegate)
        {
            return @delegate.Method;
        }

        /// <summary>
        /// This allows the usage of new Reflection API Type.GetRuntimeInterfaceMap() on .NET 3.5 and 4.0.
        /// It delegates to the old Type.GetInterfaceMap() method.
        /// </summary>
        /// <param name="type">The type to search for interfaces.</param>
        /// <param name="interfaceType">The interface type to search.</param>
        /// <returns>The interface mapping.</returns>
        public static InterfaceMapping GetRuntimeInterfaceMap(this Type type, Type interfaceType)
        {
            return type.GetInterfaceMap(interfaceType);
        }
    }
}
#endif
