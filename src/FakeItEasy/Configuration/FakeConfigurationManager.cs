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
        private readonly ICallExpressionParser callExpressionParser;
        private readonly IInterceptionAsserter interceptionAsserter;
        private readonly ExpressionCallRule.Factory ruleFactory;

        public FakeConfigurationManager(IConfigurationFactory configurationFactory, ExpressionCallRule.Factory callRuleFactory, ICallExpressionParser callExpressionParser, IInterceptionAsserter interceptionAsserter)
        {
            this.configurationFactory = configurationFactory;
            this.ruleFactory = callRuleFactory;
            this.callExpressionParser = callExpressionParser;
            this.interceptionAsserter = interceptionAsserter;
        }

        public IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            Guard.AgainstNull(callSpecification, "callSpecification");

            var parsedCallExpression = this.callExpressionParser.Parse(callSpecification);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            return this.CreateVoidArgumentValidationConfiguration(parsedCallExpression);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, "callSpecification");

            var parsedCallExpression = this.callExpressionParser.Parse(callSpecification);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var fake = GetFakeManagerCallIsMadeOn(parsedCallExpression);
            var rule = this.ruleFactory.Invoke(parsedCallExpression);

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
            var parsedCallExpression = this.callExpressionParser.Parse(propertySpecification);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var setterExpression = BuildSetterFromGetter(parsedCallExpression, propertySpecification);

            return new PropertySetterConfiguration<TValue>(
                setterExpression,
                lambda => this.CreateVoidArgumentValidationConfiguration(this.callExpressionParser.Parse(lambda)));
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

        private static FakeManager GetFakeManagerCallIsMadeOn(ParsedCallExpression parsedCallExpression)
        {
            if (parsedCallExpression.CallTarget == null)
            {
                throw new ArgumentException("The specified call is not made on a fake object.");
            }

            return Fake.GetFakeManager(parsedCallExpression.CallTarget);
        }

        private IVoidArgumentValidationConfiguration CreateVoidArgumentValidationConfiguration(ParsedCallExpression parsedCallExpression)
        {
            var fake = GetFakeManagerCallIsMadeOn(parsedCallExpression);
            var rule = this.ruleFactory.Invoke(parsedCallExpression);
            fake.AddRuleFirst(rule);

            return this.configurationFactory.CreateConfiguration(fake, rule);
        }

        private string GetExpressionDescription(ParsedCallExpression parsedCallExpression)
        {
            var matcher = new ExpressionCallMatcher(
                parsedCallExpression,
                ServiceLocator.Current.Resolve<ExpressionArgumentConstraintFactory>(),
                ServiceLocator.Current.Resolve<MethodInfoManager>());

            return matcher.DescriptionOfMatchingCall;
        }

        private MethodCallExpression BuildSetterFromGetter<TValue>(
            ParsedCallExpression parsedCallExpression,
            Expression<Func<TValue>> propertySpecification)
        {
            var memberExpression = propertySpecification.Body as MemberExpression;
            return memberExpression == null
                ? BuildSetterFromMethodCall(parsedCallExpression, propertySpecification)
                : BuildSetterFromMemberExpression<TValue>(memberExpression);
        }

        private MethodCallExpression BuildSetterFromMethodCall<TValue>(
            ParsedCallExpression parsedCallExpression,
            Expression<Func<TValue>> propertySpecification)
        {
            var methodCallExpression = propertySpecification.Body as MethodCallExpression;
            var indexerName = GetPropertyName(methodCallExpression);
            if (indexerName == null)
            {
                var expressionDescription = this.GetExpressionDescription(parsedCallExpression);
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
                var expressionDescription = this.GetExpressionDescription(parsedCallExpression);
                throw new ArgumentException("Expression '" + expressionDescription +
                                            "' refers to an indexed property that does not have a setter.");
            }

            var arguments = methodCallExpression.Arguments.Concat(new[] { BuildArgumentThatMatchesAnything<TValue>() });

            return Expression.Call(instance, indexerSetterInfo, arguments);
        }

        private void AssertThatMemberCanBeIntercepted(ParsedCallExpression parsed)
        {
            this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsed.CalledMethod,
                parsed.CallTarget);
        }
    }
}
