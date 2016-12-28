namespace FakeItEasy.Analyzer
{
    using Microsoft.CodeAnalysis;
#if CSHARP
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
#endif
    using Microsoft.CodeAnalysis.Diagnostics;
#if VISUAL_BASIC
    using Microsoft.CodeAnalysis.VisualBasic;
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

    internal static class SymbolHelpers
    {
        /// <summary>
        /// Returns the called method symbol for the invocation expression.
        /// </summary>
        /// <param name="call">The invocation expression that the symbol is required.</param>
        /// <param name="context">Current context.</param>
        /// <returns>The symbol for the invocation call.</returns>
        internal static IMethodSymbol GetCalledMethodSymbol(InvocationExpressionSyntax call, SyntaxNodeAnalysisContext context)
        {
            var name = (call?.Expression as MemberAccessExpressionSyntax)?.Name
                ?? call?.Expression as IdentifierNameSyntax;

            var method =
                name == null
                    ? null
                    : context.SemanticModel.GetSymbolInfo(name).Symbol as IMethodSymbol;

            // In Roslyn for VB.NET, some generic extension methods are "reduced", i.e. WhereConfigurationExtensions.Where<T>
            // becomes WhereConfigurationExtensions.Where, as if it were non-generic. We need the non-reduced version.
            // (in C#, ReducedFrom is always null)
            return method?.ReducedFrom ?? method;
        }
    }
}
