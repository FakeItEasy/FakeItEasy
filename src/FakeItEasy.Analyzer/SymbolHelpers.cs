namespace FakeItEasy.Analyzer
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

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

            return name == null
                ? null
                : context.SemanticModel.GetSymbolInfo(name).Symbol as IMethodSymbol;
        }
    }
}
