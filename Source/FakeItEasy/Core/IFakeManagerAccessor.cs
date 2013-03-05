namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Creation;

    /// <summary>
    /// Attaches a fake manager to the proxy so that intercepted
    /// calls can be configured.
    /// </summary>
    internal interface IFakeManagerAccessor
    {
        /// <summary>
        /// Attaches a <see cref="FakeManager"/> to the specified proxy, listening to
        /// the event raiser.
        /// </summary>
        /// <param name="typeOfFake">The type of the fake object proxy.</param>
        /// <param name="proxy">The proxy to attach to.</param>
        /// <param name="eventRaiser">The event raiser to listen to.</param>
        void AttachFakeManagerToProxy(Type typeOfFake, object proxy, ICallInterceptedEventRaiser eventRaiser);

        /// <summary>
        /// Gets the fake manager associated with the proxy.
        /// </summary>
        /// <param name="proxy">The proxy to get the manager from.</param>
        /// <returns>A fake manager.</returns>
        FakeManager GetFakeManager(object proxy);
    }
}