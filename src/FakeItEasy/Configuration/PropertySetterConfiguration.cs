namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;
    using FakeItEasy.Expressions;

    internal class PropertySetterConfiguration<TValue>
        : IPropertySetterAnyValueConfiguration<TValue>
    {
        private readonly ParsedCallExpression parsedSetterExpression;

        private readonly Func<ParsedCallExpression, IVoidArgumentValidationConfiguration> voidArgumentValidationConfigurationFactory;

        public PropertySetterConfiguration(
            ParsedCallExpression parsedCallExpression,
            Func<ParsedCallExpression, IVoidArgumentValidationConfiguration> voidArgumentValidationConfigurationFactory)
        {
            this.parsedSetterExpression = parsedCallExpression;
            this.voidArgumentValidationConfigurationFactory = voidArgumentValidationConfigurationFactory;
        }

        public IPropertySetterConfiguration To(TValue value)
        {
            return this.To(() => value);
        }

        public IPropertySetterConfiguration To(Expression<Func<TValue>> valueConstraint)
        {
            var newSetterExpression = this.CreateSetterExpressionWithNewValue(valueConstraint);
            var voidArgumentValidationConfiguration = this.CreateArgumentValidationConfiguration(newSetterExpression);
            return AsPropertySetterConfiguration(voidArgumentValidationConfiguration);
        }

        public IAfterCallSpecifiedConfiguration<IPropertySetterConfiguration> Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
        {
            return AsPropertySetterConfiguration(this.CreateArgumentValidationConfiguration(this.parsedSetterExpression))
                .Throws(exceptionFactory);
        }

        public IPropertySetterConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            var voidConfiguration = this.CreateArgumentValidationConfiguration(this.parsedSetterExpression)
                .Invokes(action);

            return AsPropertySetterConfiguration(voidConfiguration);
        }

        public IAfterCallSpecifiedConfiguration<IPropertySetterConfiguration> CallsBaseMethod()
        {
            return
                AsPropertySetterConfiguration(this.CreateArgumentValidationConfiguration(this.parsedSetterExpression))
                    .CallsBaseMethod();
        }

        public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
        {
            return this.CreateArgumentValidationConfiguration(this.parsedSetterExpression)
                .MustHaveHappened(repeatConstraint);
        }

        public IAfterCallSpecifiedConfiguration<IPropertySetterConfiguration> DoesNothing()
        {
            return
                AsPropertySetterConfiguration(this.CreateArgumentValidationConfiguration(this.parsedSetterExpression))
                    .DoesNothing();
        }

        public IPropertySetterConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            var voidConfiguration = this.CreateArgumentValidationConfiguration(this.parsedSetterExpression)
                .WhenArgumentsMatch(argumentsPredicate);

            return AsPropertySetterConfiguration(voidConfiguration);
        }

        private static IPropertySetterConfiguration AsPropertySetterConfiguration(
            IVoidConfiguration voidArgumentValidationConfiguration)
        {
            return new ProperySetterAdapter(voidArgumentValidationConfiguration);
        }

        private ParsedCallExpression CreateSetterExpressionWithNewValue(Expression<Func<TValue>> valueExpression)
        {
            var originalParameterInfos = this.parsedSetterExpression.CalledMethod.GetParameters();
            var parsedValueExpression = new ParsedArgumentExpression(
                valueExpression.Body,
                originalParameterInfos.Last());

            var arguments = this.parsedSetterExpression.ArgumentsExpressions
                .Take(originalParameterInfos.Length - 1)
                .Concat(new[] { parsedValueExpression });

            return new ParsedCallExpression(
                this.parsedSetterExpression.CalledMethod,
                this.parsedSetterExpression.CallTarget,
                arguments);
        }

        private IVoidArgumentValidationConfiguration CreateArgumentValidationConfiguration(
            ParsedCallExpression parsedSetter)
        {
            return this.voidArgumentValidationConfigurationFactory(parsedSetter);
        }

        private class ProperySetterAdapter : IPropertySetterConfiguration
        {
            private IVoidConfiguration voidConfiguration;

            public ProperySetterAdapter(IVoidConfiguration voidArgumentValidationConfiguration)
            {
                this.voidConfiguration = voidArgumentValidationConfiguration;
            }

            public IAfterCallSpecifiedConfiguration<IPropertySetterConfiguration> Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
            {
                return AsPropertySetterConfiguration(this.voidConfiguration).Throws(exceptionFactory);
            }

            public IAfterCallSpecifiedConfiguration<IPropertySetterConfiguration> CallsBaseMethod()
            {
                return AsPropertySetterConfiguration(this.voidConfiguration).CallsBaseMethod();
            }

            public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
            {
                return this.voidConfiguration.MustHaveHappened(repeatConstraint);
            }

            public IAfterCallSpecifiedConfiguration<IPropertySetterConfiguration> DoesNothing()
            {
                return AsPropertySetterConfiguration(this.voidConfiguration).DoesNothing();
            }

            public IPropertySetterConfiguration Invokes(Action<IFakeObjectCall> action)
            {
                this.voidConfiguration = this.voidConfiguration.Invokes(action);
                return this;
            }
        }
    }
}
