namespace FakeItEasy.Tests
{
    using FakeItEasy.SelfInitializedFakes;
    using FluentAssertions;
    using Xunit;

    public class RecordersTests
        : ConfigurableServiceLocatorTestBase
    {
        [Fact]
        public void FileRecorder_should_return_recording_manager_with_file_storage()
        {
            // Arrange
            var storage = new FileStorage(string.Empty, A.Fake<IFileSystem>());
            this.StubResolve<FileStorage.Factory>(x => x == "c:\\file.dat" ? storage : null);

            var recordingManager = A.Fake<RecordingManager>();
            this.StubResolve<RecordingManager.Factory>(x => x.Equals(storage) ? recordingManager : null);

            // Act
            var recorder = Recorders.FileRecorder("c:\\file.dat");

            // Assert
            recorder.Should().BeSameAs(recordingManager);
        }
    }
}
