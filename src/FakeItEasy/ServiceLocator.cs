namespace FakeItEasy
{
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static class ServiceLocator
    {
        static ServiceLocator()
        {
            RootModule.RegisterDependencies(new ServiceRegistrar());
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>An instance of the service type.</returns>
        [DebuggerStepThrough]
        internal static T Resolve<T>() where T : class
        {
            var service = Service<T>.Instance;
            return service is null
                ? throw new KeyNotFoundException($"The specified service {typeof(T)} was not registered.")
                : service;
        }

        private static class Service<T> where T : class
        {
            public static T? Instance { get; set; }
        }

        private class ServiceRegistrar : RootModule.IServiceRegistrar
        {
            public void Register<T>(T service) where T : class
            {
                Service<T>.Instance = service;
            }
        }
    }
}
