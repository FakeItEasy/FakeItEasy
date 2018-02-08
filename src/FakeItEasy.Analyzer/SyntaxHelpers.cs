namespace FakeItEasy.Analyzer
{
#if CSHARP
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
#elif VISUAL_BASIC
    using Microsoft.CodeAnalysis.VisualBasic;
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

    internal static class SyntaxHelpers
    {
        /// <summary>
        /// Create a simple member access expression.
        /// </summary>
        /// <param name="expression">The expression that defines the object whose member will be accessed.</param>
        /// <param name="name">The name of the member to access.</param>
        /// <returns>A new member access expression syntax object.</returns>
        public static MemberAccessExpressionSyntax SimpleMemberAccess(ExpressionSyntax expression, SimpleNameSyntax name)
        {
#if CSHARP
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
#elif VISUAL_BASIC
            return SyntaxFactory.SimpleMemberAccessExpression(expression, name);
#endif
        }

#if VISUAL_BASIC
        /// <summary>
        /// Create an argument from an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>A new simple argument.</returns>
        public static ArgumentSyntax Argument(ExpressionSyntax expression)
        {
            return SyntaxFactory.SimpleArgument(expression);
        }
#endif
    }
}
