namespace FakeItEasy.Analyzer
{
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
    public class ArgumentConstraintOutsideCallSpecAnalyzer : ArgumentConstraintAnalyzerBase
    {
        private static readonly ImmutableHashSet<string> MethodsSupportingArgumentConstraints =
            ImmutableHashSet.Create(
                "FakeItEasy.A.CallTo",
                "FakeItEasy.A.CallTo`1",
                "FakeItEasy.Fake`1.CallsTo",
                "FakeItEasy.Fake`1.CallsTo`1",
                "FakeItEasy.Configuration.IPropertySetterAnyValueConfiguration`1.To");

        private static readonly ImmutableHashSet<string> SupportedArgumentConstraintProperties =
            ImmutableHashSet.Create(
                "FakeItEasy.A`1._",
                "FakeItEasy.A`1.Ignored",
                "FakeItEasy.A`1.That");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DiagnosticDefinitions.ArgumentConstraintOutsideCallSpec);

        protected override bool IsSupportedArgumentConstraintProperty(string fullName) =>
            SupportedArgumentConstraintProperties.Contains(fullName);

        protected override void AnalyzeArgumentConstraintCore(string propertyFullName, SyntaxNodeAnalysisContext context, SyntaxNode completeConstraint)
        {
            if (!IsInArgumentToMethodThatSupportsArgumentConstraints(context.Node, context))
            {
                var descriptor = DiagnosticDefinitions.ArgumentConstraintOutsideCallSpec;
                var diagnostic = Diagnostic.Create(descriptor, completeConstraint.GetLocation(), completeConstraint.ToString());
                context.ReportDiagnostic(diagnostic);
            }
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
            var methodSymbol = SymbolHelpers.GetCalledMethodSymbol(invocation, context, true);
            if (methodSymbol == null)
            {
                return false;
            }

            if (MethodsSupportingArgumentConstraints.Contains(methodSymbol.GetFullName()))
            {
                return methodSymbol.Parameters.Length == 1
                       && (methodSymbol.Parameters[0].Type as INamedTypeSymbol)?.GetFullName() == "System.Linq.Expressions.Expression`1";
            }

            return false;
        }
    }
}
