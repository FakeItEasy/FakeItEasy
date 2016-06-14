namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;

    /// <summary>
    /// Default implementation of the IFakeCreator-interface.
    /// </summary>
    internal class DefaultFakeCreatorFacade
        : IFakeCreatorFacade
    {
        private readonly IFakeAndDummyManager fakeAndDummyManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFakeCreatorFacade"/> class.
        /// </summary>
        /// <param name="fakeAndDummyManager">The fake and dummy manager.</param>
        public DefaultFakeCreatorFacade(IFakeAndDummyManager fakeAndDummyManager)
        {
            this.fakeAndDummyManager = fakeAndDummyManager;
        }

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fake to create.</typeparam>
        /// <param name="optionsBuilder">Action that builds options for the created fake object.</param>
        /// <returns>The created fake object.</returns>
        /// <exception cref="FakeCreationException">Was unable to generate the fake in the current configuration.</exception>
        public T CreateFake<T>(Action<IFakeOptions<T>> optionsBuilder)
        {
            Guard.AgainstNull(optionsBuilder, "optionsBuilder");

            return (T)this.fakeAndDummyManager.CreateFake(typeof(T), options => optionsBuilder((IFakeOptions<T>)options));
        }

        public object CreateFake(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            Guard.AgainstNull(typeOfFake, nameof(typeOfFake));
            Guard.AgainstNull(optionsBuilder, nameof(optionsBuilder));

            return this.fakeAndDummyManager.CreateFake(typeOfFake, optionsBuilder);
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>
        /// A collection of fake objects of the specified type.
        /// </returns>
        public IList<T> CollectionOfFake<T>(int numberOfFakes)
        {
            return this.CollectionOfFake<T>(numberOfFakes, x => { });
        }

        public IList<T> CollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder)
        {
            var result = new List<T>();

            for (var i = 0; i < numberOfFakes; i++)
            {
                result.Add(this.CreateFake(optionsBuilder));
            }

            return result;
        }

        public IList<object> CollectionOfFake(Type typeOfFake, int numberOfFakes)
        {
            return this.CollectionOfFake(typeOfFake, numberOfFakes, x => { });
        }

        public IList<object> CollectionOfFake(Type typeOfFake, int numberOfFakes, Action<IFakeOptions> optionsBuilder)
        {
            var result = new List<object>();

            for (var i = 0; i < numberOfFakes; i++)
            {
                result.Add(this.CreateFake(typeOfFake, optionsBuilder));
            }

            return result;
        }

        /// <summary>
        /// Creates a dummy object, this can be a fake object or an object resolved
        /// from the current IFakeObjectContainer.
        /// </summary>
        /// <typeparam name="T">The type of dummy to create.</typeparam>
        /// <returns>The created dummy.</returns>
        /// <exception cref="FakeCreationException">Was unable to generate the fake in the current configuration and
        /// no dummy was registered in the container for the specified type..</exception>
        public T CreateDummy<T>()
        {
            return (T)this.fakeAndDummyManager.CreateDummy(typeof(T));
        }

        public object CreateDummy(Type typeOfDummy)
        {
            Guard.AgainstNull(typeOfDummy, nameof(typeOfDummy));

            return this.fakeAndDummyManager.CreateDummy(typeOfDummy);
        }

        /// <summary>
        /// Creates a collection of dummies of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of dummies to create.</typeparam>
        /// <param name="numberOfDummies">The number of dummies in the collection.</param>
        /// <returns>
        /// A collection of dummy objects of the specified type.
        /// </returns>
        public IList<T> CollectionOfDummy<T>(int numberOfDummies)
        {
            var result = new List<T>();

            for (var i = 0; i < numberOfDummies; i++)
            {
                result.Add(this.CreateDummy<T>());
            }

            return result;
        }

        public IList<object> CollectionOfDummy(Type typeOfDummy, int numberOfDummies)
        {
            Guard.AgainstNull(typeOfDummy, nameof(typeOfDummy));

            var result = new List<object>();

            for (var i = 0; i < numberOfDummies; i++)
            {
                result.Add(this.CreateDummy(typeOfDummy));
            }

            return result;
        }
    }
}
