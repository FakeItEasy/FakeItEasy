namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
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
    internal class NonVirtualSetupAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> CallSpecMethods =
            ImmutableHashSet.Create(
                StringComparer.Ordinal,
                "FakeItEasy.A.CallTo",
                "FakeItEasy.A.CallTo`1",
                "FakeItEasy.A.CallToSet`1",
                "FakeItEasy.Fake`1.CallsTo`1");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDefinitions.NonVirtualSetupSpecification);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.RegisterSyntaxNodeAction(
                AnalyzeMethod,
                SyntaxKind.InvocationExpression);

            context.RegisterSyntaxNodeAction(
                AnalyzeProperty,
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

            if (IsInterfaceMember(symbolInfo))
            {
                return;
            }

            if (!includes.Invoke(symbolInfo))
            {
                return;
            }

            var invocationExpression = (T)context.Node;

            var invocationParent = invocationExpression
                                    .Ancestors()
                                    .OfType<InvocationExpressionSyntax>()
                                    .FirstOrDefault();

            if (!IsSetupInvocation(context, invocationParent))
            {
                return;
            }

            if (!IsVirtual(symbolInfo))
            {
                var location = invocationExpression.GetLocation();

                Diagnostic diagnostic = Diagnostic.Create(DiagnosticDefinitions.NonVirtualSetupSpecification, location, symbolInfo.Symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsVirtual(SymbolInfo symbolInfo)
        {
            var member = symbolInfo.Symbol;

            bool isVirtual = member.IsVirtual
                             || (member.IsOverride && !member.IsSealed)
                             || member.IsAbstract;

            return isVirtual;
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

            return CallSpecMethods.Contains(methodFullName);
        }

        private static bool IsProperty(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.Kind == SymbolKind.Property;
        }

        private static bool IsMethod(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.Kind == SymbolKind.Method;
        }

        private static bool IsInterfaceMember(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.ContainingType?.TypeKind == TypeKind.Interface;
        }
    }
}
