namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;
    using FakeItEasy.Expressions;

    internal class StartConfiguration<TFake>
        : IStartConfiguration<TFake>, IHideObjectMembers
    {
        private readonly ExpressionCallRule.Factory callRuleFactory;
        private readonly IConfigurationFactory configurationFactory;
        private readonly FakeManager manager;
        private readonly IProxyGenerator proxyGenerator;
        private readonly ICallExpressionParser expressionParser;

        internal StartConfiguration(FakeManager manager, ExpressionCallRule.Factory callRuleFactory, IConfigurationFactory configurationFactory, IProxyGenerator proxyGenerator, ICallExpressionParser expressionParser)
        {
            this.manager = manager;
            this.callRuleFactory = callRuleFactory;
            this.configurationFactory = configurationFactory;
            this.proxyGenerator = proxyGenerator;
            this.expressionParser = expressionParser;
        }

        public IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var rule = this.callRuleFactory(callSpecification);
            this.manager.AddRuleFirst(rule);
            return this.configurationFactory.CreateConfiguration<TMember>(this.manager, rule);
        }

        public IVoidArgumentValidationConfiguration CallsTo(Expression<Action<TFake>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var rule = this.callRuleFactory(callSpecification);
            rule.Applicator = x => { };
            this.manager.AddRuleFirst(rule);
            return this.configurationFactory.CreateConfiguration(this.manager, rule);
        }

        public IAnyCallConfiguration AnyCall()
        {
            var rule = new AnyCallCallRule();
            this.manager.AddRuleFirst(rule);
            return this.configurationFactory.CreateAnyCallConfiguration(this.manager, rule);
        }

        private void AssertThatMemberCanBeIntercepted(LambdaExpression callSpecification)
        {
            this.AssertThatMemberCanBeIntercepted(this.expressionParser.Parse(callSpecification).CalledMethod);
        }

        private void AssertThatMemberCanBeIntercepted(MethodInfo member)
        {
            if (!this.proxyGenerator.MemberCanBeIntercepted(member))
            {
                throw new FakeConfigurationException(ExceptionMessages.MemberCanNotBeIntercepted);
            }
        }
    }
}