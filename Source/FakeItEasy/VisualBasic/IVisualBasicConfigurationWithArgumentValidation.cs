namespace FakeItEasy.VisualBasic
{
    using FakeItEasy.Configuration;

    public interface IVisualBasicConfigurationWithArgumentValidation
        : IVisualBasicConfiguration, IArgumentValidationConfiguration<IVisualBasicConfiguration>
    {

    }
}
