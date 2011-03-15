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
        private readonly ICallExpressionParser callExpressionParser;
        private readonly ExpressionCallRule.Factory ruleFactory;

        public FakeConfigurationManager(IConfigurationFactory configurationFactory, IExpressionParser parser, ExpressionCallRule.Factory callRuleFactory, IProxyGenerator proxyGenerator, ICallExpressionParser callExpressionParser)
        {
            this.configurationFactory = configurationFactory;
            this.expressionParser = parser;
            this.ruleFactory = callRuleFactory;
            this.proxyGenerator = proxyGenerator;
            this.callExpressionParser = callExpressionParser;
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
            var calledMethod = this.callExpressionParser.Parse(callSpecification).CalledMethod;

            if (!this.proxyGenerator.MemberCanBeIntercepted(calledMethod))
            {
                throw new FakeConfigurationException(ExceptionMessages.MemberCanNotBeIntercepted);
            }
        }
    }
}