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

        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            AnalyzeCall<InvocationExpressionSyntax>(context, IsMethod);
        }

        private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            AnalyzeCall<MemberAccessExpressionSyntax>(context, IsProperty);
        }

        private static void AnalyzeCall<T>(SyntaxNodeAnalysisContext context, Func<SymbolInfo, bool> includes) where T : ExpressionSyntax
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node);

            if(IsInterface(symbolInfo))
                return;

            if (!includes.Invoke(symbolInfo)) return;

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
            var callSpecMemberNames = new[]
                        {
                "FakeItEasy.A.CallTo",
                "FakeItEasy.A.CallTo`1",
                "FakeItEasy.A.CallToSet`1",
                "FakeItEasy.Fake`1.CallsTo`1"
            };

            return callSpecMemberNames.ToImmutableDictionary(name => name, name => DiagnosticDefinitions.NonVirtualSetup);
        }

        private static bool IsProperty(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.Kind == SymbolKind.Property;
        }

        private static bool IsMethod(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.Kind == SymbolKind.Method;
        }

        private static bool IsInterface(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.ContainingType.TypeKind == TypeKind.Interface;
        }
    }
}
