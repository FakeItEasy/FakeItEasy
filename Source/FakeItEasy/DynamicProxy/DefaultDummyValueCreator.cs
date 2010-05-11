namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;

    internal class DefaultDummyValueCreator
        : IDummyValueCreator
    {
        private IConstructorResolver constructorResolver;
        private IFakeObjectContainer container;
        private IProxyGenerator proxyGenerator;
        private IDummyResolvingSession session;
        private List<DummyResolverFunction> resolverFunctions;

        public DefaultDummyValueCreator(IProxyGenerator proxyGenerator, IFakeObjectContainer container, IConstructorResolver constructorResolver, IDummyResolvingSession session)
        {
            this.proxyGenerator = proxyGenerator;
            this.container = container;
            this.session = session;
            this.constructorResolver = constructorResolver;

            this.InitializeResolverFunctions();
        }

        private delegate bool DummyResolverFunction(Type type, out object dummy);

        /// <summary>
        /// Tries to create dummy value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dummy">The dummy.</param>
        /// <returns></returns>
        public bool TryCreateDummyValue(Type type, out object dummy)
        {
            if (this.session.TypeHasFailedToResolve(type))
            {
                dummy = null;
                return false;
            }

            this.session.RegisterTriedToResolveType(type);

            if (this.TryResolveFromSessionCache(type, out dummy))
            {
                return true;
            }

            if (this.TryResolveByUsingRegisteredResolverFunctions(type, out dummy))
            {
                this.session.AddResolvedValueToCache(type, dummy);
                return true;
            }

            dummy = null;
            return false;
        }

        private static bool IsTypeThatCanNotBeActivated(Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        private void InitializeResolverFunctions()
        {
            this.resolverFunctions = new List<DummyResolverFunction> 
		            {
		                this.TryResolveFromContainer,
		                this.TryResolveAsValueType,
		                this.TryResolveAsProxy,
		                this.TryResolveByActivating
		            };
        }

        private bool TryResolveByUsingRegisteredResolverFunctions(Type type, out object dummy)
        {
            foreach (var resolver in this.resolverFunctions)
            {
                if (resolver.Invoke(type, out dummy))
                {
                    return true;
                }
            }

            dummy = null;
            return false;
        }

        private bool TryResolveFromSessionCache(Type type, out object dummy)
        {
            if (this.session.TryGetCachedValue(type, out dummy))
            {
                return true;
            }

            dummy = null;
            return false;
        }

        private bool TryResolveFromContainer(Type type, out object dummy)
        {
            if (this.container.TryCreateFakeObject(type, out dummy))
            {
                return true;
            }

            dummy = null;
            return false;
        }

        private bool TryResolveAsValueType(Type type, out object dummy)
        {
            if (type.IsValueType)
            {
                dummy = Activator.CreateInstance(type);
                return true;
            }

            dummy = null;
            return false;
        }

        private bool TryResolveAsProxy(Type type, out object dummy)
        {
            var result = this.proxyGenerator.GenerateProxy(type, Enumerable.Empty<Type>(), new FakeManager(), null);

            if (result.ProxyWasSuccessfullyCreated)
            {
                dummy = result.Proxy;
                return true;
            }

            dummy = null;
            return false;
        }

        private bool TryResolveByActivating(Type type, out object dummy)
        {
            if (IsTypeThatCanNotBeActivated(type))
            {
                dummy = null;
                return false;
            }

            foreach (var constructor in this.GetSuccessfullyResolvedConstructors(type))
            {
                try
                {
                    dummy = Activator.CreateInstance(type, constructor.ArgumentsToUse.ToArray());
                    return true;
                }
                catch
                {
                }
            }

            dummy = null;
            return false;
        }

        private IEnumerable<ConstructorAndArgumentsInfo> GetSuccessfullyResolvedConstructors(Type type)
        {
            return
                from constructor in this.constructorResolver.ListAllConstructors(type)
                where constructor.Arguments.All(x => x.WasSuccessfullyResolved)
                select constructor;
        }
    }
}
