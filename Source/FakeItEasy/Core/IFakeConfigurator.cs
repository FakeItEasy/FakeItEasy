namespace FakeItEasy.Mef
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Provides configurations for fake objects of a specific type.
    /// </summary>
    [InheritedExport(typeof(IFakeConfigurator))]
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
