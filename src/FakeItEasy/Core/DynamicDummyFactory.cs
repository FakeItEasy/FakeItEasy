namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Tries to create Dummies by invoking an appropriate <see cref="IDummyFactory"/>.
    /// </summary>
    internal class DynamicDummyFactory
    {
        private readonly IEnumerable<IDummyFactory> allDummyFactories;
        private readonly ConcurrentDictionary<Type, IDummyFactory> cachedDummyFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDummyFactory" /> class.
        /// </summary>
        /// <param name="dummyFactories">The dummy factories.</param>
        public DynamicDummyFactory(IEnumerable<IDummyFactory> dummyFactories)
        {
            this.allDummyFactories = dummyFactories.OrderByDescending(factory => factory.Priority).ToArray();
            this.cachedDummyFactories = new ConcurrentDictionary<Type, IDummyFactory>();
        }

        /// <summary>
        /// Creates a Dummy object of the specified type if it's supported by the supplied factories.
        /// </summary>
        /// <param name="typeOfDummy">The type of Dummy object to create.</param>
        /// <param name="fakeObject">The Dummy object that was created, if the method returns true.</param>
        /// <returns>True if a Dummy object can be created.</returns>
        public bool TryCreateDummyObject(Type typeOfDummy, out object fakeObject)
        {
            var dummyFactory = this.cachedDummyFactories.GetOrAdd(
                typeOfDummy,
                type => this.allDummyFactories.FirstOrDefault(factory => factory.CanCreate(type)));

            if (dummyFactory == null)
            {
                fakeObject = null;
                return false;
            }

            fakeObject = dummyFactory.Create(typeOfDummy);
            return true;
        }
    }
}
