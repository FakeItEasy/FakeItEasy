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
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        /// <summary>
        /// Registers the specified service.
        /// </summary>
        /// <typeparam name="T">The type of service to register.</typeparam>
        /// <param name="service">The service.</param>
        [DebuggerStepThrough]
        public void Register<T>(T service)
        {
            this.services[typeof(T)] = service;
        }

        public IDictionary<Type, object> Build() => this.services;
    }
}
