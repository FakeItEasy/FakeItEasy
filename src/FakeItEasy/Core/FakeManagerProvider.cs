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
    /// <see cref="EnsureInitialized"/> (see remarks section of <see cref="IFakeCallProcessorProvider"/>).
    /// </remarks>
#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    internal class FakeManagerProvider : IFakeCallProcessorProvider
    {
#if FEATURE_BINARY_SERIALIZATION
        [NonSerialized]
#endif
        private readonly FakeManager.Factory fakeManagerFactory;

        private readonly IFakeManagerAccessor fakeManagerAccessor;

#if FEATURE_BINARY_SERIALIZATION
        [NonSerialized]
#endif
        private readonly Type typeOfFake;

#if FEATURE_BINARY_SERIALIZATION
        [NonSerialized]
#endif
        private readonly IProxyOptions proxyOptions;

        // We want to lock accesses to initializedFakeManager to guarantee thread-safety (see IFakeCallProcessorProvider documentation):
#pragma warning disable CA2235 // Mark all non-serializable fields
        private readonly object initializedFakeManagerLock = new object();
#pragma warning restore CA2235 // Mark all non-serializable fields

        private FakeManager initializedFakeManager;

        public FakeManagerProvider(
                FakeManager.Factory fakeManagerFactory,
                IFakeManagerAccessor fakeManagerAccessor,
                Type typeOfFake,
                IProxyOptions proxyOptions)
        {
            Guard.AgainstNull(fakeManagerFactory, nameof(fakeManagerFactory));
            Guard.AgainstNull(fakeManagerAccessor, nameof(fakeManagerAccessor));
            Guard.AgainstNull(typeOfFake, nameof(typeOfFake));
            Guard.AgainstNull(proxyOptions, nameof(proxyOptions));

            this.fakeManagerFactory = fakeManagerFactory;
            this.fakeManagerAccessor = fakeManagerAccessor;
            this.typeOfFake = typeOfFake;
            this.proxyOptions = proxyOptions;
        }

        public IFakeCallProcessor Fetch(object proxy)
        {
            Guard.AgainstNull(proxy, nameof(proxy));

            lock (this.initializedFakeManagerLock)
            {
                this.EnsureInitialized(proxy);

                return this.initializedFakeManager;
            }
        }

        public void EnsureInitialized(object proxy)
        {
            Guard.AgainstNull(proxy, nameof(proxy));

            lock (this.initializedFakeManagerLock)
            {
                if (this.initializedFakeManager == null)
                {
                    this.initializedFakeManager = this.fakeManagerFactory(this.typeOfFake, proxy);

                    this.fakeManagerAccessor.SetFakeManager(proxy, this.initializedFakeManager);

                    this.ApplyInitialConfiguration(proxy);
                }
            }
        }

        public void EnsureManagerIsRegistered()
        {
            this.fakeManagerAccessor.SetFakeManager(this.initializedFakeManager.Object, this.initializedFakeManager);
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
