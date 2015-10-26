namespace FakeItEasy.Core
{
    using System;
    using Creation;

    /// <summary>
    /// Handles global configuration of fake object.
    /// </summary>
    public interface IFakeObjectConfigurator
    {
        /// <summary>
        /// Configures a fake's creation options.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeOptions">The options to build for the fake's creation.</param>
        void ConfigureFake(Type typeOfFake, IFakeOptions fakeOptions);
    }
}