namespace FakeItEasy
{
    using System;
    using System.Diagnostics;
    using FakeItEasy.Configuration;
    using FakeItEasy.IoC;

    internal abstract class ServiceLocator
    {
        static ServiceLocator()
        {
            var container = new DictionaryContainer();
            RootModule.RegisterDependencies(container);
            ConfigurationModule.RegisterDependencies(container);
            ImportsModule.RegisterDependencies(container);
            Current = container;
        }

        internal static ServiceLocator Current { get; set; }

        [DebuggerStepThrough]
        internal T Resolve<T>()
        {
            return (T)this.Resolve(typeof(T));
        }

        [DebuggerStepThrough]
        internal abstract object Resolve(Type componentType);
    }
}
