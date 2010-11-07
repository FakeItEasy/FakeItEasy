namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Configurations for when a configured call is recorded.
    /// </summary>
    public interface IRecordingConfiguration
        : IVoidConfiguration, IAssertConfiguration
    {
    }
}