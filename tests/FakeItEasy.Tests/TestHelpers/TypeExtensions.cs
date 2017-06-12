namespace FakeItEasy.Tests.TestHelpers
{
    using System;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif

    public static class TypeExtensions
    {
        // The FakeItEasy project (targeting net40) uses ILMerge to merge Castle.Core
        // (targeting net40) into its output, while the FakeItEasy.Tests project targets
        // net45 due its dependency on xUnit.net. This leads to a compilation error because of the
        // ambiguity of the GetTypeInfo() extension methods, defined by the .NET
        // Framework v4.5 or later, and by Castle.Core net40. This method uses another name
        // to avoid conflicts.
#if FEATURE_NETCORE_REFLECTION
        /// <summary>
        /// A pass through extension method to call the GetTypeInfo() extension method defined
        /// in .NET Framework v4.5 or later.
        /// </summary>
        /// <param name="type">The type argument.</param>
        /// <returns>Type info of the type argument.</returns>
        public static TypeInfo GetTypeInformation(this Type type)
        {
            return type.GetTypeInfo();
        }
#else
        /// <summary>
        /// A pass through extension for legacy platforms that lack the GetTypeInfo() extension method.
        /// </summary>
        /// <param name="type">The type argument.</param>
        /// <returns>Type info of the type argument.</returns>
        public static Type GetTypeInformation(this Type type)
        {
            return type;
        }
#endif
    }
}
