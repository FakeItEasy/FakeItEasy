namespace FakeItEasy.Core
{
    using System;
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
        private readonly IDictionary<Type, IFakeConfigurator> registeredConfigurators;
        private readonly Cache<Type, IDummyDefinition> registeredDummyDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicContainer" /> class.
        /// </summary>
        /// <param name="dummyDefinitions">The dummy definitions.</param>
        /// <param name="fakeConfigurators">The fake configurators.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurators", Justification = "This is the correct spelling.")]
        public DynamicContainer(IEnumerable<IDummyDefinition> dummyDefinitions, IEnumerable<IFakeConfigurator> fakeConfigurators)
        {
            var dummyDefinitionsList = CreateDummyDefinitionsList(dummyDefinitions);
            this.registeredDummyDefinitions = new Cache<Type, IDummyDefinition>(type => GetDummyDefinition(type, dummyDefinitionsList));
            this.registeredConfigurators = CreateFakeConfiguratorsDictionary(fakeConfigurators);
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
            var dummyDefinition = this.registeredDummyDefinitions[typeOfDummy];

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
            IFakeConfigurator configurator = null;

            if (this.registeredConfigurators.TryGetValue(typeOfFake, out configurator))
            {
                configurator.ConfigureFake(fakeObject);
            }
        }

        private static IDummyDefinition GetDummyDefinition(Type typeOfDummy, IEnumerable<IDummyDefinition> dummyDefinitions)
        {
            IDummyDefinition dummyDefinition = dummyDefinitions
                .Where(definition => definition.CanCreateDummyOfType(typeOfDummy))
                .OrderByDescending(definition => definition.Priority)
                .FirstOrDefault();
            return dummyDefinition;
        }

        private static IDictionary<Type, IFakeConfigurator> CreateFakeConfiguratorsDictionary(IEnumerable<IFakeConfigurator> fakeConfigurers)
        {
            return fakeConfigurers.FirstFromEachKey(x => x.ForType);
        }

        private static IList<IDummyDefinition> CreateDummyDefinitionsList(IEnumerable<IDummyDefinition> dummyDefinitions)
        {
            return dummyDefinitions.ToList();
        }
    }
}