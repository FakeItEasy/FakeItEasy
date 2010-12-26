namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    internal interface IExpressionCallMatcherFactory
    {
        ICallMatcher CreateCallMathcer(LambdaExpression callSpecification);
    }
}