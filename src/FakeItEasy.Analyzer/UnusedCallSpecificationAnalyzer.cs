namespace FakeItEasy.Analyzer
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedCallSpecificationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FakeItEasy001";
        internal const string Category = "Usage";

        internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.UnusedCallSpecificationTitle), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.UnusedCallSpecificationMessageFormat), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.UnusedCallSpecificationDescription), Resources.ResourceManager, typeof(Resources));

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.RegisterSyntaxNodeAction(this.AnalyzeCall, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeCall(SyntaxNodeAnalysisContext context)
        {
            //// Check if it's a call to A.CallTo

            var call = context.Node as InvocationExpressionSyntax;
            var memberAccess = call?.Expression as MemberAccessExpressionSyntax;

            var expr = memberAccess?.Expression as IdentifierNameSyntax;
            if (expr == null)
            {
                return;
            }

            var member = memberAccess.Name as IdentifierNameSyntax;
            if (member == null)
            {
                return;
            }

            var typeSymbol = context.SemanticModel.GetSymbolInfo(expr).Symbol as INamedTypeSymbol;
            var methodSymbol = context.SemanticModel.GetSymbolInfo(member).Symbol as IMethodSymbol;

            if (typeSymbol?.GetFullName() == "FakeItEasy.A" && methodSymbol?.Name == "CallTo")
            {
                if (call.Parent is StatementSyntax)
                {
                    var firstArg = call.ArgumentList.Arguments.FirstOrDefault();
                    var lambda = firstArg?.Expression as LambdaExpressionSyntax;
                    var callSpec = lambda?.Body?.ToString()
                                   ?? firstArg?.ToString()
                                   ?? "(unknown)";
                    var diagnostic = Diagnostic.Create(Rule, call.GetLocation(), callSpec);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
