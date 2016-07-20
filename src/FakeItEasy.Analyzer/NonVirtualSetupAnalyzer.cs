using System.Collections.Generic;

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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = CreateSupportedDiagnostics();

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
                                    .Ancestors()
                                    .OfType<InvocationExpressionSyntax>()
                                    .Take(1)
                                    .ToList();

            if (!IsSetupInvocation(invocationParent))
                return;

            if (!IsFakeItEasy(context, invocationParent.Single()))
                return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node);

            if (!symbolInfo.Symbol.IsVirtual)
            {
                var location = invocationExpression.GetLocation();

                Diagnostic diagnostic = Diagnostic.Create(DiagnosticDefinitions.NonVirtualSetup, location, symbolInfo.Symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsSetupInvocation(IEnumerable<InvocationExpressionSyntax> invocationParent)
        {
            return invocationParent.Any(x => x.Expression.ToString() == "A.CallTo");
        }

        private static bool IsFakeItEasy(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax parent)
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
            var typeKind = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol.ContainingType.TypeKind;

            return typeKind == TypeKind.Interface;
        }

        private static ImmutableArray<DiagnosticDescriptor> CreateSupportedDiagnostics()
        {
            var supportedDiagnostics = new[]
            {
                DiagnosticDefinitions.NonVirtualSetup
            };

            return supportedDiagnostics.ToImmutableArray();
        }
    }
}
