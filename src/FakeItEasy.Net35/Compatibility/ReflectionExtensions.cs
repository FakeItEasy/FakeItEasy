using System;
using System.Reflection;

namespace FakeItEasy
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// A passthrough extension for legacy platforms that lacks GetTypeInfo() extension method.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        /// <summary>
        /// This allows the usage of new Reflection Api Delegate.GetMethodInfo() on .NET 3.5 and 4.0.
        /// It delegates to the old Delegate.Method property.
        /// </summary>
        /// <param name="delegate"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(this Delegate @delegate)
        {
            return @delegate.Method;
        }
    }
}
