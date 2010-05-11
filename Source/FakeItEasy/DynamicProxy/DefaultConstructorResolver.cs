namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;

    /// <summary>
    /// The default implementation of the <see cref="IConstructorResolver" /> interface.
    /// </summary>
    internal class DefaultConstructorResolver
        : IConstructorResolver
    {
        private IDictionary<Type, object> resolvedValues;
        private Type typeToResolvedConstructorFor;
        private DefaultConstructorResolver parent;
        private IFakeObjectContainer container;
        private static readonly HashSet<Type> forbiddenTypes = new HashSet<Type>() { typeof(IntPtr) };

        public DefaultConstructorResolver(IFakeObjectContainer container)
        {
            this.container = container;
        }

        public DefaultConstructorResolver(Type typeToResolveConstructorFor, IFakeObjectContainer container)
            : this(typeToResolveConstructorFor, new Dictionary<Type, object>(), container)
        {
        }

        /// <summary>
        /// Gets all the accessible constructor for the type along with dummy arguments
        /// where they can be resolved.
        /// </summary>
        /// <param name="type">The type to list constructors for.</param>
        /// <returns>A collection of constructors.</returns>
        public IEnumerable<ConstructorAndArgumentsInfo> ListAllConstructors(Type type)
        {
            var resolver = new DefaultConstructorResolver(type, this.container);
            return resolver.ListAllConstructors();
        }

        private DefaultConstructorResolver(Type typeToResolveConstructorFor, IDictionary<Type, object> resolvedValues, IFakeObjectContainer container)
        {
            this.typeToResolvedConstructorFor = typeToResolveConstructorFor;
            this.resolvedValues = resolvedValues;
            this.container = container;
        }

        private DefaultConstructorResolver(Type typeToResolveConstructorFor, DefaultConstructorResolver parent)
            : this(typeToResolveConstructorFor, parent.resolvedValues, parent.container)
        {
            this.parent = parent;
        }

        public IEnumerable<ConstructorAndArgumentsInfo> ListAllConstructors()
        {
            var constructors = this.GetConstructorsCallableByProxy();

            foreach (var constructor in constructors)
            {
                IEnumerable<ArgumentInfo> arguments = this.TryToResolveAllArguments(GetConstructorParameterTypes(constructor));
                yield return new ConstructorAndArgumentsInfo()
                {
                    Constructor = constructor,
                    Arguments = arguments
                };
            }
        }

        public IEnumerable<ConstructorAndArgumentsInfo> GetConstructorsWhereAllArgumentsCanBeResolved()
        {
            return
                from constructor in this.ListAllConstructors()
                where constructor.Arguments.All(x => x.WasSuccessfullyResolved)
                select constructor;
        }

        private static IEnumerable<Type> GetConstructorParameterTypes(ConstructorInfo constructor)
        {
            return constructor.GetParameters().Select(x => x.ParameterType);
        }

        private IEnumerable<ConstructorInfo> GetConstructorsCallableByProxy()
        {
            return
                from constructor in this.typeToResolvedConstructorFor.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where !constructor.IsPrivate
                select constructor;
        }

        private class NullInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {

            }
        }


        private bool TryResolveDummyValueOfType(Type typeOfValue, out object dummyValue)
        {
            if (forbiddenTypes.Contains(typeOfValue))
            {
                dummyValue = null;
                return false;
            }

            if (this.resolvedValues.TryGetValue(typeOfValue, out dummyValue))
            {
                return true;
            }

            if (this.container.TryCreateFakeObject(typeOfValue, out dummyValue))
            {
                this.resolvedValues.Add(typeOfValue, dummyValue);
                return true;
            }

            if (typeOfValue.IsInterface)
            {
                dummyValue = DynamicProxyProxyGenerator.proxyGenerator.CreateInterfaceProxyWithoutTarget(typeOfValue, new NullInterceptor());
                this.resolvedValues.Add(typeOfValue, dummyValue);
                return true;
            }

            if (typeOfValue.IsValueType)
            {
                dummyValue = Activator.CreateInstance(typeOfValue);
                this.resolvedValues.Add(typeOfValue, dummyValue);
                return true;
            }

            if (!this.IsRecursiveType(typeOfValue))
            {
                var resolver = new DefaultConstructorResolver(typeOfValue, this);
                var constructor = resolver.GetConstructorsWhereAllArgumentsCanBeResolved().FirstOrDefault();

                if (constructor != null)
                {
                    try
                    {
                        dummyValue = DynamicProxyProxyGenerator.proxyGenerator.CreateClassProxy(typeOfValue, new Type[] { }, ProxyGenerationOptions.Default, constructor.ArgumentsToUse.ToArray());
                        this.resolvedValues.Add(typeOfValue, dummyValue);
                        return true;
                    }
                    catch
                    {
                    }

                    try
                    {
                        dummyValue = Activator.CreateInstance(typeOfValue, constructor.ArgumentsToUse.ToArray());
                        this.resolvedValues.Add(typeOfValue, dummyValue);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }

            dummyValue = null;
            return false;
        }

        private bool IsRecursiveType(Type typeOfValue)
        {
            if (this.parent == null)
            {
                return false;
            }

            return this.parent.IsRecursiveType(typeOfValue) || typeOfValue.Equals(this.parent.typeToResolvedConstructorFor);
        }

        private IEnumerable<ArgumentInfo> TryToResolveAllArguments(IEnumerable<Type> types)
        {
            return
                from parameterType in types
                select this.CreateArgumentInfo(parameterType);
        }

        private ArgumentInfo CreateArgumentInfo(Type typeOfArgument)
        {
            object argumentValue = null;
            var wasResolved = this.TryResolveDummyValueOfType(typeOfArgument, out argumentValue);

            return new ArgumentInfo
            {
                WasSuccessfullyResolved = wasResolved,
                ResolvedValue = argumentValue,
                TypeOfArgument = typeOfArgument
            };
        }
    }
}
