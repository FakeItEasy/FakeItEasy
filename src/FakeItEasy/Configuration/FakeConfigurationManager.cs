namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Expressions;
    using FakeItEasy.Core;

    internal class FakeConfigurationManager
        : IFakeConfigurationManager
    {
        private readonly IConfigurationFactory configurationFactory;
        private readonly IExpressionParser expressionParser;
        private readonly ICallExpressionParser callExpressionParser;
        private readonly IInterceptionAsserter interceptionAsserter;
        private readonly ExpressionCallRule.Factory ruleFactory;

        public FakeConfigurationManager(IConfigurationFactory configurationFactory, IExpressionParser parser, ExpressionCallRule.Factory callRuleFactory, ICallExpressionParser callExpressionParser, IInterceptionAsserter interceptionAsserter)
        {
            this.configurationFactory = configurationFactory;
            this.expressionParser = parser;
            this.ruleFactory = callRuleFactory;
            this.callExpressionParser = callExpressionParser;
            this.interceptionAsserter = interceptionAsserter;
        }

        public IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            Guard.AgainstNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            return this.CreateVoidArgumentValidationConfiguration(callSpecification);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var fake = this.expressionParser.GetFakeManagerCallIsMadeOn(callSpecification);
            var rule = this.ruleFactory.Invoke(callSpecification);

            fake.AddRuleFirst(rule);

            return this.configurationFactory.CreateConfiguration<T>(fake, rule);
        }

        public IAnyCallConfigurationWithNoReturnTypeSpecified CallTo(object fakeObject)
        {
            var rule = new AnyCallCallRule();
            var manager = Fake.GetFakeManager(fakeObject);
            manager.AddRuleFirst(rule);

            return this.configurationFactory.CreateAnyCallConfiguration(manager, rule);
        }

        public IPropertySetterAnyValueConfiguration<TValue> CallToSet<TValue>(Expression<Func<TValue>> propertySpecification)
        {
            Guard.AgainstNull(propertySpecification, nameof(propertySpecification));
            this.AssertThatMemberCanBeIntercepted(propertySpecification);

            var setterExpression = BuildSetterFromGetter(propertySpecification);

            return new PropertySetterConfiguration<TValue>(setterExpression, this.CreateVoidArgumentValidationConfiguration);
        }

        private static string GetPropertyName(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression != null &&
                HasThis(methodCallExpression) &&
                methodCallExpression.Method.IsSpecialName)
            {
                var methodName = methodCallExpression.Method.Name;
                if (methodName.StartsWith("get_", StringComparison.Ordinal))
                {
                    return methodName.Substring(4);
                }
            }

            return null;
        }

        private static bool HasThis(MethodCallExpression methodCallExpression)
        {
            return (methodCallExpression.Method.CallingConvention & CallingConventions.HasThis) ==
                   CallingConventions.HasThis;
        }

        private static MethodCallExpression BuildSetterFromMemberExpression<TValue>(MemberExpression memberExpression)
        {
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The expression refers to '" + memberExpression.Member.Name +
                                            "', which is a field, not a property getter.");
            }

            var propertySetterInfo = propertyInfo.GetSetMethod(nonPublic: true);
            if (propertySetterInfo == null)
            {
                throw new ArgumentException("The property '" + memberExpression.Member.Name + "' does not have a setter.");
            }

            var arguments = new[] { BuildArgumentThatMatchesAnything<TValue>() };

            return Expression.Call(memberExpression.Expression, propertySetterInfo, arguments);
        }

        private static Expression BuildArgumentThatMatchesAnything<TValue>()
        {
            Expression<Func<TValue>> value = () => A<TValue>.Ignored;
            return value.Body;
        }

        private IVoidArgumentValidationConfiguration CreateVoidArgumentValidationConfiguration(LambdaExpression lambda)
        {
            var fake = this.expressionParser.GetFakeManagerCallIsMadeOn(lambda);
            var rule = this.ruleFactory.Invoke(lambda);
            fake.AddRuleFirst(rule);

            return this.configurationFactory.CreateConfiguration(fake, rule);
        }

        private string GetExpressionDescription<TValue>(Expression<Func<TValue>> expression)
        {
            var matcher = new ExpressionCallMatcher(
                expression,
                ServiceLocator.Current.Resolve<ExpressionArgumentConstraintFactory>(),
                ServiceLocator.Current.Resolve<MethodInfoManager>(),
                this.callExpressionParser);

            var expressionDescription = matcher.DescriptionOfMatchingCall;
            return expressionDescription;
        }

        private MethodCallExpression BuildSetterFromGetter<TValue>(Expression<Func<TValue>> propertySpecification)
        {
            var memberExpression = propertySpecification.Body as MemberExpression;
            return memberExpression == null
                ? BuildSetterFromMethodCall(propertySpecification)
                : BuildSetterFromMemberExpression<TValue>(memberExpression);
        }

        private MethodCallExpression BuildSetterFromMethodCall<TValue>(Expression<Func<TValue>> propertySpecification)
        {
            var methodCallExpression = propertySpecification.Body as MethodCallExpression;
            var indexerName = GetPropertyName(methodCallExpression);
            if (indexerName == null)
            {
                var expressionDescription = GetExpressionDescription(propertySpecification);
                throw new ArgumentException("Expression '" + expressionDescription +
                                            "' must refer to a property or indexer getter, but doesn't.");
            }

            var parameterTypes = methodCallExpression.Method.GetParameters()
                .Select(p => p.ParameterType)
                .Concat(new[] { methodCallExpression.Method.ReturnType })
                .ToArray();
            var instance = methodCallExpression.Object;
            var indexerSetterInfo = instance.Type.GetMethod("set_" + indexerName, parameterTypes);

            if (indexerSetterInfo == null)
            {
                var expressionDescription = GetExpressionDescription(propertySpecification);
                throw new ArgumentException("Expression '" + expressionDescription +
                                            "' refers to an indexed property that does not have a setter.");
            }

            var arguments = methodCallExpression.Arguments.Concat(new[] { BuildArgumentThatMatchesAnything<TValue>() });

            return Expression.Call(instance, indexerSetterInfo, arguments);
        }

        private void AssertThatMemberCanBeIntercepted(LambdaExpression callSpecification)
        {
            var parsed = this.callExpressionParser.Parse(callSpecification);
            this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsed.CalledMethod,
                parsed.CallTarget);
        }
    }
}
