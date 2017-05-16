namespace FakeItEasy.Core
{
    using System;

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
        /// <exception cref="ArgumentException">If <paramref name="proxy"/> is not actually a faked object.</exception>
        FakeManager GetFakeManager(object proxy);

        /// <summary>
        /// Gets the fake manager associated with the proxy, if any.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>The fake manager, or <c>null</c> if <paramref name="proxy"/> is not actually a faked object.</returns>
        FakeManager TryGetFakeManager(object proxy);

        /// <summary>
        /// Sets the fake manager for a proxy object, so that it can accessed later by <see cref="GetFakeManager"/>.
        /// </summary>
        /// <param name="proxy">The proxy to tag.</param>
        /// <param name="manager">The fake manager.</param>
        void SetFakeManager(object proxy, FakeManager manager);
    }
}
