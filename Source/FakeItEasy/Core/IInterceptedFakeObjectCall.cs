namespace FakeItEasy.Core
{
    /// <summary>
    /// Represents a call to a fake object at interception time.
    /// </summary>
    public interface IInterceptedFakeObjectCall : IWritableFakeObjectCall
    {
        /// <summary>
        /// Sets that the call should not be recorded by the fake manager.
        /// </summary>
        void DoNotRecordCall();
    }
}