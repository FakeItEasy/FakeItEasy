namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>
    /// Provides configurations for fake objects of a specific type.
    /// </summary>
    public interface IFakeConfigurer
    {
        /// <summary>
        /// The type the instance provides configuration for.
        /// </summary>
        Type ForType { get; }

        /// <summary>
        /// Applies the configuration for the specified fake object.
        /// </summary>
        /// <param name="fakeObject">The fake object to configure.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Fake object is a common term in FakeItEasy.")]
        void ConfigureFake(object fakeObject);
    }
}
