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

        private IVoidArgumentValidationConfiguration VoidConfiguration
        {
            get { return this.configurationFactory.CreateConfiguration(this.manager, this.configuredRule); }
        }

        public IAnyCallConfigurationWithReturnTypeSpecified<TMember> WithReturnType<TMember>()
        {
            this.configuredRule.ApplicableToMembersWithReturnType = typeof(TMember);
            return this.configurationFactory.CreateConfiguration<TMember>(this.manager, this.configuredRule);
        }

        public IAfterCallSpecifiedConfiguration DoesNothing()
        {
            return this.VoidConfiguration.DoesNothing();
        }

        public IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
        {
            return this.VoidConfiguration.Throws(exceptionFactory);
        }

        public IVoidConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            return this.VoidConfiguration.Invokes(action);
        }

        public IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            return this.VoidConfiguration.CallsBaseMethod();
        }

        public IAfterCallSpecifiedConfiguration AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer)
        {
            return this.VoidConfiguration.AssignsOutAndRefParametersLazily(valueProducer);
        }

        public void MustHaveHappened(Repeated repeatConstraint)
        {
            this.VoidConfiguration.MustHaveHappened(repeatConstraint);
        }

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