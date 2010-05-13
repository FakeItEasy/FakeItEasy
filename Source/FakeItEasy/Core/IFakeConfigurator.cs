namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>
    /// Provides configurations for fake objects of a specific type.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurator", Justification = "I'm from Sweden.")]
    public interface IFakeConfigurator
    {
        /// <summary>
        /// The type the instance provides configuration for.
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Applies the configuration for the specified fake object.
        /// </summary>
        /// <param name="fakeObject">The fake object to configure.</param>
        void ConfigureFake(object fakeObject);
    }
}
