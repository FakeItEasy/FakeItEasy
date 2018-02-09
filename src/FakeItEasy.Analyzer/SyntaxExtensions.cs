namespace FakeItEasy.Analyzer
{
    using Microsoft.CodeAnalysis;
#if CSHARP
    using Microsoft.CodeAnalysis.CSharp.Syntax;
#elif VISUAL_BASIC
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

    internal static class SyntaxExtensions
    {
#if CSHARP
        /// <summary>
        /// Get the expression from an argument.
        /// </summary>
        /// <param name="argument">The argument</param>
        /// <returns>Its expression.</returns>
        public static ExpressionSyntax GetExpression(this ArgumentSyntax argument)
        {
            return argument.Expression;
        }
#endif

        /// <summary>
        /// Replaces the invocation's arguments with the supplied list of arguments.
        /// Preserves the trivia around the original ArgumentList.
        /// </summary>
        /// <param name="invocation">The invocation whose arguments should be replaced.</param>
        /// <param name="newArguments">The new arguments.</param>
        /// <returns>The updated invocation.</returns>
        public static InvocationExpressionSyntax ReplaceArguments(this InvocationExpressionSyntax invocation, SeparatedSyntaxList<ArgumentSyntax> newArguments)
        {
            // Set arguments into the ArgumentList rather than building a new ArgumentList in order to preserve any trailing trivia.
            var newArgumentList = invocation.ArgumentList.WithArguments(newArguments);
            return invocation.WithArgumentList(newArgumentList);
        }
    }
}
