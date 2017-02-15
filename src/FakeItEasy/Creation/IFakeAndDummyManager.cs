namespace FakeItEasy.Creation
{
    using System;

    /// <summary>
    /// Handles the creation of fake and dummy objects.
    /// </summary>
    internal interface IFakeAndDummyManager
    {
        /// <summary>
        /// Creates a dummy of the specified type.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy to create.</param>
        /// <returns>The created dummy.</returns>
        /// <exception cref="FakeItEasy.Core.FakeCreationException">The current IProxyGenerator is not able to generate a dummy of the specified type.</exception>
        object CreateDummy(Type typeOfDummy);

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to generate.</param>
        /// <param name="optionsBuilder">Builds options for the proxy that will act as the fake.</param>
        /// <returns>A fake object.</returns>
        /// <exception cref="FakeItEasy.Core.FakeCreationException">The current IProxyGenerator is not able to generate a fake of the specified type.</exception>
        object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder);

        /// <summary>
        /// Tries to create a dummy of the specified type.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy to create.</param>
        /// <param name="result">Outputs the result dummy when creation is successful.</param>
        /// <returns>A value indicating whether the creation was successful.</returns>
        bool TryCreateDummy(Type typeOfDummy, out object result);
    }
}
