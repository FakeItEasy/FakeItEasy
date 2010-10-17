namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// A IFakeObjectContainer implementation that uses mef to load IFakeDefinitions and
    /// IFakeConfigurations.
    /// </summary>
    public class DynamicContainer
        : IFakeObjectContainer
    {
        private IDictionary<Type, IDummyDefinition> registeredDummyDefinitions;
        private IDictionary<Type, IFakeConfigurer> registeredConfigurators;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicContainer"/> class.
        /// </summary>
        public DynamicContainer(IEnumerable<IDummyDefinition> dummyDefinitions, IEnumerable<IFakeConfigurer> fakeConfigurers)
        {
            this.registeredDummyDefinitions = CreateDummyDefinitionsDictionary(dummyDefinitions);
            this.registeredConfigurators = CreateFakeConfiguratorsDictionary(fakeConfigurers);       
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
            IDummyDefinition dummyDefinition = null;

            if (!this.registeredDummyDefinitions.TryGetValue(typeOfDummy, out dummyDefinition))
            {
                fakeObject = null;
                return false;
            }

            fakeObject = dummyDefinition.CreateDummy();
            return true;
        }

        /// <summary>
        /// Applies base configuration to a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type the fake object represents.</param>
        /// <param name="fakeObject">The fake object to configure.</param>
        public void ConfigureFake(Type typeOfFake, object fakeObject)
        {
            IFakeConfigurer configurator = null;

            if (this.registeredConfigurators.TryGetValue(typeOfFake, out configurator))
            {
                configurator.ConfigureFake(fakeObject);
            }
        }

        private static IDictionary<Type, IFakeConfigurer> CreateFakeConfiguratorsDictionary(IEnumerable<IFakeConfigurer> fakeConfigurers)
        {
            return fakeConfigurers.FirstFromEachKey(x => x.ForType);
        }

        private static IDictionary<Type, IDummyDefinition> CreateDummyDefinitionsDictionary(IEnumerable<IDummyDefinition> dummyDefinitions)
        {
            return dummyDefinitions.FirstFromEachKey(x => x.ForType);
        }
    }
}