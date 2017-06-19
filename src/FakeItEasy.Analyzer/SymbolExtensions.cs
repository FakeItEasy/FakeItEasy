namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal static class SymbolExtensions
    {
        /// <summary>
        /// Returns the full name of a type, including its namespace and containing type. Generic types
        /// have their names decorated with their arity, e.g. <c>Foo`1</c> for <c>Foo&lt;T&gt;</c>.
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
            nameParts.Push(type.GetDecoratedName());
            var containingType = type.ContainingType;
            while (containingType != null)
            {
                nameParts.Push(containingType.GetDecoratedName());
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

        /// <summary>
        /// Returns the name of a type, decorated with its arity if it's a generic type.
        /// </summary>
        /// <param name="type">The type for which to get the decorated name.</param>
        /// <returns>The decorated name of the type.</returns>
        public static string GetDecoratedName(this INamedTypeSymbol type)
        {
            if (type.Arity == 0)
            {
                return type.Name;
            }

            return type.Name + "`" + type.Arity;
        }

        /// <summary>
        /// Returns the name of a method, decorated with its arity if it's a generic method.
        /// </summary>
        /// <param name="method">The method for which to get the decorated name.</param>
        /// <returns>The decorated name of the method.</returns>
        public static string GetDecoratedName(this IMethodSymbol method)
        {
            if (method.Arity == 0)
            {
                return method.Name;
            }

            return method.Name + "`" + method.Arity;
        }

        public static bool IsNullable(this INamedTypeSymbol type)
        {
            return type.GetFullName() == "System.Nullable`1";
        }
    }
}
