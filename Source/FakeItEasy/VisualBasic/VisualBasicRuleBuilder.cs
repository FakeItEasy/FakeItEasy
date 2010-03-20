namespace FakeItEasy.VisualBasic
{
    using System;
    using FakeItEasy.Api;
    using FakeItEasy.Configuration;

    public class VisualBasicRuleBuilder
        : IVisualBasicConfigurationWithArgumentValidation
    {
        public void AssertWasCalled(Func<int, bool> repeatPredicate)
        {
            //Guard.IsNotNull(repeatPredicate, "repeatPredicate");

            //var recordedRule = this.RuleBeingBuilt as RecordedCallRule;

            //if (recordedRule == null)
            //{
            //    throw new InvalidOperationException("Only RecordedCallRules can be used for assertions.");
            //}

            //recordedRule.IsAssertion = true;
            //recordedRule.Applicator = x => { };
            //recordedRule.RepeatPredicate = repeatPredicate;
#if DEBUG
            throw new NotImplementedException();
#else
#error "Must be implemented"
#endif
        }

        public IAfterCallSpecifiedConfiguration DoesNothing()
        {
            throw new NotImplementedException();
        }

        public IAfterCallSpecifiedConfiguration Throws(Exception exception)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IVisualBasicConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            //Guard.IsNotNull(argumentsPredicate, "argumentsPredicate");

            //this.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);

            //return this;
#if DEBUG
            throw new NotImplementedException();
#else
#error "Must be implemented"
#endif
        }
    }
}
