namespace FakeItEasy.Core
{
    /// <summary>
    /// Interface implemented by generated faked objects in order
    /// to access the fake object behind it.
    /// </summary>
    public interface IFakedProxy
    {
        /// <summary>
        /// Gets the fake manager behind a faked object.
        /// </summary>
        /// <returns>A fake manager.</returns>
        FakeManager FakeManager { get; }
    }
}
