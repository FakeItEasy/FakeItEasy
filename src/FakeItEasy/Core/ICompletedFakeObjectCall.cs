namespace FakeItEasy.Core
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Represents a completed call to a fake object.
    /// </summary>
    public interface ICompletedFakeObjectCall : IFakeObjectCall
    {
        /// <summary>
        /// Gets the value that was returned from the call.
        /// </summary>
        object? ReturnValue { get; }

        /// <summary>
        /// Gets the arguments used in the call, after the call is made. Includes changes to the values of out and ref arguments.
        /// </summary>
        ArgumentCollection ArgumentsAfterCall { get; }
    }
}
