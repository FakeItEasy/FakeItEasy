namespace FakeItEasy.Core
{
    /// <summary>
    /// Provides string formatting for fake object calls.
    /// </summary>
    internal interface IFakeObjectCallFormatter
    {
        /// <summary>
        /// Gets a human readable description of the specified
        /// fake object call.
        /// </summary>
        /// <param name="call">The call to get a description for.</param>
        /// <returns>A description of the call.</returns>
        string GetDescription(IFakeObjectCall call);
    }
}
