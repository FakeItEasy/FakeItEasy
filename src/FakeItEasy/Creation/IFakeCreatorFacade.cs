namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A facade used by the public API for testability.
    /// </summary>
    internal interface IFakeCreatorFacade
    {
        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fake to create.</typeparam>
        /// <param name="optionsBuilder">Action that builds options for the created fake object.</param>
        /// <returns>The created fake object.</returns>
        /// <exception cref="FakeItEasy.Core.FakeCreationException">Was unable to generate the fake in the current configuration.</exception>
        T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder);

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fake to create.</param>
        /// <param name="optionsBuilder">Action that builds options for the created fake object.</param>
        /// <returns>The created fake object.</returns>
        /// <exception cref="FakeItEasy.Core.FakeCreationException">Was unable to generate the fake in the current configuration.</exception>
        object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder);

        /// <summary>
        /// Creates a dummy object, this can be a fake object or an object resolved
        /// from a dummy factory.
        /// </summary>
        /// <typeparam name="T">The type of dummy to create.</typeparam>
        /// <returns>The created dummy.</returns>
        /// <exception cref="FakeItEasy.Core.FakeCreationException">Was unable to generate the fake in the current configuration and
        /// no dummy was registered in the container for the specified type.</exception>
        T CreateDummy<T>();

        /// <summary>
        /// Creates a dummy object, this can be a fake object or an object resolved
        /// from a dummy factory.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy to create.</param>
        /// <returns>The created dummy.</returns>
        /// <exception cref="FakeItEasy.Core.FakeCreationException">Was unable to generate the fake in the current configuration and
        /// no dummy was registered in the container for the specified type.</exception>
        object CreateDummy(Type typeOfDummy);

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        IList<T> CollectionOfFake<T>(int numberOfFakes);

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <param name="optionsBuilder">Action that builds options for the created fake object.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        IList<T> CollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder);

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fakes to create.</param>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        IList<object> CollectionOfFake(Type typeOfFake, int numberOfFakes);

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fakes to create.</param>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <param name="optionsBuilder">Action that builds options for the created fake object.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        IList<object> CollectionOfFake(Type typeOfFake, int numberOfFakes, Action<IFakeOptions> optionsBuilder);

        /// <summary>
        /// Creates a collection of dummies of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of dummies to create.</typeparam>
        /// <param name="numberOfDummies">The number of dummies in the collection.</param>
        /// <returns>A collection of dummy objects of the specified type.</returns>
        IList<T> CollectionOfDummy<T>(int numberOfDummies);

        /// <summary>
        /// Creates a collection of dummies of the specified type.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummies to create.</param>
        /// <param name="numberOfDummies">The number of dummies in the collection.</param>
        /// <returns>A collection of dummy objects of the specified type.</returns>
        IList<object> CollectionOfDummy(Type typeOfDummy, int numberOfDummies);
    }
}
