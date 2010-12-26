namespace FakeItEasy.IoC
{
    /// <summary>
    /// Manages registration of a set of components in a DictionaryContainer.
    /// </summary>
    internal abstract class Module
    {
        /// <summary>
        /// Registers the components of this module.
        /// </summary>
        /// <param name="container">The container to register components in.</param>
        public abstract void RegisterDependencies(DictionaryContainer container);
    }
}