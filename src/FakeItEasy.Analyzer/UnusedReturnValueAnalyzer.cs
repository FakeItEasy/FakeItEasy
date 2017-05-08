namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
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
    public class UnusedReturnValueAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> CallSpecMethods =
            ImmutableHashSet.Create(
                StringComparer.Ordinal,
                "FakeItEasy.A.CallTo",
                "FakeItEasy.A.CallTo`1",
                "FakeItEasy.A.CallToSet`1",
                "FakeItEasy.Fake`1.CallsTo`1",
                "FakeItEasy.Fake`1.CallsToSet`1",
                "FakeItEasy.Fake`1.AnyCall",
                "FakeItEasy.ArgumentValidationConfigurationExtensions.WithAnyArguments`1",
                "FakeItEasy.WhereConfigurationExtensions.Where`1",
                "FakeItEasy.Configuration.IAnyCallConfigurationWithNoReturnTypeSpecified.WithReturnType`1",
                "FakeItEasy.Configuration.IAnyCallConfigurationWithNoReturnTypeSpecified.WithNonVoidReturnType",
                "FakeItEasy.Configuration.IArgumentValidationConfiguration`1.WhenArgumentsMatch",
                "FakeItEasy.Configuration.IPropertySetterAnyValueConfiguration`1.To",
                "FakeItEasy.Configuration.IWhereConfiguration`1.Where");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DiagnosticDefinitions.UnusedCallSpecification);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.RegisterSyntaxNodeAction(AnalyzeCall, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeCall(SyntaxNodeAnalysisContext context)
        {
            var call = context.Node as InvocationExpressionSyntax;
            if (call == null)
            {
                return;
            }

            var methodSymbol = SymbolHelpers.GetCalledMethodSymbol(call, context);
            if (methodSymbol == null)
            {
                return;
            }

            var methodFullName =
                string.Concat(methodSymbol.ContainingType.GetFullName(), ".", methodSymbol.GetDecoratedName());

            if (!CallSpecMethods.Contains(methodFullName))
            {
                return;
            }

            if (call.Parent is ExpressionStatementSyntax)
            {
                var descriptor = DiagnosticDefinitions.UnusedCallSpecification;
                var diagnostic = Diagnostic.Create(descriptor, call.GetLocation(), call.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
