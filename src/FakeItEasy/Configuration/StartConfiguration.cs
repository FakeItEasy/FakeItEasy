namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

    internal class StartConfiguration<TFake> : IStartConfiguration<TFake>
    {
        private readonly ExpressionCallRule.Factory callRuleFactory;
        private readonly IConfigurationFactory configurationFactory;
        private readonly FakeManager manager;
        private readonly ICallExpressionParser expressionParser;
        private readonly IInterceptionAsserter interceptionAsserter;

        internal StartConfiguration(FakeManager manager, ExpressionCallRule.Factory callRuleFactory, IConfigurationFactory configurationFactory, ICallExpressionParser expressionParser, IInterceptionAsserter interceptionAsserter)
        {
            this.manager = manager;
            this.callRuleFactory = callRuleFactory;
            this.configurationFactory = configurationFactory;
            this.expressionParser = expressionParser;
            this.interceptionAsserter = interceptionAsserter;
        }

        public IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, nameof(callSpecification));

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var rule = this.callRuleFactory(this.expressionParser.Parse(callSpecification));
            this.manager.AddRuleFirst(rule);
            return this.configurationFactory.CreateConfiguration<TMember>(this.manager, rule);
        }

        public IVoidArgumentValidationConfiguration CallsTo(Expression<Action<TFake>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, nameof(callSpecification));

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var rule = this.callRuleFactory(this.expressionParser.Parse(callSpecification));
            rule.UseApplicator(x => { });
            this.manager.AddRuleFirst(rule);
            return this.configurationFactory.CreateConfiguration(this.manager, rule);
        }

        public IAnyCallConfigurationWithNoReturnTypeSpecified AnyCall()
        {
            var rule = new AnyCallCallRule();
            this.manager.AddRuleFirst(rule);
            return this.configurationFactory.CreateAnyCallConfiguration(this.manager, rule);
        }

        private void AssertThatMemberCanBeIntercepted(LambdaExpression callSpecification)
        {
            var parsedCall = this.expressionParser.Parse(callSpecification);
            this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(parsedCall.CalledMethod, this.manager.Object);
        }
    }
}
