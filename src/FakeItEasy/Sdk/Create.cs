namespace FakeItEasy.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using FakeItEasy.Creation;

    /// <summary>
    /// Provides methods for generating fake objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = nameof(A), Justification = "Is spelled correctly.")]
    public static class Create
    {
        private static IFakeAndDummyManager FakeAndDummyManager =>
            ServiceLocator.Current.Resolve<IFakeAndDummyManager>();

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to create.</param>
        /// <returns>A fake object.</returns>
        public static object Fake(Type typeOfFake)
        {
            return Fake(typeOfFake, x => { });
        }

        /// <summary>
        /// Creates a fake object of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to create.</param>
        /// <param name="optionsBuilder">A lambda where options for the built fake object can be specified.</param>
        /// <returns>A fake object.</returns>
        public static object Fake(Type typeOfFake, Action<IFakeOptions> optionsBuilder)
        {
            Guard.AgainstNull(typeOfFake, nameof(typeOfFake));
            Guard.AgainstNull(optionsBuilder, nameof(optionsBuilder));

            return FakeAndDummyManager.CreateFake(typeOfFake, optionsBuilder);
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fakes to create.</param>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        public static IList<object> CollectionOfFake(Type typeOfFake, int numberOfFakes)
        {
            return Enumerable.Range(0, numberOfFakes).Select(_ => Fake(typeOfFake)).ToList();
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <param name="typeOfFake">The type of fakes to create.</param>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <param name="optionsBuilder">A lambda where options for the built fake object can be specified.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        public static IList<object> CollectionOfFake(Type typeOfFake, int numberOfFakes, Action<IFakeOptions> optionsBuilder)
        {
            return Enumerable.Range(0, numberOfFakes).Select(_ => Fake(typeOfFake, optionsBuilder)).ToList();
        }

        /// <summary>
        /// Gets a dummy object of the specified type. The value of a dummy object
        /// should be irrelevant. Dummy objects should not be configured.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy to return.</param>
        /// <returns>A dummy object of the specified type.</returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static object Dummy(Type typeOfDummy)
        {
            Guard.AgainstNull(typeOfDummy, nameof(typeOfDummy));

            return FakeAndDummyManager.CreateDummy(typeOfDummy);
        }

        /// <summary>
        /// Creates a collection of dummies of the specified type.
        /// </summary>
        /// <param name="typeOfDummy">The type of dummy to return.</param>
        /// <param name="numberOfDummies">The number of dummies in the collection.</param>
        /// <returns>A collection of dummy objects of the specified type.</returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IList<object> CollectionOfDummy(Type typeOfDummy, int numberOfDummies)
        {
            return Enumerable.Range(0, numberOfDummies).Select(_ => Dummy(typeOfDummy)).ToList();
        }
    }
}
