using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Core;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests.TestHelpers
{
    internal static class ExpressionHelper
    {
        public static Expression<Action<T>> CreateExpression<T>(Expression<Action<T>> expression)
        {
            return expression;
        }

        public static Expression<Func<T, TReturn>> CreateExpression<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return expression;
        }

        public static ExpressionCallRule CreateRule<TFake, TReturn>(Expression<Func<TFake, TReturn>> expression)
        {
            return GetCallRuleFactory().Invoke(expression);
        }

        public static ExpressionCallRule CreateRule<TFake>(Expression<Action<TFake>> expression)
        {
            return GetCallRuleFactory().Invoke(expression);
        }

        public static ICallMatcher CreateMatcher<TFake, TReturn>(Expression<Func<TFake, TReturn>> expression)
        {
            return ServiceLocator.Current.Resolve<IExpressionCallMatcherFactory>().CreateCallMathcer(expression);
        }

        public static ICallMatcher CreateMatcher<TFake>(Expression<Action<TFake>> expression)
        {
            return ServiceLocator.Current.Resolve<IExpressionCallMatcherFactory>().CreateCallMathcer(expression);
        }

        public static IInterceptedFakeObjectCall CreateFakeCall<TFake, Treturn>(Expression<Func<TFake, Treturn>> callSpecification)
        {
            return CreateFakeCall(A.Fake<TFake>(), (LambdaExpression)callSpecification);
        }

        public static IInterceptedFakeObjectCall CreateFakeCall<TFake>(Expression<Action<TFake>> callSpecification)
        {
            return CreateFakeCall(A.Fake<TFake>(), (LambdaExpression)callSpecification);
        }

        public static Expression GetArgumentExpression<T>(Expression<Action<T>> callSpecification, int argumentIndex)
        {
            return GetArgumentExpression((LambdaExpression)callSpecification, argumentIndex);
        }

        public static Expression GetArgumentExpression<T, TReturn>(Expression<Func<T, TReturn>> callSpecification, int argumentIndex)
        {
            return GetArgumentExpression((LambdaExpression)callSpecification, argumentIndex);
        }

        private static ExpressionCallRule.Factory GetCallRuleFactory()
        {
            return ServiceLocator.Current.Resolve<ExpressionCallRule.Factory>();
        }

        private static Expression GetArgumentExpression(LambdaExpression callSpecification, int argumentIndex)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            return methodExpression.Arguments[argumentIndex];
        }

        private static IInterceptedFakeObjectCall CreateFakeCall<TFake>(TFake fakedObject, LambdaExpression callSpecification)
        {
            var result = A.Fake<IInterceptedFakeObjectCall>();
            var frozen = A.Fake<ICompletedFakeObjectCall>();

            A.CallTo(() => result.Method).Returns(GetMethodInfo(callSpecification));
            A.CallTo(() => frozen.Method).Returns(GetMethodInfo(callSpecification));

            A.CallTo(() => result.FakedObject).Returns(fakedObject);
            A.CallTo(() => frozen.FakedObject).Returns(fakedObject);

            A.CallTo(() => result.Arguments).Returns(CreateArgumentCollection(fakedObject, callSpecification));
            A.CallTo(() => frozen.Arguments).Returns(CreateArgumentCollection(fakedObject, callSpecification));

            A.CallTo(() => frozen.ReturnValue)
                .Returns(() => Fake.GetCalls(result).Matching<IInterceptedFakeObjectCall>(x => x.SetReturnValue(A<object>.Ignored)).Last().Arguments[0]);

            A.CallTo(() => result.AsReadOnly()).Returns(frozen);

            return result;
        }

        private static MethodInfo GetMethodInfo(LambdaExpression callSpecification)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return methodExpression.Method;
            }

            var memberExpression = callSpecification.Body as MemberExpression;
            var property = memberExpression.Member as PropertyInfo;
            return property.GetGetMethod(true);
        }


        private static MethodCallExpression GetMethodExpression(LambdaExpression callSpecification)
        {
            return callSpecification.Body as MethodCallExpression;
        }

        private static ArgumentCollection CreateArgumentCollection<TFake>(TFake fake, LambdaExpression callSpecification)
        {
            var methodCall = callSpecification.Body as MethodCallExpression;
            
            MethodInfo method = null;
            object[] arguments = null;

            if (methodCall != null)
            {
                method = methodCall.Method;
                arguments =
                    (from argument in methodCall.Arguments
                     select ExpressionManager.GetValueProducedByExpression(argument)).ToArray();
            }
            else
            {
                var propertyCall = callSpecification.Body as MemberExpression;
                var property = propertyCall.Member as PropertyInfo;

                method = property.GetGetMethod(true);
                arguments = new object[] { };
            }
            
            return new ArgumentCollection(arguments, method);

        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> methodAccess)
        {
            var methodExpression = methodAccess.Body as MethodCallExpression;
            return methodExpression.Method;
        }
    }

    /// <summary>
    /// Helps when configuring calls with output parameters.
    /// </summary>
    internal static class Null<T>
    {
        static public T Out;
    }
}