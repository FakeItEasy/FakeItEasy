namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Creation;

    /// <summary>
    /// Implementation of <see cref="IFakeCallProcessorProvider"/>, which returns a <see cref="FakeManager"/> as "call processor" lazily (on
    /// the first call of <see cref="Fetch"/> or <see cref="EnsureInitialized"/>).
    /// </summary>
    /// <remarks>
    /// Note that we just need to serialize the <see cref="FakeManager"/> + the lock (an "empty", *new* object will be deserialized)
    /// because <see cref="IFakeCallProcessorProvider"/> doesn't require serializability before the first call of <see cref="Fetch"/> or
    /// <see cref="EnsureInitialized "/> (see remarks section of <see cref="IFakeCallProcessorProvider"/>).
    /// </remarks>
    [Serializable]
    internal class FakeManagerProvider : IFakeCallProcessorProvider
    {
        [NonSerialized]
        private readonly FakeManager.Factory fakeManagerFactory;

        [NonSerialized]
        private readonly IFakeManagerAccessor fakeManagerAccessor;

        [NonSerialized]
        private readonly IFakeObjectConfigurator fakeObjectConfigurator;

        [NonSerialized]
        private readonly Type typeOfFake;

        [NonSerialized]
        private readonly ProxyOptions proxyOptions;

        // We want to lock accesses to initializedFakeManager to guarantee thread-safety (see IFakeCallProcessorProvider documentation):
        private readonly object initializedFakeManagerLock = new object();

        private FakeManager initializedFakeManager;

        public FakeManagerProvider(
                FakeManager.Factory fakeManagerFactory,
                IFakeManagerAccessor fakeManagerAccessor,
                IFakeObjectConfigurator fakeObjectConfigurator, 
                Type typeOfFake,
                ProxyOptions proxyOptions)
        {
            Guard.AgainstNull(fakeManagerFactory, "fakeManagerFactory");
            Guard.AgainstNull(fakeManagerAccessor, "fakeManagerAccessor");
            Guard.AgainstNull(fakeObjectConfigurator, "fakeObjectConfigurator");
            Guard.AgainstNull(typeOfFake, "typeOfFake");
            Guard.AgainstNull(proxyOptions, "proxyOptions");

            this.fakeManagerFactory = fakeManagerFactory;
            this.fakeManagerAccessor = fakeManagerAccessor;
            this.fakeObjectConfigurator = fakeObjectConfigurator;
            this.typeOfFake = typeOfFake;
            this.proxyOptions = proxyOptions;
        }

        public IFakeCallProcessor Fetch(object proxy)
        {
            Guard.AgainstNull(proxy, "proxy");

            lock (this.initializedFakeManagerLock)
            {
                this.EnsureInitialized(proxy);

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

                    this.ApplyInitialConfiguration(proxy);
                }
            }
        }

        private void ApplyInitialConfiguration(object proxy)
        {
            this.fakeObjectConfigurator.ConfigureFake(this.typeOfFake, proxy);

            foreach (var proxyConfigurationAction in this.proxyOptions.ProxyConfigurationActions)
            {
                proxyConfigurationAction(proxy);
            }
        }
    }
}