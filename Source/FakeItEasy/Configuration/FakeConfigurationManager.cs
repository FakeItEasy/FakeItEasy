namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;

    internal class FakeConfigurationManager
        : IFakeConfigurationManager
    {
        private readonly IConfigurationFactory configurationFactory;
        private readonly IExpressionParser expressionParser;
        private readonly IProxyGenerator proxyGenerator;
        private readonly ExpressionCallRule.Factory ruleFactory;

        public FakeConfigurationManager(IConfigurationFactory configurationFactory, IExpressionParser parser, ExpressionCallRule.Factory callRuleFactory, IProxyGenerator proxyGenerator)
        {
            this.configurationFactory = configurationFactory;
            this.expressionParser = parser;
            this.ruleFactory = callRuleFactory;
            this.proxyGenerator = proxyGenerator;
        }

        public IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            Guard.AgainstNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var fake = this.expressionParser.GetFakeManagerCallIsMadeOn(callSpecification);
            var rule = this.ruleFactory.Invoke(callSpecification);

            fake.AddRuleFirst(rule);

            return this.configurationFactory.CreateConfiguration(fake, rule);
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

        private void AssertThatMemberCanBeIntercepted(LambdaExpression callSpecification)
        {
            var methodCall = callSpecification.Body as MethodCallExpression;
            if (methodCall != null && !this.proxyGenerator.MemberCanBeIntercepted(methodCall.Method))
            {
                throw new FakeConfigurationException(ExceptionMessages.MemberCanNotBeIntercepted);
            }

            var propertyCall = callSpecification.Body as MemberExpression;
            if (propertyCall != null && !this.proxyGenerator.MemberCanBeIntercepted(propertyCall.Member))
            {
                throw new FakeConfigurationException("The specified member can not be configured since it can not be intercepted by the current IProxyGenerator.");
            }
        }
    }
}