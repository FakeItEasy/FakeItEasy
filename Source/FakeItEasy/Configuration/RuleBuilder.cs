namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;

    internal class RuleBuilder
        : IVoidArgumentValidationConfiguration, 
          IRepeatConfiguration, 
          IAfterCallSpecifiedConfiguration, 
          IAfterCallSpecifiedWithOutAndRefParametersConfiguration, 
          ICallCollectionAndCallMatcherAccessor
    {
        private readonly FakeAsserter.Factory asserterFactory;
        private readonly FakeManager manager;

        internal RuleBuilder(BuildableCallRule ruleBeingBuilt, FakeManager manager, FakeAsserter.Factory asserterFactory)
        {
            this.RuleBeingBuilt = ruleBeingBuilt;
            this.manager = manager;
            this.asserterFactory = asserterFactory;
        }

        /// <summary>
        /// Represents a delegate that creates a configuration object from
        /// a fake object and the rule to build.
        /// </summary>
        /// <param name="fakeObject">The fake object the rule is for.</param>
        /// <param name="ruleBeingBuilt">The rule that's being built.</param>
        /// <returns>A configuration object.</returns>
        internal delegate RuleBuilder Factory(BuildableCallRule ruleBeingBuilt, FakeManager fakeObject);

        public BuildableCallRule RuleBeingBuilt { get; private set; }

        public IEnumerable<ICompletedFakeObjectCall> Calls
        {
            get { return this.manager.RecordedCallsInScope; }
        }

        public ICallMatcher Matcher
        {
            get { return new RuleMatcher(this); }
        }

        public void NumberOfTimes(int numberOfTimesToRepeat)
        {
            Guard.IsInRange(numberOfTimesToRepeat, 1, int.MaxValue, "numberOfTimesToRepeat");

            this.RuleBeingBuilt.NumberOfTimesToCall = numberOfTimesToRepeat;
        }

        public virtual IAfterCallSpecifiedConfiguration Throws(Exception exception)
        {
            this.RuleBeingBuilt.Applicator = x => { throw exception; };
            return this;
        }

        public IVoidConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.AgainstNull(argumentsPredicate, "argumentsPredicate");

            this.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration DoesNothing()
        {
            this.RuleBeingBuilt.Applicator = x => { };
            return this;
        }

        public virtual IVoidConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            Guard.AgainstNull(action, "action");

            this.RuleBeingBuilt.Actions.Add(action);
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            this.RuleBeingBuilt.Applicator = x => { };
            this.RuleBeingBuilt.CallBaseMethod = true;
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(params object[] values)
        {
            Guard.AgainstNull(values, "values");

            this.RuleBeingBuilt.OutAndRefParametersValues = values;

            return this;
        }

        public void MustHaveHappened(Repeated repeatConstraint)
        {
            this.manager.RemoveRule(this.RuleBeingBuilt);
            var asserter = this.asserterFactory.Invoke(this.Calls.Cast<IFakeObjectCall>());

            var description = new StringBuilderOutputWriter();
            this.RuleBeingBuilt.WriteDescriptionOfValidCall(description);

            asserter.AssertWasCalled(this.Matcher.Matches, description.Builder.ToString(), repeatConstraint.Matches, repeatConstraint.ToString());
        }

        public class ReturnValueConfiguration<TMember>
            : IReturnValueArgumentValidationConfiguration<TMember>, ICallCollectionAndCallMatcherAccessor
        {
            public RuleBuilder ParentConfiguration { get; set; }

            public ICallMatcher Matcher
            {
                get { return this.ParentConfiguration.Matcher; }
            }

            public IEnumerable<ICompletedFakeObjectCall> Calls
            {
                get { return this.ParentConfiguration.Calls; }
            }

            public IAfterCallSpecifiedConfiguration Throws(Exception exception)
            {
                return this.ParentConfiguration.Throws(exception);
            }

            public IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily(Func<IFakeObjectCall, TMember> valueProducer)
            {
                Guard.AgainstNull(valueProducer, "valueProducer");

                this.ParentConfiguration.RuleBeingBuilt.Applicator = x => x.SetReturnValue(valueProducer(x));
                return this.ParentConfiguration;
            }

            public IReturnValueConfiguration<TMember> Invokes(Action<IFakeObjectCall> action)
            {
                Guard.AgainstNull(action, "action");

                this.ParentConfiguration.RuleBeingBuilt.Actions.Add(action);
                return this;
            }

            public IAfterCallSpecifiedConfiguration CallsBaseMethod()
            {
                return this.ParentConfiguration.CallsBaseMethod();
            }

            public IReturnValueConfiguration<TMember> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                Guard.AgainstNull(argumentsPredicate, "argumentsPredicate");

                this.ParentConfiguration.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);
                return this;
            }

            public void MustHaveHappened(Repeated repeatConstraint)
            {
                this.ParentConfiguration.MustHaveHappened(repeatConstraint);
            }
        }

        private class RuleMatcher
            : ICallMatcher
        {
            private readonly RuleBuilder builder;

            public RuleMatcher(RuleBuilder builder)
            {
                this.builder = builder;
            }

            public bool Matches(IFakeObjectCall call)
            {
                return this.builder.RuleBeingBuilt.IsApplicableTo(call);
            }

            public override string ToString()
            {
                return this.builder.RuleBeingBuilt.ToString();
            }
        }
    }
}