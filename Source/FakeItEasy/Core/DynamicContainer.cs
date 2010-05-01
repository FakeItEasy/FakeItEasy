namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A IFakeObjectContainer implementation that uses mef to load IFakeDefinitions and
    /// IFakeConfigurations.
    /// </summary>
    public class DynamicContainer
        : IFakeObjectContainer
    {
        private Dictionary<Type, IDummyDefinition> registeredDummyDefinitions;
        private Dictionary<Type, IFakeConfigurator> registeredConfigurators;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicContainer"/> class.
        /// </summary>
        public DynamicContainer()
        {
            var assemblyLocation = Path.GetDirectoryName(typeof(Fake).Assembly.Location);

            this.registeredDummyDefinitions =
                (from assemblyFile in Directory.GetFiles(assemblyLocation, "*.dll")
                 let assembly = Assembly.LoadFile(assemblyFile)
                 from type in assembly.GetTypes()
                 where type.GetInterfaces().Contains(typeof(IDummyDefinition))
                 let instance = TryCreateInstance(type)
                 where instance != null
                 select (IDummyDefinition)instance).ToDictionary(x => x.ForType);

            this.registeredConfigurators =
                (from assemblyFile in Directory.GetFiles(assemblyLocation, "*.dll")
                 let assembly = Assembly.LoadFile(assemblyFile)
                 from type in assembly.GetTypes()
                 where type.GetInterfaces().Contains(typeof(IFakeConfigurator))
                 let instance = TryCreateInstance(type)
                 where instance != null
                 select (IFakeConfigurator)instance).ToDictionary(x => x.ForType);
        }

        /// <summary>
        /// Creates a fake object of the specified type using the specified arguments if it's
        /// supported by the container, returns a value indicating if it's supported or not.
        /// </summary>
        /// <param name="typeOfFake">The type of fake object to create.</param>
        /// <param name="fakeObject">The fake object that was created if the method returns true.</param>
        /// <returns>True if a fake object can be created.</returns>
        public bool TryCreateFakeObject(Type typeOfFake, out object fakeObject)
        {
            IDummyDefinition dummyDefinition = null;

            if (!this.registeredDummyDefinitions.TryGetValue(typeOfFake, out dummyDefinition))
            {
                fakeObject = null;
                return false;
            }

            fakeObject = dummyDefinition.CreateFake();
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
