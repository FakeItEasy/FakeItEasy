namespace FakeItEasy.Expressions
{
    using FakeItEasy.Core;

    internal interface IExpressionCallMatcherFactory
    {
        ICallMatcher CreateCallMatcher(ParsedCallExpression callSpecification);
    }
}
