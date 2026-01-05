namespace FakeItEasy.Tests.TestHelpers;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy.Configuration;
using FakeItEasy.Core;

internal static class ExpressionHelper
{
    public static Expression<Action<T>> CreateExpression<T>(Expression<Action<T>> expression)
    {
        return expression;
    }

    public static IInterceptedFakeObjectCall CreateFakeCall<TFake, TReturn>(Expression<Func<TFake, TReturn>> callSpecification)
        where TFake : class
    {
        return CreateFakeCall(A.Fake<TFake>(), callSpecification);
    }

    public static IInterceptedFakeObjectCall CreateFakeCall<TFake>(Expression<Action<TFake>> callSpecification)
        where TFake : class
    {
        return CreateFakeCall(A.Fake<TFake>(), callSpecification);
    }

    public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> callSpecification) =>
        GetMethodInfo((LambdaExpression)callSpecification);

    public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> callSpecification) =>
        GetMethodInfo((LambdaExpression)callSpecification);

    private static IInterceptedFakeObjectCall CreateFakeCall<TFake>(TFake fakedObject, LambdaExpression callSpecification)
        where TFake : class
    {
        var arguments = CreateArgumentCollection(callSpecification);
        var result = A.Fake<IInterceptedFakeObjectCall>();

        A.CallTo(() => result.Method).Returns(arguments.Method);
        A.CallTo(() => result.FakedObject).Returns(fakedObject);
        A.CallTo(() => result.Arguments).Returns(arguments);

        return result;
    }

    private static MethodInfo GetMethodInfo(LambdaExpression callSpecification)
    {
        if (callSpecification.Body is MethodCallExpression methodExpression)
        {
            return methodExpression.Method;
        }

        var memberExpression = (MemberExpression)callSpecification.Body;
        var property = (PropertyInfo)memberExpression.Member;
        return property.GetGetMethod(true)
               ?? throw new ArgumentException("Expression does not represent a method call or propery getter.", nameof(callSpecification));
    }

    private static ArgumentCollection CreateArgumentCollection(LambdaExpression callSpecification)
    {
        var arguments = callSpecification.Body is MethodCallExpression methodCall
            ? methodCall.Arguments.Select(argument => argument.Evaluate()).ToArray()
            : Array.Empty<object>();

        return new ArgumentCollection(arguments, GetMethodInfo(callSpecification));
    }
}
