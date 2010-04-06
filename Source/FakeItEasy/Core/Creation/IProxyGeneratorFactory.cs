namespace FakeItEasy.Core.Creation
{
    /// <summary>
    /// Handles the creation of a IProxyGenerator instance.
    /// </summary>
    public interface IProxyGeneratorFactory
    {
        /// <summary>
        /// When implemented creates a new IProxyGenerator-instance.
        /// </summary>
        /// <param name="container">The IFakeObjectContainer provided by the FakeItEasy framework.</param>
        /// <returns>A new IProxyGenerator.</returns>
        IProxyGenerator CreateProxyGenerator(IFakeObjectContainer container);
    }
}
