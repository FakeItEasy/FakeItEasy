namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class NonVirtualSetupAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDefinitions.NonVirtualSetup);

        private static readonly ImmutableDictionary<string, DiagnosticDescriptor> DiagnosticsMap = CreateDiagnosticsMap();

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.RegisterSyntaxNodeAction(AnalyzeMethod,
                SyntaxKind.InvocationExpression);

            context.RegisterSyntaxNodeAction(AnalyzeProperty,
                SyntaxKind.SimpleMemberAccessExpression);
        }

        public static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            //Method so exclude non-methods and interfaces

            AnalyzeCall<InvocationExpressionSyntax>(context, x => NotMethod(x) || Interface(x));
        }

        public static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            //Propery so exclude methods

            AnalyzeCall<MemberAccessExpressionSyntax>(context, x => Method(x));
        }

        private static void AnalyzeCall<T>(SyntaxNodeAnalysisContext context, Func<SymbolInfo, bool> exclusions) where T : ExpressionSyntax
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node);

            if (exclusions.Invoke(symbolInfo)) return;

            var invocationExpression = (T)context.Node;

            var invocationParent = invocationExpression
                                    .Ancestors()
                                    .OfType<InvocationExpressionSyntax>()
                                    .FirstOrDefault();

            if (!IsSetupInvocation(context, invocationParent))
                return;

            if (!symbolInfo.Symbol.IsVirtual)
            {
                var location = invocationExpression.GetLocation();

                Diagnostic diagnostic = Diagnostic.Create(DiagnosticDefinitions.NonVirtualSetup, location, symbolInfo.Symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsSetupInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax parent)
        {
            var methodSymbol = SymbolHelpers.GetCalledMethodSymbol(parent, context);

            if (methodSymbol == null)
            {
                return false;
            }

            var methodFullName =
                string.Concat(methodSymbol.ContainingType.GetFullName(), ".", methodSymbol.GetDecoratedName());

            return DiagnosticsMap.ContainsKey(methodFullName);
        }

        private static ImmutableDictionary<string, DiagnosticDescriptor> CreateDiagnosticsMap()
        {
            var memberNames = DiagnosticSubjects.CallSpecMemberNames();

            return memberNames.ToImmutableDictionary(name => name, name => DiagnosticDefinitions.NonVirtualSetup);
        }

        private static bool NotMethod(SymbolInfo x)
        {
            return x.Symbol?.Kind != SymbolKind.Method;
        }

        private static bool Method(SymbolInfo x)
        {
            return x.Symbol?.Kind == SymbolKind.Method;
        }

        private static bool Interface(SymbolInfo x)
        {
            return x.Symbol?.ContainingType.TypeKind == TypeKind.Interface;
        }
    }
}
