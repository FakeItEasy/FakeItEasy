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

            var parsedCallExpression = this.expressionParser.Parse(callSpecification, this.manager.Object);
            this.GuardAgainstWrongFake(parsedCallExpression.CallTarget);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var rule = this.callRuleFactory(parsedCallExpression);
            return this.configurationFactory.CreateConfiguration<TMember>(this.manager, rule);
        }

        public IVoidArgumentValidationConfiguration CallsTo(Expression<Action<TFake>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, nameof(callSpecification));

            var parsedCallExpression = this.expressionParser.Parse(callSpecification, this.manager.Object);
            this.GuardAgainstWrongFake(parsedCallExpression.CallTarget);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var rule = this.callRuleFactory(parsedCallExpression);
            return this.configurationFactory.CreateConfiguration(this.manager, rule);
        }

        public IPropertySetterAnyValueConfiguration<TValue> CallsToSet<TValue>(Expression<Func<TFake, TValue>> propertySpecification)
        {
            Guard.AgainstNull(propertySpecification, nameof(propertySpecification));

            var parsedCallExpression = this.expressionParser.Parse(propertySpecification, this.manager.Object);
            this.GuardAgainstWrongFake(parsedCallExpression.CallTarget);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var parsedSetterCallExpression = PropertyExpressionHelper.BuildSetterFromGetter<TValue>(parsedCallExpression);

            return new PropertySetterConfiguration<TValue>(
                parsedSetterCallExpression,
                this.CreateVoidArgumentValidationConfiguration);
        }

        public IAnyCallConfigurationWithNoReturnTypeSpecified AnyCall()
        {
            var rule = new AnyCallCallRule();
            return this.configurationFactory.CreateAnyCallConfiguration(this.manager, rule);
        }

        private void AssertThatMemberCanBeIntercepted(ParsedCallExpression parsedCall)
        {
            this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(parsedCall.CalledMethod, this.manager.Object);
        }

        private void GuardAgainstWrongFake(object? callTarget)
        {
            if (callTarget != this.manager.Object)
            {
                throw new ArgumentException("The target of this call is not the fake object being configured.");
            }
        }

        private IVoidArgumentValidationConfiguration CreateVoidArgumentValidationConfiguration(ParsedCallExpression parsedCallExpression)
        {
            var rule = this.callRuleFactory.Invoke(parsedCallExpression);
            return this.configurationFactory.CreateConfiguration(this.manager, rule);
        }
    }
}
