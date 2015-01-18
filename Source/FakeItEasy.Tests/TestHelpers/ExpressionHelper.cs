namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

    internal static class ExpressionHelper
    {
        public static Expression<Action<T>> CreateExpression<T>(Expression<Action<T>> expression)
        {
            return expression;
        }

        public static ExpressionCallRule CreateRule<TFake>(Expression<Action<TFake>> expression)
        {
            return GetCallRuleFactory().Invoke(expression);
        }

        public static IInterceptedFakeObjectCall CreateFakeCall<TFake, TReturn>(Expression<Func<TFake, TReturn>> callSpecification)
        {
            return CreateFakeCall(A.Fake<TFake>(), callSpecification);
        }

        public static IInterceptedFakeObjectCall CreateFakeCall<TFake>(Expression<Action<TFake>> callSpecification)
        {
            return CreateFakeCall(A.Fake<TFake>(), callSpecification);
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> methodAccess)
        {
            var methodExpression = (MethodCallExpression)methodAccess.Body;
            return methodExpression.Method;
        }

        private static ExpressionCallRule.Factory GetCallRuleFactory()
        {
            return ServiceLocator.Current.Resolve<ExpressionCallRule.Factory>();
        }

        private static IInterceptedFakeObjectCall CreateFakeCall<TFake>(TFake fakedObject, LambdaExpression callSpecification)
        {
            var result = A.Fake<IInterceptedFakeObjectCall>();
            var frozen = A.Fake<ICompletedFakeObjectCall>();

            A.CallTo(() => result.Method).Returns(GetMethodInfo(callSpecification));
            A.CallTo(() => frozen.Method).Returns(GetMethodInfo(callSpecification));

            A.CallTo(() => result.FakedObject).Returns(fakedObject);
            A.CallTo(() => frozen.FakedObject).Returns(fakedObject);

            A.CallTo(() => result.Arguments).Returns(CreateArgumentCollection(callSpecification));
            A.CallTo(() => frozen.Arguments).Returns(CreateArgumentCollection(callSpecification));

            A.CallTo(() => frozen.ReturnValue)
                .ReturnsLazily(x => Fake.GetCalls(result).Matching<IInterceptedFakeObjectCall>(c => c.SetReturnValue(A<object>._)).Last().Arguments[0]);

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

            var memberExpression = (MemberExpression)callSpecification.Body;
            var property = (PropertyInfo)memberExpression.Member;
            return property.GetGetMethod(true);
        }

        private static ArgumentCollection CreateArgumentCollection(LambdaExpression callSpecification)
        {
            var methodCall = callSpecification.Body as MethodCallExpression;

            MethodInfo method;
            object[] arguments;

            if (methodCall != null)
            {
                method = methodCall.Method;
                arguments =
                    (from argument in methodCall.Arguments
                     select argument.Evaluate()).ToArray();
            }
            else
            {
                var propertyCall = (MemberExpression)callSpecification.Body;
                var property = (PropertyInfo)propertyCall.Member;

                method = property.GetGetMethod(true);
                arguments = new object[] { };
            }

            return new ArgumentCollection(arguments, method);
        }
    }
}