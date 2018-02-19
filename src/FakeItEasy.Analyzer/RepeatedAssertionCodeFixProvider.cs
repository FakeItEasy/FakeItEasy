namespace FakeItEasy.Analyzer
{
    using System.Collections.Immutable;
    using System.Globalization;
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

    using static FakeItEasy.Analyzer.SyntaxHelpers;
#if CSHARP
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
#elif VISUAL_BASIC
    using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;
#endif

#if CSHARP
    [ExportCodeFixProvider(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
#endif
    public class RepeatedAssertionCodeFixProvider : CodeFixProvider
    {
        private static readonly Task CompletedTask = Task.FromResult(false);

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(DiagnosticDefinitions.RepeatedAssertion.Id);

        private static string ChangeAssertionToNotUseRepeatedCodeFixTitle =>
            DiagnosticDefinitions.ResourceManager.GetString(nameof(ChangeAssertionToNotUseRepeatedCodeFixTitle), CultureInfo.CurrentUICulture);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault();
            if (diagnostic?.Descriptor.Id == DiagnosticDefinitions.RepeatedAssertion.Id)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ChangeAssertionToNotUseRepeatedCodeFixTitle,
                        ct => ChangeAssertion(context, diagnostic, ct),
                        ChangeAssertionToNotUseRepeatedCodeFixTitle),
                    diagnostic);
            }

            return CompletedTask;
        }

        private static async Task<Document> ChangeAssertion(
            CodeFixContext context,
            Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            // The complete assertion call, e.g. A.CallTo(() => fake.Method()).MustHaveHappened(Repeated.Exactly.Once).
            var mustHaveHappenedNode = GetMustHaveHappened(diagnostic, root);

            var repeatedNode = GetRepeatedNode(mustHaveHappenedNode);
            var repeatedType = GetRepeatedType(repeatedNode);

            InvocationExpressionSyntax newAssertionNode = mustHaveHappenedNode;
            switch (repeatedType)
            {
                case "Never":
                    newAssertionNode = RenameAssertion(newAssertionNode, "MustNotHaveHappened");
                    newAssertionNode = newAssertionNode.WithArgumentList(ArgumentList());
                    break;
                case "Once":
                case "Twice":
                    var comparisonType = GetComparisonType(repeatedNode);
                    newAssertionNode = RenameAssertion(newAssertionNode, $"MustHaveHappened{repeatedType}{comparisonType}");
                    newAssertionNode = newAssertionNode.WithArgumentList(ArgumentList());
                    break;
                case "Like":
                    var mustHaveHappenedANumberOfTimesMatchingArguments = BuildMustHaveHappenedANumberOfTimesMatchingArgumentsFromRepeated(repeatedNode);
                    newAssertionNode = RenameAssertion(newAssertionNode, "MustHaveHappenedANumberOfTimesMatching");
                    newAssertionNode = newAssertionNode.ReplaceArguments(mustHaveHappenedANumberOfTimesMatchingArguments);
                    break;
                case "Times":
                    var mustHaveHappenedArguments = BuildNewMustHaveHappenedArgumentsFromRepeated(repeatedNode);
                    newAssertionNode = newAssertionNode.ReplaceArguments(mustHaveHappenedArguments);
                    break;
            }

            var newRoot = root.ReplaceNode(mustHaveHappenedNode, newAssertionNode);
            return context.Document.WithSyntaxRoot(newRoot);
        }

        private static SeparatedSyntaxList<ArgumentSyntax> BuildNewMustHaveHappenedArgumentsFromRepeated(ArgumentSyntax repeatedNode)
        {
            // The original argument was Repeated.<comparisonType>.Times.
            var comparisonType = GetComparisonType(repeatedNode);

            // Fetch the repeated nodes's expression, which will be the Times call, and grab its argument for MustHaveHappened.
            // This is the call count specified in the assertion.
            var timesArgumentList = ((InvocationExpressionSyntax)repeatedNode.GetExpression()).ArgumentList;

            return SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
                {
                    timesArgumentList.Arguments[0],
                    Token(SyntaxKind.CommaToken),
                    Argument(SimpleMemberAccess(IdentifierName("Times"), IdentifierName(comparisonType)))
                });
        }

        private static SeparatedSyntaxList<ArgumentSyntax> BuildMustHaveHappenedANumberOfTimesMatchingArgumentsFromRepeated(ArgumentSyntax repeatedNode)
        {
            // The repeated node is something like Repeated.Like(n => n % 2 == 0).
            // Fetch its expression, which will be the Like call, and grab its argument (there's only one) to use as the
            // new assertion's call count predicate.
            return ((InvocationExpressionSyntax)repeatedNode.GetExpression()).ArgumentList.Arguments;
        }

        private static InvocationExpressionSyntax RenameAssertion(InvocationExpressionSyntax assertionNode, string newName)
        {
            var newNameNode = IdentifierName(newName);
            return assertionNode.ReplaceNode(GetMethodNameNode(assertionNode), newNameNode);
        }

        private static string GetComparisonType(ArgumentSyntax repeatedNode)
        {
            var memberAccess = GetMemberAccessFromRepeated(repeatedNode);

            // Now we have something like Repeated.Exactly.Once or Repeated.AtLeast.Times.
            // Get the Expression value, which will effectively drop the last component so we can access the middle.
            var modifierName = ((MemberAccessExpressionSyntax)memberAccess.Expression).Name.ToString();
            switch (modifierName)
            {
                case "AtLeast":
                    return "OrMore";
                case "NoMoreThan":
                    return "OrLess";
                default:
                    return modifierName;
            }
        }

        private static MemberAccessExpressionSyntax GetMemberAccessFromRepeated(ArgumentSyntax repatedNode)
        {
            // The repeated node is something like Repeated.Exactly.Once or Repeated.AtLeast.Times(4)
            // First, unwrap the expression from the node. It'll either be an invocation (for Times and Like),
            // in which case we need its expression, or a simple member access like Once or Never.
            var expression = repatedNode.GetExpression();
            return expression is InvocationExpressionSyntax argumentInvocation
                ? (MemberAccessExpressionSyntax)argumentInvocation.Expression
                : (MemberAccessExpressionSyntax)expression;
        }

        private static string GetRepeatedType(ArgumentSyntax repeatedNode)
        {
            var memberAccess = GetMemberAccessFromRepeated(repeatedNode);
            return memberAccess?.Name.ToString();
        }

        private static ArgumentSyntax GetRepeatedNode(InvocationExpressionSyntax assertionNode)
        {
            return assertionNode.ArgumentList.Arguments[0];
        }

        private static IdentifierNameSyntax GetMethodNameNode(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return (IdentifierNameSyntax)memberAccess.Name;
        }

        private static InvocationExpressionSyntax GetMustHaveHappened(Diagnostic diagnostic, SyntaxNode root)
        {
            return (InvocationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
        }
    }
}
