namespace FakeItEasy.Analyzer
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
#if CSHARP
    using Microsoft.CodeAnalysis.CSharp.Syntax;
#endif
    using Microsoft.CodeAnalysis.Diagnostics;
#if VISUAL_BASIC
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

#if CSHARP
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
#endif
    public class ArgumentConstraintNullabilityMismatchAnalyzer : ArgumentConstraintAnalyzerBase
    {
        private static readonly ImmutableHashSet<string> SupportedArgumentConstraintProperties =
            ImmutableHashSet.Create(
                "FakeItEasy.A`1._",
                "FakeItEasy.A`1.Ignored");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DiagnosticDefinitions.ArgumentConstraintNullabilityMismatch);

        protected override bool IsSupportedArgumentConstraintProperty(string fullName) =>
            SupportedArgumentConstraintProperties.Contains(fullName);

        protected override void AnalyzeArgumentConstraintCore(SyntaxNodeAnalysisContext context, SyntaxNode completeConstraint)
        {
            var constraintType = context.SemanticModel.GetTypeInfo(completeConstraint).Type as INamedTypeSymbol;
            if (constraintType == null || !constraintType.IsValueType || constraintType.IsNullable())
            {
                return;
            }

            var argument = completeConstraint.Parent as ArgumentSyntax;
#if CSHARP
            var argumentList = argument?.Parent as BaseArgumentListSyntax;
#elif VISUAL_BASIC
            var argumentList = argument?.Parent as ArgumentListSyntax;
#endif

            if (argumentList == null)
            {
                return;
            }

            if (!TryGetNameAndParameters(context, argumentList.Parent, out string methodOrIndexerName, out var parameters))
            {
                return;
            }

            int index = argumentList.Arguments.IndexOf(argument);
            if (index < 0 || index >= parameters.Count)
            {
                return;
            }

            var parameter = parameters[index];
            if (parameter.Type is INamedTypeSymbol parameterType && parameterType.IsNullable())
            {
                var nonNullableParameterType = parameterType.TypeArguments[0];
                if (constraintType.Equals(nonNullableParameterType))
                {
                    var descriptor = DiagnosticDefinitions.ArgumentConstraintNullabilityMismatch;
                    var diagnostic = Diagnostic.Create(
                        descriptor,
                        completeConstraint.GetLocation(),
                        parameter.Name,
                        methodOrIndexerName,
                        completeConstraint.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool TryGetNameAndParameters(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            out string methodOrIndexerName,
            out IReadOnlyList<IParameterSymbol> parameters)
        {
            methodOrIndexerName = null;
            parameters = null;

            switch (node)
            {
                case InvocationExpressionSyntax invocation:
                {
                    var method = SymbolHelpers.GetCalledMethodSymbol(invocation, context);
                    if (method == null)
                    {
#if VISUAL_BASIC
                        var indexer = SymbolHelpers.GetAccessedIndexerSymbol(invocation, context);
                        if (indexer == null)
                        {
                            return false;
                        }

                        methodOrIndexerName = indexer.Name;
                        parameters = indexer.Parameters;
                        return true;
#else
                        return false;
#endif
                    }

                    parameters = method.Parameters;
                    methodOrIndexerName = method.GetDecoratedName();
                    return true;
                }
#if CSHARP
                case ElementAccessExpressionSyntax elementAccess:
                {
                    var indexer = SymbolHelpers.GetAccessedIndexerSymbol(elementAccess, context);
                    if (indexer == null)
                    {
                        return false;
                    }

                    methodOrIndexerName = indexer.Name;
                    parameters = indexer.Parameters;
                    return true;
                }
#endif
                default:
                    return false;
            }
        }
    }
}
