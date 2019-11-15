namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    internal partial class AnyCallConfiguration
        : IAnyCallConfigurationWithNoReturnTypeSpecified
    {
        private readonly IConfigurationFactory configurationFactory;
        private readonly AnyCallCallRule configuredRule;
        private readonly FakeManager manager;

        public AnyCallConfiguration(FakeManager manager, AnyCallCallRule configuredRule, IConfigurationFactory configurationFactory)
        {
            this.manager = manager;
            this.configuredRule = configuredRule;
            this.configurationFactory = configurationFactory;
        }

        private IVoidArgumentValidationConfiguration VoidConfiguration =>
            this.configurationFactory.CreateConfiguration(this.manager, this.configuredRule);

        public IAnyCallConfigurationWithReturnTypeSpecified<TMember> WithReturnType<TMember>()
        {
            this.configuredRule.MakeApplicableToMembersWithReturnType(typeof(TMember));
            return this.configurationFactory.CreateConfiguration<TMember>(this.manager, this.configuredRule);
        }

        public IAnyCallConfigurationWithReturnTypeSpecified<object> WithNonVoidReturnType()
        {
            this.configuredRule.MakeApplicableToAllNonVoidReturnTypes();
            return this.configurationFactory.CreateConfiguration<object>(this.manager, this.configuredRule);
        }

        public IAnyCallConfigurationWithVoidReturnType WithVoidReturnType()
        {
            this.configuredRule.MakeApplicableToMembersWithReturnType(typeof(void));
            return this.configurationFactory.CreateConfiguration(this.manager, this.configuredRule);
        }

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> DoesNothing() => this.VoidConfiguration.DoesNothing();

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws(Func<IFakeObjectCall, Exception> exceptionFactory) =>
            this.VoidConfiguration.Throws(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws<T>() where T : Exception, new() =>
            this.Throws<IVoidConfiguration, T>();

        public IVoidConfiguration Invokes(Action<IFakeObjectCall> action) => this.VoidConfiguration.Invokes(action);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> CallsBaseMethod() =>
            this.VoidConfiguration.CallsBaseMethod();

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object?>> valueProducer) =>
            this.VoidConfiguration.AssignsOutAndRefParametersLazily(valueProducer);

        public UnorderedCallAssertion MustHaveHappened(int numberOfTimes, Times timesOption)
        {
            Guard.AgainstNull(timesOption, nameof(timesOption));

            return this.VoidConfiguration.MustHaveHappened(numberOfTimes, timesOption);
        }

        public UnorderedCallAssertion MustHaveHappenedANumberOfTimesMatching(Expression<Func<int, bool>> predicate)
        {
            Guard.AgainstNull(predicate, nameof(predicate));

            return this.VoidConfiguration.MustHaveHappenedANumberOfTimesMatching(predicate);
        }

        public IAnyCallConfigurationWithNoReturnTypeSpecified Where(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            Guard.AgainstNull(predicate, nameof(predicate));

            this.configuredRule.ApplyWherePredicate(predicate, descriptionWriter);
            return this;
        }

        public IVoidConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.AgainstNull(argumentsPredicate, nameof(argumentsPredicate));

            this.configuredRule.UsePredicateToValidateArguments(argumentsPredicate);
            return this;
        }
    }
}
