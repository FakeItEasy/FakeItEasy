namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;
    using System.Reflection;

    internal class StartConfiguration<TFake>
        : IStartConfiguration<TFake>, IHideObjectMembers
    {
        private FakeObject fakeObject;
        private ExpressionCallRule.Factory callRuleFactory;
        private IConfigurationFactory configurationFactory;
        private IProxyGenerator proxyGenerator;

        internal StartConfiguration(FakeObject fakeObject, ExpressionCallRule.Factory callRuleFactory, IConfigurationFactory configurationFactory, IProxyGenerator proxyGenerator)
        {
            this.fakeObject = fakeObject;
            this.callRuleFactory = callRuleFactory;
            this.configurationFactory = configurationFactory;
            this.proxyGenerator = proxyGenerator;
        }

        public IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var rule = this.callRuleFactory(callSpecification);
            this.fakeObject.AddRule(rule);
            return this.configurationFactory.CreateConfiguration<TMember>(this.fakeObject, rule);
        }

        public IVoidArgumentValidationConfiguration CallsTo(Expression<Action<TFake>> callSpecification)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            this.AssertThatMemberCanBeIntercepted(callSpecification);

            var rule = this.callRuleFactory(callSpecification);
            rule.Applicator = x => { };
            this.fakeObject.AddRule(rule);
            return this.configurationFactory.CreateConfiguration(this.fakeObject, rule);
        }

        private void AssertThatMemberCanBeIntercepted(LambdaExpression callSpecification)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                this.AssertThatMemberCanBeIntercepted(methodExpression.Method);
                return;
            }

            var propertyExpression = callSpecification.Body as MemberExpression;
            if (propertyExpression != null)
            {
                this.AssertThatMemberCanBeIntercepted(propertyExpression.Member);
            }
        }

        private void AssertThatMemberCanBeIntercepted(MemberInfo member)
        {
            if (!this.proxyGenerator.MemberCanBeIntercepted(member))
            {
                throw new FakeConfigurationException(ExceptionMessages.MemberCanNotBeIntercepted);            
            }
        }

        public IAnyCallConfiguration AnyCall()
        {
            var rule = new AnyCallCallRule();
            this.fakeObject.AddRule(rule);
            return this.configurationFactory.CreateAnyCallConfiguration(this.fakeObject, rule);
        }
    }
}