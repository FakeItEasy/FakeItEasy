namespace FakeItEasy.VisualBasic
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Configuration;

    internal class VisualBasicRuleBuilder
        : IVisualBasicConfigurationWithArgumentValidation
    {
        public delegate VisualBasicRuleBuilder Factory(RecordedCallRule rule, FakeManager fakeObject);

        private RecordedCallRule rule;
        private RuleBuilder wrappedBuilder;

        public VisualBasicRuleBuilder(RecordedCallRule rule, RuleBuilder wrappedBuilder)
        {
            this.rule = rule;
            this.wrappedBuilder = wrappedBuilder;

            rule.Applicator = x => { };
        }

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
            rule.IsAssertion = true;
        }

        public IVisualBasicConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.AgainstNull(argumentsPredicate, "argumentsPredicate");

            this.rule.UsePredicateToValidateArguments(argumentsPredicate);

            return this;
        }
    }
}
