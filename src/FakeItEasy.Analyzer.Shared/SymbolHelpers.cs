namespace FakeItEasy.Analyzer
{
    using System.Linq;
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
        /// <param name="useFirstCandidateIfNotResolved">Use the first candidate if symbol is not resolved.</param>
        /// <returns>The symbol for the invocation call.</returns>
        internal static IMethodSymbol? GetCalledMethodSymbol(InvocationExpressionSyntax call, SyntaxNodeAnalysisContext context, bool useFirstCandidateIfNotResolved = false)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(call);
            var symbol = symbolInfo.Symbol;
            if (symbol is null && useFirstCandidateIfNotResolved)
            {
                symbol = symbolInfo.CandidateSymbols.FirstOrDefault();
            }

            var method = symbol as IMethodSymbol;

            // In Roslyn for VB.NET, some generic extension methods are "reduced", i.e. WhereConfigurationExtensions.Where<T>
            // becomes WhereConfigurationExtensions.Where, as if it were non-generic. We need the non-reduced version.
            // (in C#, ReducedFrom is always null)
            return method?.ReducedFrom ?? method;
        }

#if CSHARP
        /// <summary>
        /// Returns the accessed indexer symbol for the element access expression.
        /// </summary>
        /// <param name="elementAccess">The element access expression that the symbol is required.</param>
        /// <param name="context">Current context.</param>
        /// <returns>The symbol for the invocation call.</returns>
        internal static IPropertySymbol? GetAccessedIndexerSymbol(ElementAccessExpressionSyntax elementAccess, SyntaxNodeAnalysisContext context)
        {
            return context.SemanticModel.GetSymbolInfo(elementAccess).Symbol as IPropertySymbol;
        }
#elif VISUAL_BASIC
        /// <summary>
        /// Returns the accessed indexer symbol for the invocation expression.
        /// </summary>
        /// <param name="invocation">The invocation expression that the symbol is required.</param>
        /// <param name="context">Current context.</param>
        /// <returns>The symbol for the invocation call.</returns>
        internal static IPropertySymbol? GetAccessedIndexerSymbol(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
        {
            return context.SemanticModel.GetSymbolInfo(invocation).Symbol as IPropertySymbol;
        }
#endif

        /// <summary>
        /// Returns the accessed property for the member access expression.
        /// </summary>
        /// <param name="access">The member access expression from which to get the property symbol.</param>
        /// <param name="context">The current analysis context.</param>
        /// <returns>The symbol for the accessed property.</returns>
        internal static IPropertySymbol? GetAccessedPropertySymbol(MemberAccessExpressionSyntax access, SyntaxNodeAnalysisContext context)
        {
            return context.SemanticModel.GetSymbolInfo(access).Symbol as IPropertySymbol;
        }
    }
}
