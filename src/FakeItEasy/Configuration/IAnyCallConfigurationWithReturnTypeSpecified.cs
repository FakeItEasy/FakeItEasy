namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Aggregate of IReturnValueArgumentValidationConfiguration&lt;T&gt; and IWhereConfiguration&lt;IAnyCallConfigurationWithReturnTypeSpecified&lt;T&gt;&gt;.
    /// </summary>
    /// <typeparam name="T">The type of fake object that is configured.</typeparam>
    public interface IAnyCallConfigurationWithReturnTypeSpecified<T>
        : IReturnValueArgumentValidationConfiguration<T>, IWhereConfiguration<IAnyCallConfigurationWithReturnTypeSpecified<T>>
    {
    }
}
