namespace FakeItEasy.VisualBasic
{
    using System;
    using FakeItEasy.Api;
    using FakeItEasy.Configuration;

    internal class VisualBasicRuleBuilder
        : IVisualBasicConfigurationWithArgumentValidation
    {
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
            throw new NotImplementedException();
        }

        public IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            throw new NotImplementedException();
        }

        public IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(params object[] values)
        {
            throw new NotImplementedException();
        }

        public void MustHaveHappened(Repeated repeatConstraint)
        {
            Guard.IsNotNull(repeatConstraint, "repeatConstraint");

            this.rule.RepeatConstraint = repeatConstraint;
            rule.IsAssertion = true;
        }

        public IVisualBasicConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.IsNotNull(argumentsPredicate, "argumentsPredicate");

            this.rule.UsePredicateToValidateArguments(argumentsPredicate);

            return this;
        }
    }
}
