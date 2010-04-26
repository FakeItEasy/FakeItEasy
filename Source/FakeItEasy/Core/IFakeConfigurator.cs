namespace FakeItEasy.Core
{
    using System;
    
    /// <summary>
    /// Provides configurations for fake objects of a specific type.
    /// </summary>
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
