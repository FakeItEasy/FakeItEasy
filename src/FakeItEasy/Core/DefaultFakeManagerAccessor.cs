namespace FakeItEasy.Core
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Default implementation of <see cref="IFakeManagerAccessor"/>.
    /// </summary>
#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    internal class DefaultFakeManagerAccessor
        : IFakeManagerAccessor
    {
        private static readonly ConditionalWeakTable<object, FakeManager> FakeManagers = new ConditionalWeakTable<object, FakeManager>();

        /// <summary>
        /// Gets the fake manager associated with the proxy.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>A fake manager.</returns>
        /// <exception cref="ArgumentException">If <paramref name="proxy"/> is not actually a faked object.</exception>
        public FakeManager GetFakeManager(object proxy)
        {
            Guard.AgainstNull(proxy, nameof(proxy));

            FakeManager result = this.TryGetFakeManager(proxy);

            if (result == null)
            {
                throw new ArgumentException(ExceptionMessages.NotRecognizedAsAFake(proxy, proxy.GetType()));
            }

            return result;
        }

        /// <summary>
        /// Gets the fake manager associated with the proxy, if any.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>The fake manager, or <c>null</c> if <paramref name="proxy"/> is not actually a faked object.</returns>
        public FakeManager TryGetFakeManager(object proxy)
        {
            Guard.AgainstNull(proxy, nameof(proxy));

            FakeManager fakeManager;
            FakeManagers.TryGetValue(proxy, out fakeManager);
            return fakeManager;
        }

        public void SetFakeManager(object proxy, FakeManager manager)
        {
            Guard.AgainstNull(proxy, nameof(proxy));
            Guard.AgainstNull(manager, nameof(manager));

            FakeManagers.Add(proxy, manager);
        }
    }
}
