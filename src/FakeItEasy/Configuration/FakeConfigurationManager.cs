namespace FakeItEasy.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

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
            Guard.AgainstNull(callSpecification, nameof(callSpecification));

            var parsedCallExpression = this.callExpressionParser.Parse(callSpecification);
            GuardAgainstNonFake(parsedCallExpression.CallTarget);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var rule = this.ruleFactory.Invoke(parsedCallExpression);
            var fake = Fake.GetFakeManager(parsedCallExpression.CallTarget!);

            return this.configurationFactory.CreateConfiguration(fake, rule);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            Guard.AgainstNull(callSpecification, nameof(callSpecification));

            var parsedCallExpression = this.callExpressionParser.Parse(callSpecification);
            GuardAgainstNonFake(parsedCallExpression.CallTarget);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var rule = this.ruleFactory.Invoke(parsedCallExpression);
            var fake = Fake.GetFakeManager(parsedCallExpression.CallTarget!);

            return this.configurationFactory.CreateConfiguration<T>(fake, rule);
        }

        public IAnyCallConfigurationWithNoReturnTypeSpecified CallTo(object fakeObject)
        {
            GuardAgainstNonFake(fakeObject);
            var rule = new AnyCallCallRule();
            var manager = Fake.GetFakeManager(fakeObject);

            return this.configurationFactory.CreateAnyCallConfiguration(manager, rule);
        }

        public IPropertySetterAnyValueConfiguration<TValue> CallToSet<TValue>(Expression<Func<TValue>> propertySpecification)
        {
            Guard.AgainstNull(propertySpecification, nameof(propertySpecification));
            var parsedCallExpression = this.callExpressionParser.Parse(propertySpecification);
            GuardAgainstNonFake(parsedCallExpression.CallTarget);
            this.AssertThatMemberCanBeIntercepted(parsedCallExpression);

            var fake = Fake.GetFakeManager(parsedCallExpression.CallTarget!);
            var parsedSetterCallExpression = PropertyExpressionHelper.BuildSetterFromGetter<TValue>(parsedCallExpression);

            return new PropertySetterConfiguration<TValue>(
                parsedSetterCallExpression,
                newParsedSetterCallExpression =>
                    this.CreateVoidArgumentValidationConfiguration(fake, newParsedSetterCallExpression));
        }

        private static void GuardAgainstNonFake(object? target)
        {
            if (target is object)
            {
                Fake.GetFakeManager(target);
            }
        }

        private IVoidArgumentValidationConfiguration CreateVoidArgumentValidationConfiguration(FakeManager fake, ParsedCallExpression parsedCallExpression)
        {
            var rule = this.ruleFactory.Invoke(parsedCallExpression);

            return this.configurationFactory.CreateConfiguration(fake, rule);
        }

        private void AssertThatMemberCanBeIntercepted(ParsedCallExpression parsed)
        {
            this.interceptionAsserter.AssertThatMethodCanBeInterceptedOnInstance(
                parsed.CalledMethod,
                parsed.CallTarget);
        }
    }
}
