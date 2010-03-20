namespace FakeItEasy.VisualBasic
{
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides configuration from VisualBasic.
    /// </summary>
    public interface IVisualBasicConfigurationWithArgumentValidation
        : IVisualBasicConfiguration, IArgumentValidationConfiguration<IVisualBasicConfiguration>
    {

    }
}
