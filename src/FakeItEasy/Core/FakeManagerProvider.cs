namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Creation;

    /// <summary>
    /// Implementation of <see cref="IFakeCallProcessorProvider"/>, which returns a <see cref="FakeManager"/> as "call processor" lazily (on
    /// the first call of <see cref="Fetch"/> or <see cref="EnsureInitialized(object, object?)"/>).
    /// </summary>
    internal class FakeManagerProvider : IFakeCallProcessorProvider
    {
        private readonly FakeManager.Factory fakeManagerFactory;

        private readonly IFakeManagerAccessor fakeManagerAccessor;

        private readonly Type typeOfFake;

        private readonly IProxyOptions proxyOptions;

        // We want to lock accesses to initializedFakeManager to guarantee thread-safety (see IFakeCallProcessorProvider documentation):
        private readonly object initializedFakeManagerLock = new object();

        private FakeManager? initializedFakeManager;

        public FakeManagerProvider(
                FakeManager.Factory fakeManagerFactory,
                IFakeManagerAccessor fakeManagerAccessor,
                Type typeOfFake,
                IProxyOptions proxyOptions)
        {
            Guard.AgainstNull(fakeManagerFactory);
            Guard.AgainstNull(fakeManagerAccessor);
            Guard.AgainstNull(typeOfFake);
            Guard.AgainstNull(proxyOptions);

            this.fakeManagerFactory = fakeManagerFactory;
            this.fakeManagerAccessor = fakeManagerAccessor;
            this.typeOfFake = typeOfFake;
            this.proxyOptions = proxyOptions;
        }

        public IFakeCallProcessor Fetch(object proxy)
        {
            Guard.AgainstNull(proxy);

            lock (this.initializedFakeManagerLock)
            {
                this.EnsureInitialized(proxy);

                return this.initializedFakeManager!;
            }
        }

        public void EnsureInitialized(object proxy) => this.EnsureInitialized(proxy, null);

        public void EnsureInitialized(object proxy, object? alias)
        {
            Guard.AgainstNull(proxy);

            lock (this.initializedFakeManagerLock)
            {
                if (this.initializedFakeManager is null)
                {
                    this.initializedFakeManager = this.fakeManagerFactory(this.typeOfFake, proxy, this.proxyOptions.Name);

                    this.fakeManagerAccessor.SetFakeManager(proxy, this.initializedFakeManager);
                    if (alias is not null)
                    {
                        this.fakeManagerAccessor.SetFakeManager(alias, this.initializedFakeManager);
                    }

                    // If it exists, the alias will have a more appropriate type than the proxy.
                    this.ApplyInitialConfiguration(alias ?? proxy);

                    this.initializedFakeManager.CaptureInitialState();
                }
            }
        }

        private void ApplyInitialConfiguration(object proxy)
        {
            foreach (var proxyConfigurationAction in this.proxyOptions.ProxyConfigurationActions)
            {
                proxyConfigurationAction(proxy);
            }
        }
    }
}
