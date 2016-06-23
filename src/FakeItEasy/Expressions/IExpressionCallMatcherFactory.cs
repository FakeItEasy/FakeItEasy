namespace FakeItEasy.Expressions
{
    using FakeItEasy.Core;

    internal interface IExpressionCallMatcherFactory
    {
        ICallMatcher CreateCallMathcer(ParsedCallExpression callSpecification);
    }
}
