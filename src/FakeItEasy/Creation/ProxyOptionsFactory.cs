namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class ProxyOptionsFactory : IProxyOptionsFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<ProxyOptions, IFakeOptions>> FakeOptionsFactoryCache = new();

        private readonly ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue;

        public ProxyOptionsFactory(ImplicitOptionsBuilderCatalogue implicitOptionsBuilderCatalogue)
        {
            this.implicitOptionsBuilderCatalogue = implicitOptionsBuilderCatalogue;
        }

        public IProxyOptions BuildProxyOptions(Type typeOfFake, Action<IFakeOptions>? optionsBuilder)
        {
            var implicitOptionsBuilder = this.implicitOptionsBuilderCatalogue.GetImplicitOptionsBuilder(typeOfFake);

            if (implicitOptionsBuilder is null && optionsBuilder is null)
            {
                return ProxyOptions.Default;
            }

            var proxyOptions = new ProxyOptions();
            var fakeOptions = FakeOptionsFactoryCache.GetOrAdd(typeOfFake, GetFakeOptionsFactory).Invoke(proxyOptions);

            if (implicitOptionsBuilder is not null)
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

        private static FakeOptions<T> CreateFakeOptions<T>(ProxyOptions proxyOptions) where T : class => new FakeOptions<T>(proxyOptions);

        private static Func<ProxyOptions, IFakeOptions> GetFakeOptionsFactory(Type typeOfFake)
        {
            var method = typeof(ProxyOptionsFactory).GetMethod(nameof(CreateFakeOptions), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(typeOfFake);
#if LACKS_GENERIC_METHODINFO_CREATEDELEGATE
            return (Func<ProxyOptions, IFakeOptions>)method.CreateDelegate(typeof(Func<ProxyOptions, IFakeOptions>));
#else
            return method.CreateDelegate<Func<ProxyOptions, IFakeOptions>>();
#endif
        }
    }
}
