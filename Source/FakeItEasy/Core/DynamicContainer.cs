namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// A IFakeObjectContainer implementation that uses MEF to load IFakeDefinitions and
    /// IFakeConfigurations.
    /// </summary>
    public class DynamicContainer
        : IFakeObjectContainer
    {
        private readonly IEnumerable<IDummyDefinition> allDummyDefinitions;
        private readonly IEnumerable<IFakeConfigurator> allFakeConfigurators;
        private readonly ConcurrentDictionary<Type, IFakeConfigurator> cachedFakeConfigurators;
        private readonly ConcurrentDictionary<Type, IDummyDefinition> cachedDummyDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicContainer" /> class.
        /// </summary>
        /// <param name="dummyDefinitions">The dummy definitions.</param>
        /// <param name="fakeConfigurators">The fake configurators.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurators", Justification = "This is the correct spelling.")]
        public DynamicContainer(IEnumerable<IDummyDefinition> dummyDefinitions, IEnumerable<IFakeConfigurator> fakeConfigurators)
        {
            this.allDummyDefinitions = dummyDefinitions.OrderByDescending(definition => definition.Priority).ToArray();
            this.allFakeConfigurators = fakeConfigurators.OrderByDescending(definition => definition.Priority).ToArray();
            this.cachedDummyDefinitions = new ConcurrentDictionary<Type, IDummyDefinition>();
            this.cachedFakeConfigurators = new ConcurrentDictionary<Type, IFakeConfigurator>();
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
            var dummyDefinition = this.cachedDummyDefinitions.GetOrAdd(
                typeOfDummy,
                type => this.allDummyDefinitions.FirstOrDefault(definition => definition.CanCreateDummyOfType(type)));

            if (dummyDefinition == null)
            {
                fakeObject = null;
                return false;
            }

            fakeObject = dummyDefinition.CreateDummyOfType(typeOfDummy);
            return true;
        }

        /// <summary>
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeObject">The fake object to configure.</param>
        public void ConfigureFake(Type typeOfFake, object fakeObject)
        {
            var fakeConfigurator = this.cachedFakeConfigurators.GetOrAdd(
                typeOfFake,
                type => this.allFakeConfigurators.FirstOrDefault(configurator => configurator.CanConfigureFakeOfType(type)));

            if (fakeConfigurator != null)
            {
                fakeConfigurator.ConfigureFake(fakeObject);
            }
        }
    }
}