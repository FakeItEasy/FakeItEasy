namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    internal class AnyCallConfiguration
        : IAnyCallConfiguration
    {
        private FakeManager fakeObject;
        private AnyCallCallRule configuredRule;
        private IConfigurationFactory configurationFactory;

        public AnyCallConfiguration(FakeManager fakeObject, AnyCallCallRule configuredRule, IConfigurationFactory configurationFactory)
        {
            this.fakeObject = fakeObject;
            this.configuredRule = configuredRule;
            this.configurationFactory = configurationFactory;
        }

        private IVoidArgumentValidationConfiguration VoidConfiguration
        {
            get
            {
                return this.configurationFactory.CreateConfiguration(this.fakeObject, this.configuredRule);
            }
        }

        public IReturnValueArgumentValidationConfiguration<TMember> WithReturnType<TMember>()
        {
            this.configuredRule.ApplicableToMembersWithReturnType = typeof(TMember);
            return this.configurationFactory.CreateConfiguration<TMember>(this.fakeObject, this.configuredRule);
        }

        public IAfterCallSpecifiedConfiguration DoesNothing()
        {
            return this.VoidConfiguration.DoesNothing();
        }

        public IAfterCallSpecifiedConfiguration Throws(Exception exception)
        {
            return this.VoidConfiguration.Throws(exception);
        }

        public IVoidConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            return this.VoidConfiguration.Invokes(action);
        }

        public IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            return this.VoidConfiguration.CallsBaseMethod();
        }

        public IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(params object[] values)
        {
            return this.VoidConfiguration.AssignsOutAndRefParameters(values);
        }

        public void MustHaveHappened(Repeated repeatConstraint)
        {
            this.VoidConfiguration.MustHaveHappened(repeatConstraint);
        }
    }
}
