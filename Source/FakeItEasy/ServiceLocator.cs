using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Api;
using FakeItEasy.Expressions;
using System.Diagnostics;
using FakeItEasy.IoC;
using FakeItEasy.Configuration;

namespace FakeItEasy
{
    internal abstract class ServiceLocator
    {
        static ServiceLocator()
        {
            var container = new DictionaryContainer();
            new RootModule().RegisterDependencies(container);
            new ConfigurationModule().RegisterDependencies(container);
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