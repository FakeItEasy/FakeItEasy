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

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="argumentsForConstructor">Arguments to use for the constructor of the proxied type.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        public ProxyResult GenerateProxy(Type typeToProxy, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor)
        {
            ProxyResult result = null;

            if (!TryGenerateProxy(typeToProxy, fakeObject, argumentsForConstructor, out result))
            {
                result = new DynamicProxyResult(typeToProxy, string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="container">A fake object container the proxy generator can use to get arguments for constructor.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        public ProxyResult GenerateProxy(Type typeToProxy, FakeObject fakeObject, IFakeObjectContainer container)
        {
            ProxyResult result = null;

            if (!TryGenerateProxy(typeToProxy, fakeObject, container, out result))
            {
                result = new DynamicProxyResult(typeToProxy, string.Empty);
            }

            return result;
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
                return MethodCanBeIntercepted(method);
            }

            var property = member as PropertyInfo;

            if (property != null)
            {
                return PropertyCanBeIntercepted(property);
            }

            return false;
        }

        private static bool MethodCanBeIntercepted(MethodInfo method)
        {
            return method.IsVirtual;
        }

        private static bool PropertyCanBeIntercepted(PropertyInfo property)
        {
            return GetPropertyGetAndSetMethods(property).Any(x => MethodCanBeIntercepted(x));
        }

        private static IEnumerable<MethodInfo> GetPropertyGetAndSetMethods(PropertyInfo property)
        {
            var result = new List<MethodInfo>(2);

            var getMethod = property.GetGetMethod(true);
            if (getMethod != null)
            {
                result.Add(getMethod);
            }

            var setMethod = property.GetSetMethod(true);
            if (setMethod != null)
            {
                result.Add(setMethod);
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="result">The result of the generation if it was successful.</param>
        /// <param name="container">A fake object container the proxy generator can use to get arguments for constructor.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        private static bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IFakeObjectContainer container, out ProxyResult result)
        {
            var request = new ConstructorResolvingProxyGenerationRequest(container);
            return request.TryGenerateProxy(typeToProxy, fakeObject, out result);
        }

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="argumentsForConstructor">Arguments to use for the constructor of the proxied type.</param>
        /// <param name="result">The result of the generation if it was successful.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        private static bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor, out ProxyResult result)
        {
            if (typeToProxy.IsInterface)
            {
                throw new ArgumentException(ExceptionMessages.ArgumentsForConstructorOnInterfaceType);
            }

            result = GenerateClassProxy(typeToProxy, argumentsForConstructor, fakeObject);
            return true;
        }

        private static DynamicProxyResult GenerateClassProxy(Type typeToProxy, IEnumerable<object> argumentsForConstructor, FakeObject fakeObject)
        {
            var result = new DynamicProxyResult(typeToProxy);
            result.Proxy = (IFakedProxy)proxyGenerator.CreateClassProxy(typeToProxy, interfacesToImplement, new ProxyGenerationOptions(), argumentsForConstructor.ToArray(), CreateFakeObjectInterceptor(fakeObject), result);
            return result;
        }

        private static FakeObjectInterceptor CreateFakeObjectInterceptor(FakeObject fakeObject)
        {
            return new FakeObjectInterceptor { FakeObject = fakeObject };
        }

        /// <summary>
        /// Handles the resolving of constructor to use and generating a proxy
        /// of the specified type.
        /// </summary>
        private class ConstructorResolvingProxyGenerationRequest
        {
            private IFakeObjectContainer container;
            private HashSet<ConstructorInfo> constructorsCurrentlyBeingResolved;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructorResolvingProxyGenerationRequest"/> class.
            /// </summary>
            /// <param name="container">The container.</param>
            public ConstructorResolvingProxyGenerationRequest(IFakeObjectContainer container)
            {
                this.container = container;
                this.constructorsCurrentlyBeingResolved = new HashSet<ConstructorInfo>();
            }

            /// <summary>
            /// Tries to generate a proxy for the specified type. If the type is an interface
            /// a proxy is generated and returned. If the type is a class the request will try
            /// to find the best suited constructor to use and resolve arguments to use with this constructor.
            /// </summary>
            /// <param name="typeToProxy">The type to proxy.</param>
            /// <param name="fakeObject">The fake object.</param>
            /// <param name="result">The result.</param>
            /// <returns>A value indicating if the proxy was generated or not.</returns>
            public bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, out ProxyResult result)
            {
                if (!TypeCanBeProxied(typeToProxy))
                {
                    result = null;
                    return false;
                }

                if (typeToProxy.IsInterface)
                {
                    result = GenerateInterfaceProxy(typeToProxy, fakeObject);
                    return true;
                }

                return this.TryResolveConstructorAndGenerateProxy(typeToProxy, fakeObject, out result);
            }

            private bool TryResolveConstructorAndGenerateProxy(Type typeToProxy, FakeObject fakeObject, out ProxyResult result)
            {
                foreach (var constructor in GetUsableConstructors(typeToProxy))
                {
                    IEnumerable<object> resolvedArguments;

                    if (TryResolveConstructorArguments(constructor, out resolvedArguments))
                    {
                        try
                        {
                            result = GenerateClassProxy(typeToProxy, resolvedArguments, fakeObject);
                            return true;
                        }
                        catch (TargetInvocationException) { }
                    }
                }

                result = null;
                return false;
            }

            private bool TryResolveConstructorArguments(ConstructorInfo constructor, out IEnumerable<object> arguments)
            {
                if (!this.constructorsCurrentlyBeingResolved.Contains(constructor))
                {
                    this.constructorsCurrentlyBeingResolved.Add(constructor);

                    var resolvedArguments = new List<object>();
                    var argumentTypes = constructor.GetParameters().Select(x => x.ParameterType);
                    if (ResolveArgumentsFromTypes(argumentTypes, resolvedArguments, container))
                    {
                        this.constructorsCurrentlyBeingResolved.Remove(constructor);
                        arguments = resolvedArguments;
                        return true;
                    }
                }

                arguments = null;
                return false;
            }

            private static IEnumerable<ConstructorInfo> GetUsableConstructors(Type type)
            {
                return
                    from constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    where !constructor.IsPrivate
                    orderby constructor.GetParameters().Length descending 
                    select constructor;
            }

            private bool ResolveArgumentsFromTypes(IEnumerable<Type> argumentTypes, ICollection<object> arguments, IFakeObjectContainer container)
            {
                foreach (var argumentType in argumentTypes)
                {
                    object resolvedArgument = null;
                    if (container.TryCreateFakeObject(argumentType, out resolvedArgument))
                    {
                        arguments.Add(resolvedArgument);
                    }
                    else if (TryCreateValueTypeArgument(argumentType, out resolvedArgument))
                    {
                        arguments.Add(resolvedArgument);
                    }
                    else if (TryCreateProxiedArgument(argumentType, out resolvedArgument))
                    {
                        arguments.Add(resolvedArgument);
                    }
                    else if (TryCreateSealedTypeArgument(argumentType, out resolvedArgument))
                    {
                        arguments.Add(resolvedArgument);
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            
            private static DynamicProxyResult GenerateInterfaceProxy(Type typeToProxy, FakeObject fakeObject)
            {
                var result = new DynamicProxyResult(typeToProxy);
                result.Proxy = (IFakedProxy)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, interfacesToImplement, CreateFakeObjectInterceptor(fakeObject), result);
                return result;
            }

            private bool TryCreateProxiedArgument(Type type, out object argument)
            {
                ProxyResult result;
                if (this.TryGenerateProxy(type, null, out result))
                {
                    argument = result.Proxy;
                    return true;
                }

                argument = null;
                return false;
            }


            private static bool TryCreateValueTypeArgument(Type type, out object argument)
            {
                if (type.IsValueType)
                {
                    argument = Activator.CreateInstance(type, true);
                    return true;
                }

                argument = null;
                return false;
            }

            private static bool TryCreateSealedTypeArgument(Type type, out object argument)
            {
                var constructor = type.GetConstructor(new Type[] { });

                if (constructor == null)
                {
                    argument = null;
                    return false;
                }

                argument = Activator.CreateInstance(type, true);
                return true;
            }

            private static bool TypeCanBeProxied(Type type)
            {
                return !type.IsSealed;
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

    /// <summary>
    /// An implementation of the IProxyGenerator interface that uses DynamicProxy2 to
    /// generate proxies.
    /// </summary>
    internal class DynamicProxyProxyGeneratorNew
         : IProxyGeneratorNew
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static Type[] interfacesToImplement = new Type[] { typeof(IFakedProxy), typeof(ICanInterceptObjectMembers) };
        private IFakeObjectContainer container;

        public DynamicProxyProxyGeneratorNew(IFakeObjectContainer container)
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

        private static DynamicProxyResult CreateInterfaceProxy(Type typeToProxy, FakeObjectInterceptor fakeObjectInterceptor)
        {
            var proxyResult = new DynamicProxyResult(typeToProxy);
            proxyResult.Proxy = (IFakedProxy)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, interfacesToImplement, fakeObjectInterceptor, proxyResult);
            return proxyResult;
        }

        private ProxyResult DoGenerateProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor)
        {
            var fakeObjectInterceptor = CreateFakeObjectInterceptor(fakeObject);

            if (typeToProxy.IsInterface)
            {
                return CreateInterfaceProxy(typeToProxy, fakeObjectInterceptor);
            }
            
            return this.CreateClassProxy(typeToProxy, fakeObjectInterceptor, argumentsForConstructor);
        }

        private DynamicProxyResult CreateClassProxy(Type typeToProxy, FakeObjectInterceptor fakeObjectInterceptor, IEnumerable<object> argumentsForConstructor)
        {
            var candidateConstructorArguments = this.CreateConstructorArgumentCandidateSets(typeToProxy, argumentsForConstructor);

            DynamicProxyResult result = null;
            if (!this.TryGenerateProxyUsingCandidateConstructorArguments(typeToProxy, fakeObjectInterceptor, candidateConstructorArguments, out result))
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
                        interfacesToImplement,
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
                    dummyValue = DynamicProxyProxyGeneratorNew.proxyGenerator.CreateInterfaceProxyWithoutTarget(typeOfValue);
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