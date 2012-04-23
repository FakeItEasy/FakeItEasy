namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configuration from VisualBasic.
    /// </summary>
    public interface IRecordingConfigurationWithArgumentValidation
        : IRecordingConfiguration, IArgumentValidationConfiguration<IRecordingConfiguration>
    {
    }
}