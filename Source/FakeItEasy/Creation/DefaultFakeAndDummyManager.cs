namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection.Emit;
    using FakeItEasy.Core;

    /// <summary>
    /// The default implementation of the IFakeAndDummyManager interface.
    /// </summary>
    internal class DefaultFakeAndDummyManager
        : IFakeAndDummyManager
    {
        private readonly FakeObjectCreator fakeCreator;
        private readonly IFakeObjectConfigurator fakeConfigurator;
        private readonly IDummyValueCreationSession session;

        public DefaultFakeAndDummyManager(IDummyValueCreationSession session, FakeObjectCreator fakeCreator, IFakeObjectConfigurator fakeConfigurator)
        {
            this.session = session;
            this.fakeCreator = fakeCreator;
            this.fakeConfigurator = fakeConfigurator;
        }

        public object CreateDummy(Type typeOfDummy)
        {
            object result;
            if (!this.session.TryResolveDummyValue(typeOfDummy, out result))
            {
                throw new FakeCreationException();
            }

            return result;
        }

        public object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            var proxyOptions = this.BuildProxyOptions(typeOfFake, optionsBuilder);

            return this.fakeCreator.CreateFake(typeOfFake, proxyOptions, this.session, throwOnFailure: true);
        }

        public bool TryCreateDummy(Type typeOfDummy, out object result)
        {
            return this.session.TryResolveDummyValue(typeOfDummy, out result);
        }

        public bool TryCreateFake(Type typeOfFake, IProxyOptions options, out object result)
        {
            result = this.fakeCreator.CreateFake(typeOfFake, options, this.session, throwOnFailure: false);
            return result != null;
        }

        private static IFakeOptions CreateFakeOptions(Type typeOfFake, ProxyOptions proxyOptions)
        {
            var optionsConstructor = typeof(FakeOptions<>)
                .MakeGenericType(typeOfFake)
                .GetConstructor(new[] { typeof(ProxyOptions) });

            return (IFakeOptions)optionsConstructor.Invoke(new object[] { proxyOptions });
        }

        private IProxyOptions BuildProxyOptions(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            var proxyOptions = new ProxyOptions();
            var options = CreateFakeOptions(typeOfFake, proxyOptions);

            this.fakeConfigurator.ConfigureFake(typeOfFake, options);
            optionsBuilder.Invoke(options);
            return proxyOptions;
        }

        private class FakeOptions<T>
            : IFakeOptions<T>, IFakeOptions
        {
            private readonly ProxyOptions proxyOptions;

            public FakeOptions(ProxyOptions proxyOptions)
            {
                this.proxyOptions = proxyOptions;
            }

            public IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
            {
                this.proxyOptions.ArgumentsForConstructor = argumentsForConstructor;
                return this;
            }

            public IFakeOptions<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall)
            {
                this.proxyOptions.ArgumentsForConstructor = GetConstructorArgumentsFromExpression(constructorCall);
                return this;
            }

            public IFakeOptions<T> WithAdditionalAttributes(
                IEnumerable<CustomAttributeBuilder> customAttributeBuilders)
            {
                Guard.AgainstNull(customAttributeBuilders, "customAttributeBuilders");

                foreach (var customAttributeBuilder in customAttributeBuilders)
                {
                    this.proxyOptions.AddAttribute(customAttributeBuilder);
                }

                return this;
            }

            public IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance)
            {
                var wrapper = new FakeWrapperConfigurator<T>(this, wrappedInstance);
                this.ConfigureFake(fake => wrapper.ConfigureFakeToWrap(fake));
                return wrapper;
            }

            public IFakeOptions<T> Implements(Type interfaceType)
            {
                this.proxyOptions.AddInterfaceToImplement(interfaceType);
                return this;
            }

            public IFakeOptions<T> Implements<TInterface>()
            {
                return this.Implements(typeof(TInterface));
            }

            public IFakeOptions<T> ConfigureFake(Action<T> action)
            {
                this.proxyOptions.AddProxyConfigurationAction(proxy => action((T)proxy));
                return this;
            }

            IFakeOptions IFakeOptions.ConfigureFake(Action<object> action)
            {
                return (IFakeOptions)this.ConfigureFake(fake => action(fake));
            }

            private static IEnumerable<object> GetConstructorArgumentsFromExpression(Expression<Func<T>> constructorCall)
            {
                AssertThatExpressionRepresentConstructorCall(constructorCall);
                return ((NewExpression)constructorCall.Body).Arguments.Select(argument => argument.Evaluate());
            }

            private static void AssertThatExpressionRepresentConstructorCall(Expression<Func<T>> constructorCall)
            {
                if (constructorCall.Body.NodeType != ExpressionType.New)
                {
                    throw new ArgumentException(ExceptionMessages.NonConstructorExpressionMessage);
                }
            }
        }
    }
}