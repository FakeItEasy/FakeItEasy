namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Aggregate of IVoidArgumentValidationConfiguration&lt;T&gt; and IWhereConfiguration&lt;IAnyCallConfigurationWithVoidReturnType&gt;.
    /// </summary>
    public interface IAnyCallConfigurationWithVoidReturnType
        : IVoidArgumentValidationConfiguration,
          IWhereConfiguration<IAnyCallConfigurationWithVoidReturnType>
    {
    }
}
