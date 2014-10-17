namespace FakeItEasy.Core
{
    using System;

    /// <summary>
    /// Implementation of <see cref="ILazyInterceptionSinkProvider"/>, which returns a <see cref="FakeManager"/> as interception sink.
    /// </summary>
    internal class LazyFakeManagerProvider : ILazyInterceptionSinkProvider
    {
        private readonly FakeManager.Factory fakeManagerFactory;
        private readonly IFakeManagerAccessor fakeManagerAccessor;
        private readonly IFakeObjectConfigurator configurer;
        private readonly Type typeOfFake;

        // We want to lock accesses to initializedFakeManager because to guarantee thread-safety (see ILazyInterceptionSinkProvider documentation):
        private readonly object initializedFakeManagerLock = new object();

        private FakeManager initializedFakeManager;

        public LazyFakeManagerProvider(
                FakeManager.Factory fakeManagerFactory,
                IFakeManagerAccessor fakeManagerAccessor,
                IFakeObjectConfigurator configurer,
                Type typeOfFake)
        {
            Guard.AgainstNull(fakeManagerFactory, "fakeManagerFactory");
            Guard.AgainstNull(fakeManagerAccessor, "fakeManagerAccessor");
            Guard.AgainstNull(configurer, "configurer");
            Guard.AgainstNull(typeOfFake, "typeOfFake");

            this.fakeManagerFactory = fakeManagerFactory;
            this.fakeManagerAccessor = fakeManagerAccessor;
            this.configurer = configurer;
            this.typeOfFake = typeOfFake;
        }

        public IInterceptionSink Fetch(object proxy)
        {
            Guard.AgainstNull(proxy, "proxy");

            lock (this.initializedFakeManagerLock)
            {
                this.EnsureInitialized(proxy);

                if (!ReferenceEquals(this.initializedFakeManager.Object, proxy))
                {
                    throw new ArgumentException(ExceptionMessages.FakeManagerWasInitializedWithDifferentProxyMessage, "proxy");
                }

                return this.initializedFakeManager;
            }
        }

        public void EnsureInitialized(object proxy)
        {
            Guard.AgainstNull(proxy, "proxy");

            lock (this.initializedFakeManagerLock)
            {
                if (this.initializedFakeManager == null)
                {
                    this.initializedFakeManager = this.fakeManagerFactory(this.typeOfFake, proxy);

                    this.fakeManagerAccessor.TagProxy(proxy, this.initializedFakeManager);

                    this.configurer.ConfigureFake(this.typeOfFake, proxy);
                }
            }
        }
    }
}