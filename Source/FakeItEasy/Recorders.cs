namespace FakeItEasy
{
    using FakeItEasy.SelfInitializedFakes;

    /// <summary>
    /// Provides methods for creating recorders for self initializing fakes.
    /// </summary>
    public static class Recorders
    {
        /// <summary>
        /// Gets a recorder that records to and loads calls from the specified file.
        /// </summary>
        /// <param name="fileName">The file to use for recording.</param>
        /// <returns>A recorder instance.</returns>
        public static ISelfInitializingFakeRecorder FileRecorder(string fileName)
        {
            var fileStorageFactory = ServiceLocator.Current.Resolve<FileStorage.Factory>();
            var storage = fileStorageFactory.Invoke(fileName);

            var managerFactory = ServiceLocator.Current.Resolve<RecordingManager.Factory>();
            return managerFactory.Invoke(storage);
        }
    }
}