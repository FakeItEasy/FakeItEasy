namespace FakeItEasy.Configuration
{
    using System;
    using FakeItEasy.Core;

    internal class RecordingRuleBuilder
        : IRecordingConfigurationWithArgumentValidation
    {
        private readonly RecordedCallRule rule;
        private readonly RuleBuilder wrappedBuilder;

        public RecordingRuleBuilder(RecordedCallRule rule, RuleBuilder wrappedBuilder)
        {
            this.rule = rule;
            this.wrappedBuilder = wrappedBuilder;

            rule.Applicator = x => { };
        }

        public delegate RecordingRuleBuilder Factory(RecordedCallRule rule, FakeManager fakeObject);
        
        public IAfterCallSpecifiedConfiguration DoesNothing()
        {
            return this.wrappedBuilder.DoesNothing();
        }

        public IAfterCallSpecifiedConfiguration Throws(Exception exception)
        {
            return this.wrappedBuilder.Throws(exception);
        }

        public IVoidConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            return this.wrappedBuilder.Invokes(action);
        }

        public IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            return this.wrappedBuilder.CallsBaseMethod();
        }

        public IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(params object[] values)
        {
            return this.wrappedBuilder.AssignsOutAndRefParameters(values);
        }

        public void MustHaveHappened(Repeated repeatConstraint)
        {
            Guard.AgainstNull(repeatConstraint, "repeatConstraint");

            this.rule.RepeatConstraint = repeatConstraint;
            this.rule.IsAssertion = true;
        }

        public IRecordingConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.AgainstNull(argumentsPredicate, "argumentsPredicate");

            this.rule.UsePredicateToValidateArguments(argumentsPredicate);

            return this;
        }
    }
}