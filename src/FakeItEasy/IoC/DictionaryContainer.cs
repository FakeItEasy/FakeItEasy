namespace FakeItEasy.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// A simple implementation of an IoC container.
    /// </summary>
    internal class DictionaryContainer
    {
        /// <summary>
        /// The dictionary that stores the registered service factories.
        /// </summary>
        private readonly Dictionary<Type, Func<DictionaryContainer, object>> serviceFactories;

        private readonly Dictionary<Type, object> cachedServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryContainer"/> class.
        /// </summary>
        public DictionaryContainer()
        {
            this.serviceFactories = new Dictionary<Type, Func<DictionaryContainer, object>>();
            this.cachedServices = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Registers the specified resolver as a singleton.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="resolver">The resolver.</param>
        [DebuggerStepThrough]
        public void RegisterSingleton<T>(Func<DictionaryContainer, T> resolver)
        {
            this.serviceFactories.Add(typeof(T), c => (object)resolver.Invoke(c));
        }

        public IDictionary<Type, object> Build()
        {
            foreach (var registeredServiceType in this.serviceFactories.Keys)
            {
                this.Resolve(registeredServiceType);
            }

            return this.cachedServices;
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>An instance of the service type.</returns>
        public T Resolve<T>()
        {
            return (T)this.Resolve(typeof(T));
        }

        private object Resolve(Type serviceType)
        {
            if (this.cachedServices.TryGetValue(serviceType, out object cachedService))
            {
                return cachedService;
            }

            if (this.serviceFactories.TryGetValue(serviceType, out Func<DictionaryContainer, object> serviceFactory))
            {
                var serviceObject = serviceFactory.Invoke(this);
                this.cachedServices[serviceType] = serviceObject;
                return serviceObject;
            }

            throw new KeyNotFoundException($"The specified service {serviceType} was not registered in the container.");
        }
    }
}
