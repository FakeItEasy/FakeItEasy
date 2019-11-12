namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Reflection;
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

#pragma warning disable SA1114 // Parameter list must follow declaration
        private static readonly ImmutableArray<Ancestor> ExpectedNodeHierarchy =
            ImmutableArray.Create(
#if VISUAL_BASIC
                new Ancestor(typeof(ExpressionStatementSyntax), optional: true),
#endif
                new Ancestor(typeof(LambdaExpressionSyntax)),
                new Ancestor(typeof(ArgumentSyntax)),
                new Ancestor(typeof(ArgumentListSyntax)),
                new Ancestor(typeof(InvocationExpressionSyntax)));
#pragma warning restore SA1114 // Parameter list must follow declaration

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDefinitions.NonVirtualSetupSpecification);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(
                AnalyzeMethod,
                SyntaxKind.InvocationExpression);

            context.RegisterSyntaxNodeAction(
                AnalyzeProperty,
                SyntaxKind.SimpleMemberAccessExpression);

#if CSHARP
            context.RegisterSyntaxNodeAction(
                AnalyzeCSharpIndexer,
                SyntaxKind.ElementAccessExpression);
#elif VISUAL_BASIC
            context.RegisterSyntaxNodeAction(
                AnalyzeVBIndexer,
                SyntaxKind.InvocationExpression);
#endif
        }

        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            AnalyzeCall<InvocationExpressionSyntax>(context, IsMethod);
        }

        private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            AnalyzeCall<MemberAccessExpressionSyntax>(context, IsProperty);
        }

#if CSHARP
        private static void AnalyzeCSharpIndexer(SyntaxNodeAnalysisContext context)
        {
            AnalyzeCall<ElementAccessExpressionSyntax>(context, IsIndexer);
        }
#endif

#if VISUAL_BASIC
        private static void AnalyzeVBIndexer(SyntaxNodeAnalysisContext context)
        {
            AnalyzeCall<InvocationExpressionSyntax>(context, IsIndexer);
        }
#endif

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

            var invocationParent = FindInvocationInHierarchy(invocationExpression);

            if (invocationParent is null || !IsSetupInvocation(context, invocationParent))
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
            return methodSymbol is object && CallSpecMethods.Contains(methodSymbol.GetFullName());
        }

        private static bool IsProperty(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.Kind == SymbolKind.Property;
        }

        private static bool IsMethod(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.Kind == SymbolKind.Method;
        }

        private static bool IsIndexer(SymbolInfo symbolInfo)
        {
            if (symbolInfo.Symbol?.Kind == SymbolKind.Property)
            {
                var propertySymbol = (IPropertySymbol)symbolInfo.Symbol;
                return propertySymbol.IsIndexer;
            }

            return false;
        }

        private static bool IsInterfaceMember(SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol?.ContainingType?.TypeKind == TypeKind.Interface;
        }

        private static InvocationExpressionSyntax? FindInvocationInHierarchy(SyntaxNode node)
        {
            SyntaxNode? parent = null;
            foreach (var ancestor in ExpectedNodeHierarchy)
            {
                parent = node.Parent;
                if (!ancestor.Type.GetTypeInfo().IsAssignableFrom(parent.GetType().GetTypeInfo()))
                {
                    if (ancestor.Optional)
                    {
                        continue;
                    }

                    return null;
                }

                node = parent;
            }

            return parent as InvocationExpressionSyntax;
        }

        private class Ancestor
        {
            public Ancestor(Type type, bool optional = false)
            {
                this.Type = type;
                this.Optional = optional;
            }

            public Type Type { get; }

            public bool Optional { get; }
        }
    }
}
