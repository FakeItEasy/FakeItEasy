namespace FakeItEasy.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A combination of the IAfterCallSpecifiedConfiguration and IOutAndRefParametersConfiguration
    /// interfaces.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WithOut", Justification = "Refers to the two words 'with out' not the word 'without'.")]
    public interface IAfterCallSpecifiedWithOutAndRefParametersConfiguration
        : IAfterCallSpecifiedConfiguration, IOutAndRefParametersConfiguration
    {
    }
}