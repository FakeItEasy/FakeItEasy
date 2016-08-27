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

            var methodSymbol = SymbolHelpers.GetCalledMethodSymbol(call, context);
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

        private static ImmutableDictionary<string, DiagnosticDescriptor> CreateDiagnosticsMap()
        {
            var memberNames = DiagnosticSubjects.CallSpecMemberNames();

            return memberNames.ToImmutableDictionary(name => name, name => DiagnosticDefinitions.UnusedCallSpecification);

        }
    }
}
