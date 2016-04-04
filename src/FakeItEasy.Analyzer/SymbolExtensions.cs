namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal static class SymbolExtensions
    {
        /// <summary>
        /// Returns the full name of a type, including its namespace and containing type.
        /// </summary>
        /// <param name="type">The type for which to get the full name.</param>
        /// <returns>The full name of the type.</returns>
        public static string GetFullName(this INamedTypeSymbol type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var nameParts = new Stack<string>();
            nameParts.Push(type.Name);
            var containingType = type.ContainingType;
            while (containingType != null)
            {
                nameParts.Push(containingType.Name);
                containingType = containingType.ContainingType;
            }

            var containingNamespace = type.ContainingNamespace;
            while (!string.IsNullOrEmpty(containingNamespace?.Name))
            {
                nameParts.Push(containingNamespace.Name);
                containingNamespace = containingNamespace.ContainingNamespace;
            }

            return string.Join(".", nameParts);
        }
    }
}
