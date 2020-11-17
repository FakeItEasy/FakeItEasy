namespace FakeItEasy
{
    using System;

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
    }
}
