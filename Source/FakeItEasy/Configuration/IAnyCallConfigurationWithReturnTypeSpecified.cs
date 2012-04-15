namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Aggregate of IReturnValueArgumentValidationConfiguration<T> and IWhereConfiguration<IAnyCallConfigurationWithReturnTypeSpecified<T>>
    /// </summary>
    /// <typeparam name="T">The type of fake object that is configured.</typeparam>
    public interface IAnyCallConfigurationWithReturnTypeSpecified<T>
        : IReturnValueArgumentValidationConfiguration<T>, IWhereConfiguration<IAnyCallConfigurationWithReturnTypeSpecified<T>>
    {
        
    }
}