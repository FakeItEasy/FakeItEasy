namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using FakeItEasy.Configuration;
    using FakeItEasy.IoC;

    internal class ServiceLocator
    {
        private readonly IDictionary<Type, object> registeredServices;

        static ServiceLocator()
        {
            var containerBuilder = new DictionaryContainer();
            RootModule.RegisterDependencies(containerBuilder);
            ConfigurationModule.RegisterDependencies(containerBuilder);
            ImportsModule.RegisterDependencies(containerBuilder);
            Current = new ServiceLocator(containerBuilder.Build());
        }

        private ServiceLocator(IDictionary<Type, object> registeredServices)
        {
            this.registeredServices = registeredServices;
        }

        internal static ServiceLocator Current { get; }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>An instance of the service type.</returns>
        [DebuggerStepThrough]
        internal T Resolve<T>()
        {
            if (this.registeredServices.TryGetValue(typeof(T), out object service))
            {
                return (T)service;
            }

            throw new KeyNotFoundException($"The specified service {typeof(T)} was not registered.");
        }
    }
}
