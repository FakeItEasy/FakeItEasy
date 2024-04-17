namespace FakeItEasy.Core
{
    /// <summary>
    /// The responsibility of this interface is to instantiate a new <see cref="IFakeCallProcessor"/> and to fully initialize it (e.g.
    /// applying the initial configuration). This should happen one time on the first call of <see cref="Fetch"/> or
    /// <see cref="EnsureInitialized(object, object?)"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that the implementation of this interface must be thread-safe and guarantee the one-time-initialization property when accessing
    /// the interface methods concurrently (could happen when using this provider in spawned threads in a proxy base constructor).
    /// </para>
    /// </remarks>
    internal interface IFakeCallProcessorProvider
    {
        /// <summary>
        /// Create and initialize a new <see cref="IFakeCallProcessor"/> for <paramref name="proxy"/>.
        /// </summary>
        /// <param name="proxy">The corresponding proxy object of the new <see cref="IFakeCallProcessor"/>.</param>
        /// <returns>The created <see cref="IFakeCallProcessor"/>.</returns>
        IFakeCallProcessor Fetch(object proxy);

        /// <summary>
        /// Ensures that the <see cref="IFakeCallProcessor"/> is initialized (and can be used to retrieve the proxy's fake call processor later on).
        /// </summary>
        /// <param name="proxy">The corresponding proxy object of the new <see cref="IFakeCallProcessor"/>.</param>
        void EnsureInitialized(object proxy);

        /// <summary>
        /// Ensures that the <see cref="IFakeCallProcessor"/> is initialized (and can be used to retrieve the proxy's fake call processor later on).
        /// </summary>
        /// <param name="proxy">The corresponding proxy object of the new <see cref="IFakeCallProcessor"/>.</param>
        /// <param name="alias">Another representation of the Fake. Popular when faking a delegate.</param>
        void EnsureInitialized(object proxy, object alias);
    }
}
