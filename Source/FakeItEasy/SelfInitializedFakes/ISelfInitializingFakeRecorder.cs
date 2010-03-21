namespace FakeItEasy.SelfInitializedFakes
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// An interface for recorders that provides stored responses for self initializing fakes.
    /// </summary>
    public interface ISelfInitializingFakeRecorder
        : IDisposable 
    {
        /// <summary>
        /// Applies the call if the call has been recorded.
        /// </summary>
        /// <param name="call">The call to apply to from recording.</param>
        void ApplyNext(IWritableFakeObjectCall call);

        /// <summary>
        /// Gets a value indicating if the recorder is currently recording.
        /// </summary>
        bool IsRecording { get; }
        
        /// <summary>
        /// Records the specified call.
        /// </summary>
        /// <param name="call">The call to record.</param>
        void RecordCall(ICompletedFakeObjectCall call);
    }
}
