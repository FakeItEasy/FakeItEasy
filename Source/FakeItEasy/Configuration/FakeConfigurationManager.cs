namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Expressions;

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

        public IAnyCallConfigurationWithNoReturnTypeSpecified CallTo(object fakeObject)
        {
            var rule = new AnyCallCallRule();
            var manager = Fake.GetFakeManager(fakeObject);
            manager.AddRuleFirst(rule);

            return this.configurationFactory.CreateAnyCallConfiguration(manager, rule);
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