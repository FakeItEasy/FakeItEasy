namespace FakeItEasy
{
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="MethodBase"/>.
    /// </summary>
    internal static class MethodBaseExtensions
    {
        public static string GetGenericArgumentsCSharp(this MethodBase method)
        {
            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length == 0)
            {
                return string.Empty;
            }

            return string.Concat("<", string.Join(", ", genericArguments.Select(type => type.FullNameCSharp()).ToArray()), ">");
        }
    }
}