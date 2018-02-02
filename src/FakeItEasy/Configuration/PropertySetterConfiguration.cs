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

        public IPropertySetterConfiguration To(TValue value) =>
            this.To(() => value);

        public IPropertySetterConfiguration To(Expression<Func<TValue>> valueConstraint)
        {
            var newSetterExpression = this.CreateSetterExpressionWithNewValue(valueConstraint);
            var voidArgumentValidationConfiguration = this.CreateArgumentValidationConfiguration(newSetterExpression);
            return AsPropertySetterConfiguration(voidArgumentValidationConfiguration);
        }

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws(Func<IFakeObjectCall, Exception> exceptionFactory) =>
            AsPropertySetterConfiguration(this.CreateArgumentValidationConfiguration(this.parsedSetterExpression))
                .Throws(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1>(Func<T1, Exception> exceptionFactory) =>
            this.Throws<IPropertySetterConfiguration, T1>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1, T2>(Func<T1, T2, Exception> exceptionFactory) =>
            this.Throws<IPropertySetterConfiguration, T1, T2>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1, T2, T3>(Func<T1, T2, T3, Exception> exceptionFactory) =>
            this.Throws<IPropertySetterConfiguration, T1, T2, T3>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Exception> exceptionFactory) =>
            this.Throws<IPropertySetterConfiguration, T1, T2, T3, T4>(exceptionFactory);

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T>() where T : Exception, new() =>
            this.Throws<IPropertySetterConfiguration, T>();

        public IPropertySetterConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            var voidConfiguration = this.CreateArgumentValidationConfiguration(this.parsedSetterExpression)
                .Invokes(action);

            return AsPropertySetterConfiguration(voidConfiguration);
        }

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> CallsBaseMethod() =>
            AsPropertySetterConfiguration(this.CreateArgumentValidationConfiguration(this.parsedSetterExpression))
                .CallsBaseMethod();

        public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint) =>
            this.CreateArgumentValidationConfiguration(this.parsedSetterExpression).MustHaveHappened(repeatConstraint);

        public UnorderedCallAssertion MustHaveHappened(int numberOfTimes, Times timesOption)
        {
            Guard.AgainstNull(timesOption, nameof(timesOption));

            return this.CreateArgumentValidationConfiguration(this.parsedSetterExpression).MustHaveHappened(numberOfTimes, timesOption);
        }

        public UnorderedCallAssertion MustHaveHappenedANumberOfTimesMatching(Expression<Func<int, bool>> predicate)
        {
            Guard.AgainstNull(predicate, nameof(predicate));

            return this.CreateArgumentValidationConfiguration(this.parsedSetterExpression).MustHaveHappenedANumberOfTimesMatching(predicate);
        }

        public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> DoesNothing() =>
            AsPropertySetterConfiguration(this.CreateArgumentValidationConfiguration(this.parsedSetterExpression))
                .DoesNothing();

        public IPropertySetterConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            var voidConfiguration = this.CreateArgumentValidationConfiguration(this.parsedSetterExpression)
                .WhenArgumentsMatch(argumentsPredicate);

            return AsPropertySetterConfiguration(voidConfiguration);
        }

        private static IPropertySetterConfiguration AsPropertySetterConfiguration(
                IVoidConfiguration voidArgumentValidationConfiguration) =>
            new PropertySetterAdapter(voidArgumentValidationConfiguration);

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
                ParsedCallExpression parsedSetter) =>
            this.voidArgumentValidationConfigurationFactory(parsedSetter);

        private class PropertySetterAdapter : IPropertySetterConfiguration
        {
            private IVoidConfiguration voidConfiguration;

            public PropertySetterAdapter(IVoidConfiguration voidArgumentValidationConfiguration)
            {
                this.voidConfiguration = voidArgumentValidationConfiguration;
            }

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws(Func<IFakeObjectCall, Exception> exceptionFactory) =>
                new PropertySetterAfterCallConfiguredAdapter(this.voidConfiguration.Throws(exceptionFactory));

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1>(Func<T1, Exception> exceptionFactory) =>
                this.Throws<IPropertySetterConfiguration, T1>(exceptionFactory);

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1, T2>(Func<T1, T2, Exception> exceptionFactory) =>
                this.Throws<IPropertySetterConfiguration, T1, T2>(exceptionFactory);

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1, T2, T3>(Func<T1, T2, T3, Exception> exceptionFactory) =>
                this.Throws<IPropertySetterConfiguration, T1, T2, T3>(exceptionFactory);

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Exception> exceptionFactory) =>
                this.Throws<IPropertySetterConfiguration, T1, T2, T3, T4>(exceptionFactory);

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> Throws<T>() where T : Exception, new() =>
                this.Throws<IPropertySetterConfiguration, T>();

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> CallsBaseMethod() =>
                new PropertySetterAfterCallConfiguredAdapter(this.voidConfiguration.CallsBaseMethod());

            public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint) =>
                this.voidConfiguration.MustHaveHappened(repeatConstraint);

            public UnorderedCallAssertion MustHaveHappened(int numberOfTimes, Times timesOption)
            {
                Guard.AgainstNull(timesOption, nameof(timesOption));

                return this.voidConfiguration.MustHaveHappened(numberOfTimes, timesOption);
            }

            public UnorderedCallAssertion MustHaveHappenedANumberOfTimesMatching(Expression<Func<int, bool>> predicate)
            {
                Guard.AgainstNull(predicate, nameof(predicate));

                return this.voidConfiguration.MustHaveHappenedANumberOfTimesMatching(predicate);
            }

            public IAfterCallConfiguredConfiguration<IPropertySetterConfiguration> DoesNothing() =>
                new PropertySetterAfterCallConfiguredAdapter(this.voidConfiguration.DoesNothing());

            public IPropertySetterConfiguration Invokes(Action<IFakeObjectCall> action)
            {
                this.voidConfiguration = this.voidConfiguration.Invokes(action);
                return this;
            }
        }

        private class PropertySetterAfterCallConfiguredAdapter : IAfterCallConfiguredConfiguration<IPropertySetterConfiguration>
        {
            private readonly IAfterCallConfiguredConfiguration<IVoidConfiguration> voidAfterCallConfiguration;

            public PropertySetterAfterCallConfiguredAdapter(IAfterCallConfiguredConfiguration<IVoidConfiguration> voidAfterCallConfiguration)
            {
                this.voidAfterCallConfiguration = voidAfterCallConfiguration;
            }

            public IThenConfiguration<IPropertySetterConfiguration> NumberOfTimes(int numberOfTimes) =>
                new PropertySetterThenAdapter(this.voidAfterCallConfiguration.NumberOfTimes(numberOfTimes));
        }

        private class PropertySetterThenAdapter : IThenConfiguration<IPropertySetterConfiguration>
        {
            private readonly IThenConfiguration<IVoidConfiguration> voidThenConfiguration;

            public PropertySetterThenAdapter(IThenConfiguration<IVoidConfiguration> voidThenConfiguration)
            {
                this.voidThenConfiguration = voidThenConfiguration;
            }

            public IPropertySetterConfiguration Then => AsPropertySetterConfiguration(this.voidThenConfiguration.Then);
        }
    }
}
