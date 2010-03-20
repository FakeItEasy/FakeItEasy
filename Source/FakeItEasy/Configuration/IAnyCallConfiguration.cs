namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Configuration for any call to a faked object.
    /// </summary>
    public interface IAnyCallConfiguration
        : IVoidConfiguration
    {
        /// <summary>
        /// Matches calls that has the return type specified in the generic type parameter.
        /// </summary>
        /// <typeparam name="TMember">The return type of the members to configure.</typeparam>
        /// <returns>A configuration object.</returns>
        IReturnValueArgumentValidationConfiguration<TMember> WithReturnType<TMember>();
    }
}
