namespace FakeItEasy.Analyzer
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
#if CSHARP
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
#elif VISUAL_BASIC
    using Microsoft.CodeAnalysis.VisualBasic;
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

#if CSHARP
    [ExportCodeFixProvider(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
#endif
    public class ArgumentConstraintNullabilityMismatchCodeFixProvider : CodeFixProvider
    {
        private static readonly Task CompletedTask = Task.FromResult(false);

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(DiagnosticDefinitions.ArgumentConstraintNullabilityMismatch.Id);

        private static string MakeConstraintNullableCodeFixTitle =>
            DiagnosticDefinitions.ResourceManager.GetString(nameof(MakeConstraintNullableCodeFixTitle));

        private static string MakeNotNullConstraintCodeFixTitle =>
            DiagnosticDefinitions.ResourceManager.GetString(nameof(MakeNotNullConstraintCodeFixTitle));

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault();
            if (diagnostic == null)
            {
                return CompletedTask;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    MakeConstraintNullableCodeFixTitle,
                    ct => this.MakeConstraintNullableAsync(context, diagnostic, ct)),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    MakeNotNullConstraintCodeFixTitle,
                    ct => this.MakeNotNullConstraintAsync(context, diagnostic, ct)),
                diagnostic);

            return CompletedTask;
        }

        private static MemberAccessExpressionSyntax GetConstraintNode(Diagnostic diagnostic, SyntaxNode root)
        {
            // getInnermostNodeForTie: true to disambiguate between ArgumentSyntax and
            // MemberAccessExpressionSyntax, which have the same source span. We want the
            // MemberAccessExpressionSyntax node, which is the innermost one.
            return root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as MemberAccessExpressionSyntax;
        }

        private static MemberAccessExpressionSyntax SimpleMemberAccess(ExpressionSyntax expression, SimpleNameSyntax name)
        {
#if CSHARP
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
#elif VISUAL_BASIC
            return SyntaxFactory.SimpleMemberAccessExpression(expression, name);
#endif
        }

        private async Task<Document> MakeConstraintNullableAsync(CodeFixContext context, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken);

            var constraintNode = GetConstraintNode(diagnostic, root);

            // The A<T> type
            var aType = constraintNode?.Expression as GenericNameSyntax;

            // The T type
            var constraintType = aType?.TypeArgumentList.Arguments.FirstOrDefault();
            if (constraintType != null)
            {
                // The T? type
                var nullableConstraintType = SyntaxFactory.NullableType(constraintType);

                // Replace T node with T? and return updated document
                var newRoot = root.ReplaceNode(constraintType, nullableConstraintType);
                return context.Document.WithSyntaxRoot(newRoot);
            }

            return context.Document;
        }

        private async Task<Document> MakeNotNullConstraintAsync(CodeFixContext context, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken);

            var constraintNode = GetConstraintNode(diagnostic, root);

            // The A<T> type
            var aType = constraintNode?.Expression as GenericNameSyntax;

            // The T type
            var constraintType = aType?.TypeArgumentList.Arguments.FirstOrDefault();
            if (constraintType != null)
            {
                // The T? type
                var nullableConstraintType = SyntaxFactory.NullableType(constraintType);

                // The A<?> type
                var nullableAType = aType.ReplaceNode(constraintType, nullableConstraintType);

                // A<T?>.That
                var thatNode =
                    SimpleMemberAccess(
                        nullableAType,
                        SyntaxFactory.IdentifierName("That"));

                // The new A<T?>.That.IsNotNull() constraint
                var newConstraintNode =
                    SyntaxFactory.InvocationExpression(
                        SimpleMemberAccess(
                            thatNode,
                            SyntaxFactory.IdentifierName("IsNotNull")),
                        SyntaxFactory.ArgumentList());

                // Replace node and return updated document
                var newRoot = root.ReplaceNode(constraintNode, newConstraintNode);
                return context.Document.WithSyntaxRoot(newRoot);
            }

            return context.Document;
        }
    }
}
