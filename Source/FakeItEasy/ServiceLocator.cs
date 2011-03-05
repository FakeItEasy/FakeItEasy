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
            new RootModule().RegisterDependencies(container);
            new ConfigurationModule().RegisterDependencies(container);
            new ImportsModule().RegisterDependencies(container);
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

    public static class ComponentStore
    {
        private static IDependencyResolver currentResolver = CreateDefaultResolver();

        private static IDependencyResolver CreateDefaultResolver()
        {
            return new ServiceLocatorDependencyResolver();
        }

        private class ServiceLocatorDependencyResolver
            : IDependencyResolver, IDependencyScope
        {
            public object Resolve(Type typeOfDependency)
            {
                return ServiceLocator.Current.Resolve(typeOfDependency);
            }

            public void Dispose()
            {
                
            }

            public IDependencyScope CreateScope()
            {
                return this;
            }
        }


        public static IDisposable RegisterDependencyResolver(IDependencyResolver factory)
        {
            currentResolver = factory;
            return A.Fake<IDisposable>();
        }

        public static IDependencyScope BeginResolve()
        {
            return currentResolver.CreateScope();
        }
    }

    public interface IDependencyResolver
    {
        IDependencyScope CreateScope();
    }

    public interface IDependencyScope
        : IDisposable
    {
        object Resolve(Type typeOfDependency);
    }
}