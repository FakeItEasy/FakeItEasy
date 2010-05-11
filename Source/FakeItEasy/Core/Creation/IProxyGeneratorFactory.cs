namespace FakeItEasy.Core.Creation
{
    using FakeItEasy.DynamicProxy;

    /// <summary>
    /// Handles the creation of a IProxyGenerator instance.
    /// </summary>
    public interface IProxyGeneratorFactory
    {
        /// <summary>
        /// When implemented creates a new IProxyGenerator-instance.
        /// </summary>
        /// <param name="session">The session for the proxy generator to work in.</param>
        /// <returns>A new IProxyGenerator.</returns>
        IProxyGenerator CreateProxyGenerator(IDummyResolvingSession session);
    }
}
