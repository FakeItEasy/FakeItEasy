namespace FakeItEasy.Configuration
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Core;

    internal class PropertySetterConfiguration<TValue>
        : IPropertySetterAnyValueConfiguration<TValue>
    {
        private readonly MethodCallExpression setterExpression;

        private readonly Func<LambdaExpression, IVoidArgumentValidationConfiguration> voidArgumentValidationConfigurationFactory;

        public PropertySetterConfiguration(
            MethodCallExpression setterExpression,
            Func<LambdaExpression, IVoidArgumentValidationConfiguration> voidArgumentValidationConfigurationFactory)
        {
            this.setterExpression = setterExpression;
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

        public IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
        {
            return this.CreateArgumentValidationConfiguration(this.setterExpression)
                .Throws(exceptionFactory);
        }

        public IPropertySetterConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            var voidConfiguration = this.CreateArgumentValidationConfiguration(this.setterExpression)
                .Invokes(action);
            return AsPropertySetterConfiguration(voidConfiguration);
        }

        public IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            return this.CreateArgumentValidationConfiguration(this.setterExpression)
                .CallsBaseMethod();
        }

        public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
        {
            return this.CreateArgumentValidationConfiguration(this.setterExpression)
                .MustHaveHappened(repeatConstraint);
        }

        public IAfterCallSpecifiedConfiguration DoesNothing()
        {
            return this.CreateArgumentValidationConfiguration(this.setterExpression)
                .DoesNothing();
        }

        public IPropertySetterConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            var voidConfiguration = this.CreateArgumentValidationConfiguration(this.setterExpression)
                .WhenArgumentsMatch(argumentsPredicate);
            return AsPropertySetterConfiguration(voidConfiguration);
        }

        private static IPropertySetterConfiguration AsPropertySetterConfiguration(IVoidConfiguration voidArgumentValidationConfiguration)
        {
            return new ProperySetterAdapter(voidArgumentValidationConfiguration);
        }

        private MethodCallExpression CreateSetterExpressionWithNewValue(Expression<Func<TValue>> valueExpression)
        {
            var arguments = this.setterExpression.Arguments
                .Take(this.setterExpression.Arguments.Count - 1)
                .Concat(new[] { valueExpression.Body });
            return Expression.Call(
                this.setterExpression.Object,
                this.setterExpression.Method,
                arguments);
        }

        private IVoidArgumentValidationConfiguration CreateArgumentValidationConfiguration(
            MethodCallExpression expression)
        {
            return this.voidArgumentValidationConfigurationFactory(Expression.Lambda(expression));
        }

        private class ProperySetterAdapter : IPropertySetterConfiguration
        {
            private IVoidConfiguration voidConfiguration;

            public ProperySetterAdapter(IVoidConfiguration voidArgumentValidationConfiguration)
            {
                this.voidConfiguration = voidArgumentValidationConfiguration;
            }

            public IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
            {
                return this.voidConfiguration.Throws(exceptionFactory);
            }

            public IAfterCallSpecifiedConfiguration CallsBaseMethod()
            {
                return this.voidConfiguration.CallsBaseMethod();
            }

            public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
            {
                return this.voidConfiguration.MustHaveHappened(repeatConstraint);
            }

            public IAfterCallSpecifiedConfiguration DoesNothing()
            {
                return this.voidConfiguration.DoesNothing();
            }

            public IPropertySetterConfiguration Invokes(Action<IFakeObjectCall> action)
            {
                this.voidConfiguration = this.voidConfiguration.Invokes(action);
                return this;
            }
        }
    }
}
