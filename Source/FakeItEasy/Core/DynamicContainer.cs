namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics.CodeAnalysis;

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
        public DynamicContainer(ITypeCatalogue typeAccessor)
        {
            this.registeredDummyDefinitions = CreateDummyDefinitionsDictionary(typeAccessor);
            this.registeredConfigurators = CreateFakeConfiguratorsDictionary(typeAccessor);       
        }

        /// <summary>
        /// Creates a fake object of the specified type using the specified arguments if it's
        /// supported by the container, returns a value indicating if it's supported or not.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to create.</param>
        /// <param name="fakeObject">The fake object that was created if the method returns true.</param>
        /// <returns>True if a fake object can be created.</returns>
        public bool TryCreateDummyObject(Type typeOfFake, out object fakeObject)
        {
            IDummyDefinition dummyDefinition = null;

            if (!this.registeredDummyDefinitions.TryGetValue(typeOfFake, out dummyDefinition))
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

        private static IDictionary<Type, IFakeConfigurer> CreateFakeConfiguratorsDictionary(ITypeCatalogue typeAccessor)
        {
            return GetOneInstancePerTypeImplementing<IFakeConfigurer>(typeAccessor).FirstFromEachKey(x => x.ForType);
        }

        private static IDictionary<Type, IDummyDefinition> CreateDummyDefinitionsDictionary(ITypeCatalogue typeAccessor)
        {
            return GetOneInstancePerTypeImplementing<IDummyDefinition>(typeAccessor).FirstFromEachKey(x => x.ForType);
        }

        private static IEnumerable<TInterface> GetOneInstancePerTypeImplementing<TInterface>(ITypeCatalogue typeAccessor)
        {
            return from type in typeAccessor.GetAvailableTypes()
                   where type.GetInterfaces().Contains(typeof(TInterface))
                   let instance = TryCreateInstance(type)
                   where instance != null
                   select (TInterface)instance;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Used to determine if an instance could be created or not.")]
        private static object TryCreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}