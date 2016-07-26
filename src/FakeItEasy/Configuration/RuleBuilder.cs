namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;

    internal class RuleBuilder
        : IVoidArgumentValidationConfiguration,
          IAfterCallSpecifiedWithOutAndRefParametersConfiguration<IVoidConfiguration>,
          IThenConfiguration<IVoidConfiguration>
    {
        private readonly FakeAsserter.Factory asserterFactory;
        private readonly FakeManager manager;
        private bool wasRuleAdded;

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
        /// <param name="ruleBeingBuilt">The rule that's being built.</param>
        /// <param name="fakeObject">The fake object the rule is for.</param>
        /// <returns>A configuration object.</returns>
        internal delegate RuleBuilder Factory(BuildableCallRule ruleBeingBuilt, FakeManager fakeObject);

        public BuildableCallRule RuleBeingBuilt { get; }

        IVoidConfiguration IThenConfiguration<IVoidConfiguration>.Then => this.Then;

        public IEnumerable<ICompletedFakeObjectCall> Calls => this.manager.GetRecordedCalls();

        public ICallMatcher Matcher => new RuleMatcher(this);

        private RuleBuilder Then
        {
            get
            {
                var newRule = this.RuleBeingBuilt.CloneCallSpecification();
                return new RuleBuilder(newRule, this.manager, this.asserterFactory) { PreviousRule = this.RuleBeingBuilt };
            }
        }

        private BuildableCallRule PreviousRule { get; set; }

        public IThenConfiguration<IVoidConfiguration> NumberOfTimes(int numberOfTimesToRepeat)
        {
            if (numberOfTimesToRepeat <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfTimesToRepeat),
                    numberOfTimesToRepeat,
                    "The number of times to repeat is not greater than zero.");
            }

            this.RuleBeingBuilt.NumberOfTimesToCall = numberOfTimesToRepeat;
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration<IVoidConfiguration> Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
        {
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.UseApplicator(x => { throw exceptionFactory(x); });
            return this;
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> Throws<T1>(Func<T1, Exception> exceptionFactory)
        {
            return this.Throws<IVoidConfiguration, T1>(exceptionFactory);
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> Throws<T1, T2>(Func<T1, T2, Exception> exceptionFactory)
        {
            return this.Throws<IVoidConfiguration, T1, T2>(exceptionFactory);
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> Throws<T1, T2, T3>(Func<T1, T2, T3, Exception> exceptionFactory)
        {
            return this.Throws<IVoidConfiguration, T1, T2, T3>(exceptionFactory);
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> Throws<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Exception> exceptionFactory)
        {
            return this.Throws<IVoidConfiguration, T1, T2, T3, T4>(exceptionFactory);
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> Throws<T>() where T : Exception, new()
        {
            return this.Throws<IVoidConfiguration, T>();
        }

        public IVoidConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.AgainstNull(argumentsPredicate, nameof(argumentsPredicate));

            this.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration<IVoidConfiguration> DoesNothing()
        {
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.UseDefaultApplicator();
            return this;
        }

        public virtual IVoidConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            Guard.AgainstNull(action, nameof(action));
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.Actions.Add(action);
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration<IVoidConfiguration> CallsBaseMethod()
        {
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.UseApplicator(x => { });
            this.RuleBeingBuilt.CallBaseMethod = true;
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer)
        {
            Guard.AgainstNull(valueProducer, nameof(valueProducer));

            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.OutAndRefParametersValueProducer = valueProducer;

            return this;
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1>(Func<T1, object[]> valueProducer)
        {
            return this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1>(valueProducer);
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1, T2>(Func<T1, T2, object[]> valueProducer)
        {
            return this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1, T2>(valueProducer);
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1, T2, T3>(Func<T1, T2, T3, object[]> valueProducer)
        {
            return this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1, T2, T3>(valueProducer);
        }

        public IAfterCallSpecifiedConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object[]> valueProducer)
        {
            return this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1, T2, T3, T4>(valueProducer);
        }

        public IVoidConfiguration Then
        {
            get
            {
                var newRule = this.RuleBeingBuilt.CloneCallSpecification();
                this.manager.AddRuleAfter(this.RuleBeingBuilt, newRule);
                return new RuleBuilder(newRule, this.manager, this.asserterFactory);
            }
        }

        public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
        {
            Guard.AgainstNull(repeatConstraint, nameof(repeatConstraint));

            var asserter = this.asserterFactory.Invoke(this.Calls);

            var description = new StringBuilderOutputWriter();
            this.RuleBeingBuilt.WriteDescriptionOfValidCall(description);

            asserter.AssertWasCalled(this.Matcher.Matches, description.Builder.ToString(), repeatConstraint.Matches, repeatConstraint.ToString());

            return new UnorderedCallAssertion(this.manager, this.Matcher, description.Builder.ToString(), repeatConstraint);
        }

        private void AddRuleIfNeeded()
        {
            if (!this.wasRuleAdded)
            {
                if (this.PreviousRule != null)
                {
                    this.manager.AddRuleAfter(this.PreviousRule, this.RuleBeingBuilt);
                }
                else
                {
                    this.manager.AddRuleFirst(this.RuleBeingBuilt);
                }

                this.wasRuleAdded = true;
            }
        }

        public class ReturnValueConfiguration<TMember>
            : IAnyCallConfigurationWithReturnTypeSpecified<TMember>,
              IAfterCallSpecifiedWithOutAndRefParametersConfiguration<IReturnValueConfiguration<TMember>>,
              IThenConfiguration<IReturnValueConfiguration<TMember>>
        {
            public ReturnValueConfiguration(RuleBuilder parentConfiguration)
            {
                this.ParentConfiguration = parentConfiguration;
            }

            public RuleBuilder ParentConfiguration { get; }

            public ICallMatcher Matcher => this.ParentConfiguration.Matcher;

            public IReturnValueConfiguration<TMember> Then
            {
                get
                {
                    var newConfiguration = this.ParentConfiguration.Then;
                    return new ReturnValueConfiguration<TMember>(newConfiguration);
                }
            }

            public IEnumerable<ICompletedFakeObjectCall> Calls => this.ParentConfiguration.Calls;

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
            {
                this.ParentConfiguration.Throws(exceptionFactory);
                return this;
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> Throws<T1>(Func<T1, Exception> exceptionFactory)
            {
                return this.Throws<IReturnValueConfiguration<TMember>, T1>(exceptionFactory);
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> Throws<T1, T2>(Func<T1, T2, Exception> exceptionFactory)
            {
                return this.Throws<IReturnValueConfiguration<TMember>, T1, T2>(exceptionFactory);
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> Throws<T1, T2, T3>(Func<T1, T2, T3, Exception> exceptionFactory)
            {
                return this.Throws<IReturnValueConfiguration<TMember>, T1, T2, T3>(exceptionFactory);
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> Throws<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Exception> exceptionFactory)
            {
                return this.Throws<IReturnValueConfiguration<TMember>, T1, T2, T3, T4>(exceptionFactory);
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> Throws<T>() where T : Exception, new()
            {
                return this.Throws<IReturnValueConfiguration<TMember>, T>();
            }

            public IAfterCallSpecifiedWithOutAndRefParametersConfiguration<IReturnValueConfiguration<TMember>> ReturnsLazily(Func<IFakeObjectCall, TMember> valueProducer)
            {
                Guard.AgainstNull(valueProducer, nameof(valueProducer));
                this.ParentConfiguration.AddRuleIfNeeded();
                this.ParentConfiguration.RuleBeingBuilt.UseApplicator(x => x.SetReturnValue(valueProducer(x)));
                return this;
            }

            public IReturnValueConfiguration<TMember> Invokes(Action<IFakeObjectCall> action)
            {
                this.ParentConfiguration.Invokes(action);
                return this;
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> CallsBaseMethod()
            {
                this.ParentConfiguration.CallsBaseMethod();
                return this;
            }

            public IReturnValueConfiguration<TMember> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                this.ParentConfiguration.WhenArgumentsMatch(argumentsPredicate);
                return this;
            }

            public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
            {
                return this.ParentConfiguration.MustHaveHappened(repeatConstraint);
            }

            public IAnyCallConfigurationWithReturnTypeSpecified<TMember> Where(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                this.ParentConfiguration.RuleBeingBuilt.ApplyWherePredicate(predicate, descriptionWriter);
                return this;
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer)
            {
                this.ParentConfiguration.AssignsOutAndRefParametersLazily(valueProducer);
                return this;
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> AssignsOutAndRefParametersLazily<T1>(Func<T1, object[]> valueProducer)
            {
                return this.AssignsOutAndRefParametersLazily<IReturnValueConfiguration<TMember>, T1>(valueProducer);
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> AssignsOutAndRefParametersLazily<T1, T2>(Func<T1, T2, object[]> valueProducer)
            {
                return this.AssignsOutAndRefParametersLazily<IReturnValueConfiguration<TMember>, T1, T2>(valueProducer);
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> AssignsOutAndRefParametersLazily<T1, T2, T3>(Func<T1, T2, T3, object[]> valueProducer)
            {
                return this.AssignsOutAndRefParametersLazily<IReturnValueConfiguration<TMember>, T1, T2, T3>(valueProducer);
            }

            public IAfterCallSpecifiedConfiguration<IReturnValueConfiguration<TMember>> AssignsOutAndRefParametersLazily<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object[]> valueProducer)
            {
                return this.AssignsOutAndRefParametersLazily<IReturnValueConfiguration<TMember>, T1, T2, T3, T4>(valueProducer);
            }

            public IThenConfiguration<IReturnValueConfiguration<TMember>> NumberOfTimes(int numberOfTimesToRepeat)
            {
                this.ParentConfiguration.NumberOfTimes(numberOfTimesToRepeat);
                return this;
            }

            public IReturnValueConfiguration<TMember> Then
            {
                get
                {
                    var newConfiguration = (RuleBuilder)this.ParentConfiguration.Then;
                    return new ReturnValueConfiguration<TMember>(newConfiguration);
                }
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
                Guard.AgainstNull(call, nameof(call));

                return this.builder.RuleBeingBuilt.IsApplicableTo(call) &&
                       ReferenceEquals(this.builder.manager.Object, call.FakedObject);
            }

            public override string ToString()
            {
                return this.builder.RuleBeingBuilt.ToString();
            }
        }
    }
}
