namespace FakeItEasy.Analyzer
{
    using System;
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

    public abstract class ArgumentConstraintAnalyzerBase : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.RegisterSyntaxNodeAction(this.AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
        }

        protected abstract bool IsSupportedArgumentConstraintProperty(string fullName);

        protected abstract void AnalyzeArgumentConstraintCore(string propertyFullName, SyntaxNodeAnalysisContext context, SyntaxNode completeConstraint);

        private static SyntaxNode GetCompleteConstraint(SyntaxNode node)
        {
            while (node.Parent.Kind() == SyntaxKind.SimpleMemberAccessExpression || node.Parent.Kind() == SyntaxKind.InvocationExpression)
            {
                node = node.Parent;
            }

            return node;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
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

            if (!this.IsSupportedArgumentConstraintProperty(propertyFullName))
            {
                return;
            }

            var completeConstraint = GetCompleteConstraint(context.Node);

            this.AnalyzeArgumentConstraintCore(propertyFullName, context, completeConstraint);
        }
    }
}
