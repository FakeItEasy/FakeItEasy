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
    class NonVirtualSetupAnalyzer:DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDefinitions.NonVirtualSetup);

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
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            if (IsInterface(context, invocationExpression))
                return;

            var invocationParent = invocationExpression
                                    .AncestorsAndSelf()
                                    .OfType<InvocationExpressionSyntax>()
                                    .FirstOrDefault();

            //Property call isn't considered an Invocation, just the fakeiteasy call itself
           
            if (!IsSetupInvocation(context, invocationParent))
                return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node);

            if (symbolInfo.Symbol == null)
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
            //Is this genuine FakeItEasy A.CallTo method?

            const string fakeItEasyMethod = "FakeItEasy.A.CallTo";

            var methodSymbol = SymbolHelpers.GetCalledMethodSymbol(parent, context);

            if (methodSymbol == null)
            {
                return false;
            }

            var methodFullName =
                string.Concat(methodSymbol.ContainingType.GetFullName(), ".", methodSymbol.GetDecoratedName());

            return methodFullName == fakeItEasyMethod;
        }

        private static bool IsInterface(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol;

            var typeKind = symbol?.ContainingType.TypeKind;

            return typeKind == TypeKind.Interface;
        }
    }
}
