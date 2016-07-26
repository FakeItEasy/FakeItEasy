namespace FakeItEasy.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Configures the next behavior for this call.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    public interface IThenConfiguration<out TInterface> : IHideObjectMembers
    {
        /// <summary>
        /// Gets an object that lets you specify how the call will behave after the previous configuration has been consumed.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = nameof(Then), Justification = "Part of the fluent syntax.")]
        TInterface Then { get; }
    }
}
