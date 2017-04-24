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
    public class ArgumentConstraintOutsideCallSpecAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> ArgumentConstraintProperties =
            ImmutableHashSet.Create(
                StringComparer.Ordinal,
                "FakeItEasy.A`1._",
                "FakeItEasy.A`1.Ignored",
                "FakeItEasy.A`1.That");

        private static readonly ImmutableHashSet<string> MethodsSupportingArgumentConstraints =
            ImmutableHashSet.Create(
                StringComparer.Ordinal,
                "FakeItEasy.A.CallTo",
                "FakeItEasy.A.CallTo`1",
                "FakeItEasy.Fake`1.CallsTo",
                "FakeItEasy.Fake`1.CallsTo`1",
                "FakeItEasy.Configuration.IPropertySetterAnyValueConfiguration`1.To");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DiagnosticDefinitions.ArgumentConstraintOutsideCallSpec);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.RegisterSyntaxNodeAction(AnalyzeConstraint, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeConstraint(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = context.Node as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                return;
            }

            var propertySymbol = SymbolHelpers.GetAccessedPropertySymbol(memberAccess, context);
            if (propertySymbol == null)
            {
                return;
            }

            var propertyFullName =
                string.Concat(propertySymbol.ContainingType.GetFullName(), ".", propertySymbol.Name);

            if (!ArgumentConstraintProperties.Contains(propertyFullName))
            {
                return;
            }

            if (!IsInArgumentToMethodThatSupportsArgumentConstraints(context.Node, context))
            {
                var descriptor = DiagnosticDefinitions.ArgumentConstraintOutsideCallSpec;
                var completeNode = FindCompleteNode(context.Node);
                var diagnostic = Diagnostic.Create(descriptor, completeNode.GetLocation(), completeNode.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static SyntaxNode FindCompleteNode(SyntaxNode node)
        {
            while (node.Parent.Kind() == SyntaxKind.SimpleMemberAccessExpression || node.Parent.Kind() == SyntaxKind.InvocationExpression)
            {
                node = node.Parent;
            }

            return node;
        }

        private static bool IsInArgumentToMethodThatSupportsArgumentConstraints(SyntaxNode node, SyntaxNodeAnalysisContext context)
        {
            while (node != null)
            {
                switch (node.Kind())
                {
#if CSHARP
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
#elif VISUAL_BASIC
                    case SyntaxKind.GetAccessorBlock:
                    case SyntaxKind.SetAccessorBlock:
                    case SyntaxKind.SubBlock:
                    case SyntaxKind.ConstructorBlock:
#endif
                        break;
                }

                var invocation = node as InvocationExpressionSyntax;
                if (invocation != null && SupportsArgumentConstraints(invocation, context))
                {
                    return true;
                }

                node = node.Parent;
            }

            return false;
        }

        private static bool SupportsArgumentConstraints(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
        {
            var methodSymbol = SymbolHelpers.GetCalledMethodSymbol(invocation, context);
            if (methodSymbol == null)
            {
                return false;
            }

            var methodFullName =
                string.Concat(methodSymbol.ContainingType.GetFullName(), ".", methodSymbol.GetDecoratedName());

            if (MethodsSupportingArgumentConstraints.Contains(methodFullName))
            {
                return methodSymbol.Parameters.Length == 1
                       && (methodSymbol.Parameters[0].Type as INamedTypeSymbol)?.GetFullName() == "System.Linq.Expressions.Expression`1";
            }

            return false;
        }
    }
}
