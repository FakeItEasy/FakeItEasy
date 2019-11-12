namespace FakeItEasy.Analyzer
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
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

#if CSHARP
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
#endif
    public class ArgumentConstraintTypeMismatchAnalyzer : ArgumentConstraintAnalyzerBase
    {
        internal const string ParameterTypeKey = "parameterType";

        private static readonly ImmutableHashSet<string> SupportedArgumentConstraintProperties =
            ImmutableHashSet.Create(
                "FakeItEasy.A`1._",
                "FakeItEasy.A`1.Ignored",
                "FakeItEasy.A`1.That");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
                DiagnosticDefinitions.ArgumentConstraintNullabilityMismatch,
                DiagnosticDefinitions.ArgumentConstraintTypeMismatch);

        protected override bool IsSupportedArgumentConstraintProperty(string fullName) =>
            SupportedArgumentConstraintProperties.Contains(fullName);

        protected override void AnalyzeArgumentConstraintCore(string propertyFullName, SyntaxNodeAnalysisContext context, SyntaxNode completeConstraint)
        {
            var constraintType = context.SemanticModel.GetTypeInfo(completeConstraint).Type as INamedTypeSymbol;
            if (constraintType is null)
            {
                return;
            }

            var argument = completeConstraint.Parent as ArgumentSyntax;
#if CSHARP
            var argumentList = argument?.Parent as BaseArgumentListSyntax;
#elif VISUAL_BASIC
            var argumentList = argument?.Parent as ArgumentListSyntax;
#endif

            if (argumentList is null)
            {
                return;
            }

            if (!TryGetNameAndParameters(context, argumentList.Parent, out var methodOrIndexerName, out var parameters))
            {
                return;
            }

            int index = argumentList.Arguments.IndexOf(argument);
            if (index < 0 || index >= parameters.Count)
            {
                return;
            }

            var parameter = parameters[index];
            if (parameter.Type is INamedTypeSymbol parameterType)
            {
                var conversionType = context.SemanticModel.Compilation.ClassifyConversion(constraintType, parameterType);
                if (conversionType.IsIdentity || !conversionType.Exists)
                {
                    return;
                }

                var nonNullableParameterType = parameterType.IsNullable() ? parameterType.TypeArguments[0] : null;

                if (nonNullableParameterType is object &&
                    constraintType.IsValueType &&
                    !constraintType.IsNullable() &&
                    SymbolEqualityComparer.Default.Equals(constraintType, nonNullableParameterType))
                {
                    if (propertyFullName != "FakeItEasy.A`1.That")
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
                else
                {
#if CSHARP
                    if (!conversionType.IsImplicit || !conversionType.IsReference)
#elif VISUAL_BASIC
                    if (!conversionType.IsWidening || !conversionType.IsReference)
#endif
                    {
                        var propertiesBuilder = ImmutableDictionary.CreateBuilder<string, string>();
                        propertiesBuilder.Add(ParameterTypeKey, parameterType.ToString());

                        var descriptor = DiagnosticDefinitions.ArgumentConstraintTypeMismatch;
                        var diagnostic = Diagnostic.Create(
                            descriptor,
                            completeConstraint.GetLocation(),
                            propertiesBuilder.ToImmutable(),
                            parameter.Name,
                            methodOrIndexerName,
                            parameter.Type,
                            completeConstraint.ToString(),
                            constraintType);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private static bool TryGetNameAndParameters(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            [NotNullWhen(true)]
            out string? methodOrIndexerName,
            [NotNullWhen(true)]
            out IReadOnlyList<IParameterSymbol>? parameters)
        {
            methodOrIndexerName = null;
            parameters = null;

            switch (node)
            {
                case InvocationExpressionSyntax invocation:
                    {
                        var method = SymbolHelpers.GetCalledMethodSymbol(invocation, context);
                        if (method is null)
                        {
#if VISUAL_BASIC
                            var indexer = SymbolHelpers.GetAccessedIndexerSymbol(invocation, context);
                            if (indexer is null)
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
                        if (indexer is null)
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
