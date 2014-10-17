namespace FakeItEasy.Core
{
    /// <summary>
    /// Attaches a fake manager to the proxy so that intercepted
    /// calls can be configured.
    /// </summary>
    internal interface IFakeManagerAccessor
    {
        /// <summary>
        /// Gets the fake manager associated with the proxy.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>A fake manager.</returns>
        FakeManager GetFakeManager(object proxy);

        /// <summary>
        /// Tags a proxy object, so that it can accessed later by <see cref="GetFakeManager"/>.
        /// </summary>
        /// <param name="proxy">The proxy to tag.</param>
        /// <param name="manager">The fake manager.</param>
        void TagProxy(object proxy, FakeManager manager);
    }
}