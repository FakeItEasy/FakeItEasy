namespace FakeItEasy.Core
{
    using System;

    /// <summary>
    /// Represents an ojbect that can build a fake object given a number
    /// of options.
    /// </summary>
    internal interface IFakeObjectBuilder
    {
        /// <summary>
        /// Generates a fake object according to the options.
        /// </summary>
        /// <typeparam name="T">The type of fake to generate.</typeparam>
        /// <param name="options">Options for the generation of the fake object.</param>
        /// <returns>A fake object.</returns>
        T GenerateFake<T>(Action<IFakeBuilderOptionsBuilder<T>> options);
    }
}
