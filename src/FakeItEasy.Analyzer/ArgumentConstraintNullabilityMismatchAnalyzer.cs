namespace FakeItEasy.Analyzer
{
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
            var argumentList = argument?.Parent as ArgumentListSyntax;
            var invocation = argumentList?.Parent as InvocationExpressionSyntax;
            if (invocation == null)
            {
                return;
            }

            // TODO: could be an indexer, not a method
            var method = SymbolHelpers.GetCalledMethodSymbol(invocation, context);
            if (method == null)
            {
                return;
            }

            int index = argumentList.Arguments.IndexOf(argument);
            if (index < 0 || index >= method.Parameters.Length)
            {
                return;
            }

            var parameter = method.Parameters[index];
            var parameterType = parameter.Type as INamedTypeSymbol;
            if (parameterType != null && parameterType.IsNullable())
            {
                var nullableParameterType = (INamedTypeSymbol)parameter.Type;
                var nonNullableParameterType = nullableParameterType.TypeArguments[0];
                if (constraintType.Equals(nonNullableParameterType))
                {
                    var descriptor = DiagnosticDefinitions.ArgumentConstraintNullabilityMismatch;
                    var diagnostic = Diagnostic.Create(
                        descriptor,
                        completeConstraint.GetLocation(),
                        parameter.Name,
                        method.GetDecoratedName(),
                        completeConstraint.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
