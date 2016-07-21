namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Configures the next behavior for this call.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IThenConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Returns an object that lets you specify how the call will behave after the previous configuration has been consumed.
        /// </summary>
        TInterface Then { get; }
    }
}
