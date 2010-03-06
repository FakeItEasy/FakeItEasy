namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Lets the developer configure output values of out and ref parameters.
    /// </summary>
    public interface IOutAndRefParametersConfiguration
    {
        /// <summary>
        /// Specifies output values for out and ref parameters. Specify the values in the order
        /// the ref and out parameters has in the configured call, any non out and ref parameters are ignored.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration AssignsOutAndRefParameters(params object[] values);
    }
}
