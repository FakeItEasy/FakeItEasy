namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using FakeItEasy.Core;

    /// <summary>
    /// The default implementation of the IFakeAndDummyManager interface.
    /// </summary>
    internal class DefaultFakeAndDummyManager
        : IFakeAndDummyManager
    {
        private readonly FakeObjectCreator fakeCreator;
        private readonly DynamicOptionsBuilder dynamicOptionsBuilder;
        private readonly IDummyValueCreationSession session;

        public DefaultFakeAndDummyManager(IDummyValueCreationSession session, FakeObjectCreator fakeCreator, DynamicOptionsBuilder dynamicOptionsBuilder)
        {
            this.session = session;
            this.fakeCreator = fakeCreator;
            this.dynamicOptionsBuilder = dynamicOptionsBuilder;
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

            this.dynamicOptionsBuilder.BuildOptions(typeOfFake, options);
            optionsBuilder.Invoke(options);
            return proxyOptions;
        }

        private class FakeOptions<T>
            : FakeOptionsBase<T>
        {
            private readonly ProxyOptions proxyOptions;

            public FakeOptions(ProxyOptions proxyOptions)
            {
                this.proxyOptions = proxyOptions;
            }

            public override IFakeOptions<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor)
            {
                this.proxyOptions.ArgumentsForConstructor = argumentsForConstructor;
                return this;
            }

            public override IFakeOptions<T> WithAttributes(
                params Expression<Func<Attribute>>[] attributes)
            {
                Guard.AgainstNull(attributes, nameof(attributes));

                foreach (var attribute in attributes)
                {
                    this.proxyOptions.AddAttribute(attribute);
                }

                return this;
            }

#if FEATURE_SELF_INITIALIZED_FAKES
            public override IFakeOptionsForWrappers<T> Wrapping(T wrappedInstance)
#else
            public override IFakeOptions<T> Wrapping(T wrappedInstance)
#endif
            {
                Guard.AgainstNull(wrappedInstance, nameof(wrappedInstance));

                var wrapper = new FakeWrapperConfigurator<T>(this, wrappedInstance);
                this.ConfigureFake(fake => wrapper.ConfigureFakeToWrap(fake));
                return wrapper;
            }

            public override IFakeOptions<T> Implements(Type interfaceType)
            {
                this.proxyOptions.AddInterfaceToImplement(interfaceType);
                return this;
            }

            public override IFakeOptions<T> ConfigureFake(Action<T> action)
            {
                this.proxyOptions.AddProxyConfigurationAction(proxy => action((T)proxy));
                return this;
            }
        }
    }
}
