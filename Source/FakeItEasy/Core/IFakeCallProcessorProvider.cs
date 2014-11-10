namespace FakeItEasy.Core
{
    /// <summary>
    /// The responsibility of this interface is to instantiate a new <see cref="IFakeCallProcessor"/> and to fully initialize it (e.g. 
    /// applying the initial configuration). This should happen one time on the first call of <see cref="Fetch"/> or
    /// <see cref="EnsureInitialized"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Note that the implementation of this interface must be thread-safe and guarantee the one-time-initialization property when accessing 
    /// the interface methods concurrently (could happen when using this provider in spawned threads in a proxy base constructor).
    /// </para>
    /// <para>
    /// An implementation of this interface should also be serializable and the deserialized object should behave like the original one
    /// *after* it has been initialized (i.e. after the first call of <see cref="Fetch"/> or <see cref="EnsureInitialized "/>) because 
    /// we don't need to serialize a fake before it has been initialized (returned to the user).
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
        /// Ensures that the <see cref="IFakeCallProcessor"/> is initialized (and can be used via the proxy's tag later on).
        /// </summary>
        /// <param name="proxy">The corresponding proxy object of the new <see cref="IFakeCallProcessor"/>.</param>
        void EnsureInitialized(object proxy);
    }
}