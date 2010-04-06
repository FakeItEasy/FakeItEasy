namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;

    /// <summary>
    /// An implementation of the IProxyGenerator interface that uses DynamicProxy2 to
    /// generate proxies.
    /// </summary>
    internal class DynamicProxyProxyGenerator
         : IProxyGenerator
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static Type[] interfacesToImplement = new Type[] { typeof(IFakedProxy), typeof(ICanInterceptObjectMembers) };
        private IFakeObjectContainer container;

        public DynamicProxyProxyGenerator(IFakeObjectContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="additionalInterfacesToImplement">Any extra interfaces to be implemented by the generated proxy.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="argumentsForConstructor">Arguments to use for the constructor of the proxied type.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        public ProxyResult GenerateProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor)
        {
            if (typeToProxy.IsValueType || typeToProxy.IsSealed)
            {
                return new DynamicProxyResult(typeToProxy, "");
            }
            
            AssertThatArgumentsForConstructorAreNotSpecifiedForInterfaceType(typeToProxy, argumentsForConstructor);

            return DoGenerateProxy(typeToProxy, additionalInterfacesToImplement, fakeObject, argumentsForConstructor);
        }

        /// <summary>
        /// Gets a value indicating if the specified member can be intercepted on a proxied
        /// instance.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>
        /// True if the member can be intercepted, otherwise false.
        /// </returns>
        public bool MemberCanBeIntercepted(MemberInfo member)
        {
            var method = member as MethodInfo;

            if (method != null)
            {
                return method.IsVirtual;
            }

            var property = (PropertyInfo)member;
            return property.GetAccessors().All(x => x.IsVirtual);
        }

        private static void AssertThatArgumentsForConstructorAreNotSpecifiedForInterfaceType(Type typeToProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (typeToProxy.IsInterface && argumentsForConstructor != null)
            {
                throw new ArgumentException(ExceptionMessages.ArgumentsForConstructorOnInterfaceType, "argumentsForConstructor");
            }
        }

        private static DynamicProxyResult CreateInterfaceProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeObjectInterceptor fakeObjectInterceptor)
        {
            var proxyResult = new DynamicProxyResult(typeToProxy);
            proxyResult.Proxy = (IFakedProxy)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, GetAllInterfacesToImplement(additionalInterfacesToImplement), fakeObjectInterceptor, proxyResult);
            return proxyResult;
        }

        private ProxyResult DoGenerateProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor)
        {
            var fakeObjectInterceptor = CreateFakeObjectInterceptor(fakeObject);

            if (typeToProxy.IsInterface)
            {
                return CreateInterfaceProxy(typeToProxy, additionalInterfacesToImplement, fakeObjectInterceptor);
            }
            
            return this.CreateClassProxy(typeToProxy, additionalInterfacesToImplement, fakeObjectInterceptor, argumentsForConstructor);
        }

        private DynamicProxyResult CreateClassProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeObjectInterceptor fakeObjectInterceptor, IEnumerable<object> argumentsForConstructor)
        {
            var candidateConstructorArguments = this.CreateConstructorArgumentCandidateSets(typeToProxy, argumentsForConstructor);

            DynamicProxyResult result = null;
            if (!this.TryGenerateProxyUsingCandidateConstructorArguments(typeToProxy, additionalInterfacesToImplement, fakeObjectInterceptor, candidateConstructorArguments, out result))
            {
                result = new DynamicProxyResult(typeToProxy, "false");
            }

            return result;
        }

        private IEnumerable<IEnumerable<object>> CreateConstructorArgumentCandidateSets(Type typeToProxy, IEnumerable<object> argumentsForConstructor)
        {
            IEnumerable<IEnumerable<object>> argumentSetsForConstructor = null;

            if (argumentsForConstructor != null)
            {
                argumentSetsForConstructor = new[] { argumentsForConstructor };
            }
            else
            {
                argumentSetsForConstructor =
                    from constructor in this.ResolveConstructors(typeToProxy)
                    select constructor.ArgumentsToUse;
            }

            return argumentSetsForConstructor;
        }

        private bool TryGenerateProxyUsingCandidateConstructorArguments(
            Type typeToProxy, 
            IEnumerable<Type> additionalInterfacesToImplement,
            FakeObjectInterceptor fakeObjectInterceptor, 
            IEnumerable<IEnumerable<object>> argumentsForConstructor, 
            out DynamicProxyResult proxyResult)
        {
            foreach (var argumentSet in argumentsForConstructor)
            {
                try
                {
                    proxyResult = new DynamicProxyResult(typeToProxy);

                    proxyResult.Proxy = (IFakedProxy)proxyGenerator.CreateClassProxy(
                        typeToProxy,
                        GetAllInterfacesToImplement(additionalInterfacesToImplement),
                        ProxyGenerationOptions.Default,
                        argumentSet.ToArray(),
                        fakeObjectInterceptor,
                        proxyResult);

                    return true;
                }
                catch (Exception)
                {
                }
            }

            proxyResult = null;
            return false;
        }

        private static Type[] GetAllInterfacesToImplement(IEnumerable<Type> additionalInterfacesToImplement)
        { 
            if (additionalInterfacesToImplement == null)
            {
                additionalInterfacesToImplement = Enumerable.Empty<Type>();
            }

            return interfacesToImplement.Concat(additionalInterfacesToImplement).ToArray();
        }

        private IEnumerable<ConstructorAndArgumentsInfo> ResolveConstructors(Type typeToProxy)
        {
            var resolver = new ConstructorResolver(typeToProxy, this.container);
            return resolver.ResolveConstructors();
        }

        private static FakeObjectInterceptor CreateFakeObjectInterceptor(FakeObject fakeObject)
        {
            return new FakeObjectInterceptor()
            {
                FakeObject = fakeObject
            };
        }

        private class ConstructorAndArgumentsInfo
        {
            public IEnumerable<object> ArgumentsToUse { get; set; }
        }

        private class ConstructorResolver
        {
            private IDictionary<Type, object> resolvedValues;
            private IFakeObjectContainer container;
            private Type typeToResolvedConstructorFor;
            private ConstructorResolver parent;

            public ConstructorResolver(Type typeToResolveConstructorFor, IFakeObjectContainer container)
                : this(typeToResolveConstructorFor, new Dictionary<Type, object>(), container)
            {
            }

            private ConstructorResolver(Type typeToResolveConstructorFor, IDictionary<Type, object> resolvedValues, IFakeObjectContainer container)
            {
                this.typeToResolvedConstructorFor = typeToResolveConstructorFor;
                this.resolvedValues = resolvedValues;
                this.container = container;
            }

            private ConstructorResolver(Type typeToResolveConstructorFor, ConstructorResolver parent)
                : this(typeToResolveConstructorFor, parent.resolvedValues, parent.container)
            {
                this.parent = parent;
            }

            public IEnumerable<ConstructorAndArgumentsInfo> ResolveConstructors()
            { 
                var constructors = GetConstructorsCallableByProxy();
                            
                foreach (var constructor in constructors)
                {
                    IEnumerable<object> argumentValues = null;
                    if (this.TryResolveAllValues(GetConstructorParameterTypes(constructor), out argumentValues))
                    {
                        yield return new ConstructorAndArgumentsInfo()
                        {
                            ArgumentsToUse = argumentValues
                        };
                    }
                }
            }

            private static IEnumerable<Type> GetConstructorParameterTypes(ConstructorInfo constructor)
            {
                return constructor.GetParameters().Select(x => x.ParameterType);
            }

            private IOrderedEnumerable<ConstructorInfo> GetConstructorsCallableByProxy()
            {
                return
                    from constructor in this.typeToResolvedConstructorFor.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    where !constructor.IsPrivate
                    orderby constructor.GetParameters().Length descending
                    select constructor;
            }

            private bool TryResolveDummyValueOfType(Type typeOfValue, out object dummyValue)
            {
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
                    dummyValue = DynamicProxyProxyGenerator.proxyGenerator.CreateInterfaceProxyWithoutTarget(typeOfValue);
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
                    var resolver = new ConstructorResolver(typeOfValue, this);
                    var constructor = resolver.ResolveConstructors().FirstOrDefault();

                    if (constructor != null)
                    {
                        try
                        {
                            dummyValue = Activator.CreateInstance(typeOfValue, constructor.ArgumentsToUse.ToArray());
                            this.resolvedValues.Add(typeOfValue, dummyValue);
                            return true;
                        }
                        catch (TargetInvocationException)
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

            private bool TryResolveAllValues(IEnumerable<Type> types, out IEnumerable<object> resolvedValues)
            {
                var result = new List<object>();

                foreach (var parameterType in types)
                {
                    object value = null;

                    if (!this.TryResolveDummyValueOfType(parameterType, out value))
                    {
                        resolvedValues = null;
                        return false;
                    }

                    result.Add(value);
                }

                resolvedValues = result;
                return true;
            }
        }

        [Serializable]
        private class FakeObjectInterceptor
            : IInterceptor
        {
            private static readonly MethodInfo getProxyManagerMethod = typeof(IFakedProxy).GetProperty("FakeObject").GetGetMethod();

            public FakeObject FakeObject;

            [DebuggerStepThrough]
            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.Equals(getProxyManagerMethod))
                {
                    invocation.ReturnValue = this.FakeObject;
                }
                else
                {
                    invocation.Proceed();
                }
            }
        }

        [Serializable]
        private class DynamicProxyResult
            : ProxyResult, IInterceptor
        {
            public DynamicProxyResult(Type typeOfProxy)
                : base(typeOfProxy)
            {
                this.ProxyWasSuccessfullyCreated = true;
            }

            public DynamicProxyResult(Type typeToProxy, string errorMessage)
                : this(typeToProxy)
            {
                this.ProxyWasSuccessfullyCreated = false;
                this.ErrorMessage = errorMessage;
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;

            [DebuggerStepThrough]
            public void Intercept(IInvocation invocation)
            {
                var handler = this.CallWasIntercepted;
                if (handler != null)
                {
                    var call = new InvocationCallAdapter(invocation);
                    handler(this.Proxy, new CallInterceptedEventArgs(call));
                }
            }

            public new IFakedProxy Proxy
            {
                [DebuggerStepThrough]
                get
                {
                    return base.Proxy;
                }
                [DebuggerStepThrough]
                set
                {
                    base.Proxy = value;
                }
            }
        }
    }
}