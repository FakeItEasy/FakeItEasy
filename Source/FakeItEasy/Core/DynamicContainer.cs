namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Creation;

    /// <summary>
    /// A fake object container that uses MEF to load <see cref="IDummyFactory"/>s and
    /// <see cref="IFakeOptionsBuilder"/>s.
    /// </summary>
    public class DynamicContainer
        : IFakeObjectContainer
    {
        private readonly IEnumerable<IDummyFactory> allDummyFactories;
        private readonly IEnumerable<IFakeOptionsBuilder> allFakeOptionsBuilders;
        private readonly ConcurrentDictionary<Type, IFakeOptionsBuilder> cachedFakeOptionsBuilders;
        private readonly ConcurrentDictionary<Type, IDummyFactory> cachedDummyFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicContainer" /> class.
        /// </summary>
        /// <param name="dummyFactories">The dummy factories.</param>
        /// <param name="fakeOptionsBuilders">The fake options builders.</param>
        public DynamicContainer(IEnumerable<IDummyFactory> dummyFactories, IEnumerable<IFakeOptionsBuilder> fakeOptionsBuilders)
        {
            this.allDummyFactories = dummyFactories.OrderByDescending(factory => factory.Priority).ToArray();
            this.allFakeOptionsBuilders = fakeOptionsBuilders.OrderByDescending(factory => factory.Priority).ToArray();
            this.cachedDummyFactories = new ConcurrentDictionary<Type, IDummyFactory>();
            this.cachedFakeOptionsBuilders = new ConcurrentDictionary<Type, IFakeOptionsBuilder>();
        }

        /// <summary>
        /// Creates a fake object of the specified type using the specified arguments if it's
        /// supported by the container, returns a value indicating if it's supported or not.
        /// </summary>
        /// <param name="typeOfDummy">The type of fake object to create.</param>
        /// <param name="fakeObject">The fake object that was created if the method returns true.</param>
        /// <returns>True if a fake object can be created.</returns>
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

        /// <summary>
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeOptions">The options to build for the fake's creation.</param>
        public void BuildOptions(Type typeOfFake, IFakeOptions fakeOptions)
        {
            var fakeOptionsBuilder = this.cachedFakeOptionsBuilders.GetOrAdd(
                typeOfFake,
                type => this.allFakeOptionsBuilders.FirstOrDefault(builder => builder.CanBuildOptionsForFakeOfType(type)));

            if (fakeOptionsBuilder != null)
            {
                fakeOptionsBuilder.BuildOptions(typeOfFake, fakeOptions);
            }
        }
    }
}
