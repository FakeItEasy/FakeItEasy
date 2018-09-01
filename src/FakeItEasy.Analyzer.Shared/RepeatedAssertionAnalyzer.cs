namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
#if CSHARP
    using FakeItEasy.Analyzer;
#endif
    using Microsoft.CodeAnalysis;
#if CSHARP
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
#endif
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
#if VISUAL_BASIC
    using Microsoft.CodeAnalysis.VisualBasic;
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

#if CSHARP
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
#endif
    public class RepeatedAssertionAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(DiagnosticDefinitions.RepeatedAssertion);

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
            if (!(context.Node is InvocationExpressionSyntax call) ||
                call.ArgumentList.Arguments.Count != 1 ||
                GetTypeName(context, call.ArgumentList.Arguments[0]) != "FakeItEasy.Repeated")
            {
                return;
            }

            // Abort if the called method isn't MustHaveHappened or if the referenced version of
            // FakeItEasy doesn't support the new assertion API, as evidenced by the absence of the
            // MustHaveHappenedANumberOfTimesMatching method.
            var calledMethod = SymbolHelpers.GetCalledMethodSymbol(call, context);
            if (calledMethod.GetFullName() != "FakeItEasy.Configuration.IAssertConfiguration.MustHaveHappened" ||
                !calledMethod.ContainingType.MemberNames.Contains("MustHaveHappenedANumberOfTimesMatching"))
            {
                return;
            }

            // call is an InvocationExpression whose expression will be a MemberAccessExpression
            // corresponding to the MustHaveHappened call. The text "MustHaveHappened" can be found
            // as that node's Name.
            var memberAccessNode = (MemberAccessExpressionSyntax)call.Expression;
            var mustHaveHappenedNode = memberAccessNode.Name;

            var descriptor = DiagnosticDefinitions.RepeatedAssertion;
            var location = Location.Create(call.SyntaxTree, TextSpan.FromBounds(mustHaveHappenedNode.SpanStart, call.ArgumentList.Span.End));
            context.ReportDiagnostic(Diagnostic.Create(descriptor, location));
        }

        private static string GetTypeName(SyntaxNodeAnalysisContext context, ArgumentSyntax argument)
        {
            return context.SemanticModel.GetTypeInfo(argument.GetExpression()).ConvertedType?.ToString();
        }
    }
}
