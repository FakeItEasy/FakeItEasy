namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedReturnValueAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableDictionary<string, DiagnosticDescriptor> DiagnosticsMap = CreateDiagnosticsMap();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            DiagnosticsMap.Values.ToImmutableArray();

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

            var methodSymbol = GetCalledMethodSymbol(call, context);
            if (methodSymbol == null)
            {
                return;
            }

            var methodFullName =
                string.Concat(methodSymbol.ContainingType.GetFullName(), ".", methodSymbol.GetDecoratedName());

            var descriptor = DiagnosticsMap.GetValueOrDefault(methodFullName);
            if (descriptor == null)
            {
                return;
            }

            if (call.Parent is StatementSyntax)
            {
                var diagnostic = Diagnostic.Create(descriptor, call.GetLocation(), call.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static IMethodSymbol GetCalledMethodSymbol(InvocationExpressionSyntax call, SyntaxNodeAnalysisContext context)
        {
            var memberAccess = call?.Expression as MemberAccessExpressionSyntax;
            return context.SemanticModel.GetSymbolInfo(memberAccess?.Name).Symbol as IMethodSymbol;
        }

        private static ImmutableDictionary<string, DiagnosticDescriptor> CreateDiagnosticsMap()
        {
            var callSpecMemberNames = new[]
            {
                "FakeItEasy.A.CallTo",
                "FakeItEasy.A.CallTo`1",
                "FakeItEasy.A.CallToSet`1",
                "FakeItEasy.Fake`1.CallsTo`1",
                "FakeItEasy.Fake`1.AnyCall",
                "FakeItEasy.ArgumentValidationConfigurationExtensions.WithAnyArguments`1",
                "FakeItEasy.WhereConfigurationExtensions.Where`1",
                "FakeItEasy.Configuration.IAnyCallConfigurationWithNoReturnTypeSpecified.WithReturnType`1",
                "FakeItEasy.Configuration.IArgumentValidationConfiguration`1.WhenArgumentsMatch",
                "FakeItEasy.Configuration.IPropertySetterAnyValueConfiguration`1.To",
                "FakeItEasy.Configuration.IWhereConfiguration`1.Where"
            };

            return callSpecMemberNames.ToImmutableDictionary(name => name, name => DiagnosticDefinitions.UnusedCallSpecification);
        }
    }
}
