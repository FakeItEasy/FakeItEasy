namespace FakeItEasy.Core.Generation
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
        /// <exception cref="In"
        object CreateDummy(Type typeOfDummy);
        object CreateFake(Type typeOfDummy, FakeOptions options);
        bool TryCreateDummy(Type typeOfFake);
        bool TryCreateFake(Type typeOfFake, FakeOptions options, out object result);
    }
}