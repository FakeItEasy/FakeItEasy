namespace FakeItEasy.Core
{
    /// <summary>
    /// Represents a completed call to a fake object.
    /// </summary>
    public interface ICompletedFakeObjectCall
        : IFakeObjectCall
    {
        /// <summary>
        /// The value set to be returned from the call.
        /// </summary>
        object ReturnValue { get; }
    }
}