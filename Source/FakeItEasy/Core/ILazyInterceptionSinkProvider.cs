namespace FakeItEasy.Core
{
    /// <summary>
    /// The responsibility of this interface is to instantiate a new <see cref="IInterceptionSink"/> and to fully initialize it (e.g. 
    /// applying the initial configuration). This should happen one time on the first call of <see cref="Fetch"/> or
    /// <see cref="EnsureInitialized"/>.
    /// </summary>
    /// <remarks>
    /// Note that the implementation of this interface must be thread-safe and guarantee the one-time-initialization property when accessing 
    /// the interface methods concurrently (could happen when using this provider in spawned threads in a proxy base constructor).
    /// </remarks>
    public interface ILazyInterceptionSinkProvider
    {
        /// <summary>
        /// Create and initialize a new <see cref="IInterceptionSink"/> for <paramref name="proxy"/>.
        /// </summary>
        /// <param name="proxy">The corresponding proxy object of the new <see cref="IInterceptionSink"/>.</param>
        /// <returns>The created <see cref="IInterceptionSink"/>.</returns>
        IInterceptionSink Fetch(object proxy);

        /// <summary>
        /// Ensures that the <see cref="IInterceptionSink"/> is initialized (and can be used via the proxy's tag later on).
        /// </summary>
        /// <param name="proxy">The corresponding proxy object of the new <see cref="IInterceptionSink"/>.</param>
        void EnsureInitialized(object proxy);
    }
}