namespace FakeItEasy.SelfInitializedFakes
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents storage for recorded calls for self initializing
    /// fakes.
    /// </summary>
    public interface ICallStorage
    {
        /// <summary>
        /// Loads the recorded calls for the specified recording.
        /// </summary>
        /// <param name="recordingId">The id of the recording to load calls for.</param>
        /// <returns>The recorded calls for the recording with the specified id.</returns>
        IEnumerable<CallData> Load();

        /// <summary>
        /// Saves the specified calls as the recording with the specified id,
        /// overwriting any previous recording.
        /// </summary>
        /// <param name="recordingId">The id of the recording.</param>
        /// <param name="calls">The calls to save.</param>
        void Save(IEnumerable<CallData> calls);
    }
}
