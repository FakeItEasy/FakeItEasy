namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Aggregate of IVoidArgumentValidationConfiguration and IWhereConfiguration&lt;IAnyCallConfigurationWithVoidReturnType&gt;.
    /// </summary>
    public interface IAnyCallConfigurationWithVoidReturnType
        : IVoidArgumentValidationConfiguration,
          IWhereConfiguration<IAnyCallConfigurationWithVoidReturnType>
    {
    }
}
