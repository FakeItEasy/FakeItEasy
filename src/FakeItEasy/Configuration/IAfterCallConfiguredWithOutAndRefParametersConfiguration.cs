namespace FakeItEasy.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A combination of the IAfterCallConfiguredConfiguration and IOutAndRefParametersConfiguration
    /// interfaces.
    /// </summary>
    /// <typeparam name="TInterface">The type of configuration interface to return.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "WithOut", Justification = "Refers to the two words 'with out' not the word 'without'.")]
    public interface IAfterCallConfiguredWithOutAndRefParametersConfiguration<out TInterface>
        : IAfterCallConfiguredConfiguration<TInterface>, IOutAndRefParametersConfiguration<TInterface>
    {
    }
}
