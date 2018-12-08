namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using FakeItEasy.Core;

    /// <summary>
    /// The default implementation of the IFakeAndDummyManager interface.
    /// </summary>
    internal class DefaultFakeAndDummyManager
        : IFakeAndDummyManager
    {
        private static readonly ConcurrentDictionary<Type, Func<ProxyOptions, IFakeOptions>> FakeOptionsFactoryCache = new ConcurrentDictionary<Type, Func<ProxyOptions, IFakeOptions>>();

        private readonly IFakeObjectCreator fakeCreator;
        private readonly ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue;
        private readonly IDummyValueResolver dummyValueResolver;

        public DefaultFakeAndDummyManager(IDummyValueResolver dummyValueResolver, IFakeObjectCreator fakeCreator, ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue)
        {
            this.dummyValueResolver = dummyValueResolver;
            this.fakeCreator = fakeCreator;
            this.implicitOptionsBuilderCatalogue = implicitOptionsBuilderCatalogue;
        }

        public object CreateDummy(Type typeOfDummy)
        {
            return this.dummyValueResolver.TryResolveDummyValue(typeOfDummy).Result;
        }

        public object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            var proxyOptions = this.BuildProxyOptions(typeOfFake, optionsBuilder);

            return this.fakeCreator.CreateFake(typeOfFake, proxyOptions, this.dummyValueResolver).Result;
        }

        public bool TryCreateDummy(Type typeOfDummy, out object result)
        {
            var creationResult = this.dummyValueResolver.TryResolveDummyValue(typeOfDummy);
            if (creationResult.WasSuccessful)
            {
                result = creationResult.Result;
                return true;
            }

            result = default;
            return false;
        }

        private static IFakeOptions CreateFakeOptions<T>(ProxyOptions proxyOptions) => new FakeOptions<T>(proxyOptions);

        private static Func<ProxyOptions, IFakeOptions> GetFakeOptionsFactory(Type typeOfFake)
        {
            var method = typeof(DefaultFakeAndDummyManager).GetMethod(nameof(CreateFakeOptions), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeOfFake);

            return (Func<ProxyOptions, IFakeOptions>)method.CreateDelegate(typeof(Func<ProxyOptions, IFakeOptions>));
        }

        private IProxyOptions BuildProxyOptions(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            var implicitOptionsBuilder = this.implicitOptionsBuilderCatalogue.GetImplicitOptionsBuilder(typeOfFake);

            if (implicitOptionsBuilder == null && optionsBuilder == null)
            {
                return ProxyOptions.Default;
            }

            var proxyOptions = new ProxyOptions();
            var fakeOptions = FakeOptionsFactoryCache.GetOrAdd(typeOfFake, GetFakeOptionsFactory).Invoke(proxyOptions);

            if (implicitOptionsBuilder != null)
            {
                try
                {
                    implicitOptionsBuilder.BuildOptions(typeOfFake, fakeOptions);
                }
                catch (Exception ex)
                {
                    throw new UserCallbackException(
                        ExceptionMessages.UserCallbackThrewAnException($"Fake options builder '{implicitOptionsBuilder.GetType()}'"),
                        ex);
                }
            }

            optionsBuilder?.Invoke(fakeOptions);

            return proxyOptions;
        }
    }
}
