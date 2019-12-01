namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class FakeAndDummyManager
    {
        private static readonly ConcurrentDictionary<Type, Func<ProxyOptions, IFakeOptions>> FakeOptionsFactoryCache = new ConcurrentDictionary<Type, Func<ProxyOptions, IFakeOptions>>();

        private static readonly Action<IFakeOptions> DefaultOptionsBuilder = options => { };

        private readonly IFakeObjectCreator fakeCreator;
        private readonly ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue;
        private readonly IDummyValueResolver dummyValueResolver;

        public FakeAndDummyManager(IDummyValueResolver dummyValueResolver, IFakeObjectCreator fakeCreator, ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue)
        {
            this.dummyValueResolver = dummyValueResolver;
            this.fakeCreator = fakeCreator;
            this.implicitOptionsBuilderCatalogue = implicitOptionsBuilderCatalogue;
        }

        public object? CreateDummy(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext)
        {
            return this.dummyValueResolver.TryResolveDummyValue(typeOfDummy, resolutionContext).Result;
        }

        public object CreateFake(
            Type typeOfFake,
            LoopDetectingResolutionContext resolutionContext)
        {
            var proxyOptions = this.BuildProxyOptions(typeOfFake, DefaultOptionsBuilder);

            return this.fakeCreator.CreateFake(typeOfFake, proxyOptions, this.dummyValueResolver, resolutionContext).Result !;
        }

        public object CreateFake(
            Type typeOfFake,
            Action<IFakeOptions> optionsBuilder,
            LoopDetectingResolutionContext resolutionContext)
        {
            var proxyOptions = this.BuildProxyOptions(typeOfFake, optionsBuilder);

            return this.fakeCreator.CreateFake(typeOfFake, proxyOptions, this.dummyValueResolver, resolutionContext).Result !;
        }

        public bool TryCreateDummy(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext, out object? result)
        {
            var creationResult = this.dummyValueResolver.TryResolveDummyValue(typeOfDummy, resolutionContext);
            if (creationResult.WasSuccessful)
            {
                result = creationResult.Result;
                return true;
            }

            result = default;
            return false;
        }

        private static IFakeOptions CreateFakeOptions<T>(ProxyOptions proxyOptions) where T : class => new FakeOptions<T>(proxyOptions);

        private static Func<ProxyOptions, IFakeOptions> GetFakeOptionsFactory(Type typeOfFake)
        {
            var method = typeof(FakeAndDummyManager).GetMethod(nameof(CreateFakeOptions), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeOfFake);

            return (Func<ProxyOptions, IFakeOptions>)method.CreateDelegate(typeof(Func<ProxyOptions, IFakeOptions>));
        }

        private IProxyOptions BuildProxyOptions(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            var implicitOptionsBuilder = this.implicitOptionsBuilderCatalogue.GetImplicitOptionsBuilder(typeOfFake);

            if (implicitOptionsBuilder is null && optionsBuilder == DefaultOptionsBuilder)
            {
                return ProxyOptions.Default;
            }

            var proxyOptions = new ProxyOptions();
            var fakeOptions = FakeOptionsFactoryCache.GetOrAdd(typeOfFake, GetFakeOptionsFactory).Invoke(proxyOptions);

            if (implicitOptionsBuilder is object)
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
