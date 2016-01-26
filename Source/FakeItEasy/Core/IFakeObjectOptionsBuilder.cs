namespace FakeItEasy.Core
{
    using System;
    using Creation;

    /// <summary>
    /// Handles global configuration of fake objects by building fake creation options.
    /// </summary>
    public interface IFakeObjectOptionsBuilder
    {
        /// <summary>
        /// Builds a fake's creation options.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeOptions">The options to build for the fake's creation.</param>
        void BuildOptions(Type typeOfFake, IFakeOptions fakeOptions);
    }
}