namespace FakeItEasy.Creation
{
    using System;
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
        private readonly IDummyValueResolver dummyValueResolver;

        public DefaultFakeAndDummyManager(IDummyValueResolver dummyValueResolver, FakeObjectCreator fakeCreator, DynamicOptionsBuilder dynamicOptionsBuilder)
        {
            this.dummyValueResolver = dummyValueResolver;
            this.fakeCreator = fakeCreator;
            this.dynamicOptionsBuilder = dynamicOptionsBuilder;
        }

        public object CreateDummy(Type typeOfDummy)
        {
            return this.dummyValueResolver.TryResolveDummyValue(new DummyCreationSession(), typeOfDummy).GetResultAsDummy();
        }

        public object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            var proxyOptions = this.BuildProxyOptions(typeOfFake, optionsBuilder);

            return this.fakeCreator.CreateFake(typeOfFake, proxyOptions, new DummyCreationSession(), this.dummyValueResolver).GetResultAsFake();
        }

        public bool TryCreateDummy(Type typeOfDummy, out object result)
        {
            var creationResult = this.dummyValueResolver.TryResolveDummyValue(new DummyCreationSession(), typeOfDummy);
            if (creationResult.WasSuccessful)
            {
                result = creationResult.GetResultAsDummy();
                return true;
            }

            result = default;
            return false;
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
    }
}
