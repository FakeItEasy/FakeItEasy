namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Handles global configuration of fake object.
    /// </summary>
    public interface IFakeObjectConfigurator
    {
        /// <summary>
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeObject">The fake object to configure.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Fake object is a common term in FakeItEasy.")]
        void ConfigureFake(Type typeOfFake, object fakeObject);
    }
}