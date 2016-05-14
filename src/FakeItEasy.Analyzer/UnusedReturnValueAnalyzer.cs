namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using static DiagnosticDefinitions;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedReturnValueAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableDictionary<string, DiagnosticDescriptor> Diagnostics =
            GetDiagnosticsMap(nameof(UnusedCallSpecification));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            Diagnostics.Values.ToImmutableArray();

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

            var diagnosticName = GetDiagnosticName(methodSymbol);
            if (diagnosticName == null)
            {
                return;
            }

            var descriptor = Diagnostics.GetValueOrDefault(diagnosticName);
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

        private static string GetDiagnosticName(IMethodSymbol methodSymbol)
        {
            return methodSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass.GetFullName() == "FakeItEasy.Analysis.MustUseReturnValueAttribute")
                ?.ConstructorArguments.FirstOrDefault().Value as string;
        }
    }
}
