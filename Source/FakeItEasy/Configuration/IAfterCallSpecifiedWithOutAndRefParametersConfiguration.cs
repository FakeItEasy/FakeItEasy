namespace FakeItEasy.Configuration
{
    /// <summary>
    /// A combination of the IAfterCallSpecifiedConfiguration and IOutAndRefParametersConfiguration
    /// interfaces.
    /// </summary>
    public interface IAfterCallSpecifiedWithOutAndRefParametersConfiguration
        : IAfterCallSpecifiedConfiguration, IOutAndRefParametersConfiguration
    { }
}
