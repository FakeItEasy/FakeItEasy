namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides a method to specify on which fake events should be managed.
    /// </summary>
    public interface IManageEventConfiguration
    {
        /// <summary>
        /// Specifies on which fake events should be managed.
        /// </summary>
        /// <param name="fake">The fake on which events should be managed.</param>
#pragma warning disable CA1716 // Identifiers should not match keywords
        void Of(object fake);
#pragma warning restore CA1716 // Identifiers should not match keywords
    }
}
