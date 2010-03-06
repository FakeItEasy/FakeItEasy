namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Expressions;
using FakeItEasy.Core;

    internal class FakeConfigurationManager
            : IFakeConfigurationManager
    {
        private IConfigurationFactory configurationFactory;
        private IExpressionParser expressionParser;
        private ExpressionCallRule.Factory ruleFactory;
        private IProxyGenerator proxyGenerator;

        public FakeConfigurationManager(IConfigurationFactory configurationFactory, IExpressionParser parser, ExpressionCallRule.Factory callRuleFactory, IProxyGenerator proxyGenerator)
        {
            this.configurationFactory = configurationFactory;
            this.expressionParser = parser;
            this.ruleFactory = callRuleFactory;
            this.proxyGenerator = proxyGenerator;
        }

        public IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var fake = this.expressionParser.GetFakeObjectCallIsMadeOn(callSpecification);
            var rule = this.ruleFactory.Invoke(callSpecification);

            fake.AddRule(rule);

            return this.configurationFactory.CreateConfiguration(fake, rule);
        }

        public IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var fake = this.expressionParser.GetFakeObjectCallIsMadeOn(callSpecification);
            var rule = this.ruleFactory.Invoke(callSpecification);

            fake.AddRule(rule);

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
