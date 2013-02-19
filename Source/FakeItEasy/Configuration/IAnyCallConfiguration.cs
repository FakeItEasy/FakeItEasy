namespace FakeItEasy.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Configuration for any call to a faked object.
    /// </summary>
    public interface IAnyCallConfigurationWithNoReturnTypeSpecified
        : IWhereConfiguration<IAnyCallConfigurationWithNoReturnTypeSpecified>, IVoidArgumentValidationConfiguration
    {
        /// <summary>
        /// Matches calls that has the return type specified in the generic type parameter.
        /// </summary>
        /// <typeparam name="TMember">The return type of the members to configure.</typeparam>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to provide a strongly typed fluent API.")]
        IAnyCallConfigurationWithReturnTypeSpecified<TMember> WithReturnType<TMember>();
    }
}