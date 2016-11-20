namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;

    internal class AnyCallConfiguration
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
            this.configuredRule.ApplicableToMembersWithReturnType = typeof(TMember);
            return this.configurationFactory.CreateConfiguration<TMember>(this.manager, this.configuredRule);
        }

        public IAnyCallConfigurationWithReturnTypeSpecified<object> WithNonVoidReturnType()
        {
            this.configuredRule.ApplicableToAllNonVoidReturnTypes = true;
            return this.configurationFactory.CreateConfiguration<object>(this.manager, this.configuredRule);
        }

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> DoesNothing() => this.VoidConfiguration.DoesNothing();

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws(Func<IFakeObjectCall, Exception> exceptionFactory) =>
            this.VoidConfiguration.Throws(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws<T1>(Func<T1, Exception> exceptionFactory) =>
            this.Throws<IVoidConfiguration, T1>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws<T1, T2>(Func<T1, T2, Exception> exceptionFactory) =>
            this.Throws<IVoidConfiguration, T1, T2>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws<T1, T2, T3>(Func<T1, T2, T3, Exception> exceptionFactory) =>
            this.Throws<IVoidConfiguration, T1, T2, T3>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Exception> exceptionFactory) =>
            this.Throws<IVoidConfiguration, T1, T2, T3, T4>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> Throws<T>() where T : Exception, new() =>
            this.Throws<IVoidConfiguration, T>();

        public IVoidConfiguration Invokes(Action<IFakeObjectCall> action) => this.VoidConfiguration.Invokes(action);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> CallsBaseMethod() =>
            this.VoidConfiguration.CallsBaseMethod();

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer) =>
            this.VoidConfiguration.AssignsOutAndRefParametersLazily(valueProducer);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1>(Func<T1, object[]> valueProducer) =>
            this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1>(valueProducer);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1, T2>(Func<T1, T2, object[]> valueProducer) =>
            this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1, T2>(valueProducer);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1, T2, T3>(Func<T1, T2, T3, object[]> valueProducer) =>
            this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1, T2, T3>(valueProducer);

        public IAfterCallConfiguredConfiguration<IVoidConfiguration> AssignsOutAndRefParametersLazily<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object[]> valueProducer) =>
            this.AssignsOutAndRefParametersLazily<IVoidConfiguration, T1, T2, T3, T4>(valueProducer);

        public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint) =>
            this.VoidConfiguration.MustHaveHappened(repeatConstraint);

        public IAnyCallConfigurationWithNoReturnTypeSpecified Where(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
        {
            this.configuredRule.ApplyWherePredicate(predicate, descriptionWriter);
            return this;
        }

        public IVoidConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.configuredRule.UsePredicateToValidateArguments(argumentsPredicate);
            return this;
        }
    }
}
