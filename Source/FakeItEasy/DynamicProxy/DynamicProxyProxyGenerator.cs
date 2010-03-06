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

            if (!this.TryGenerateProxy(typeToProxy, fakeObject, argumentsForConstructor, out result))
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

            if (!this.TryGenerateProxy(typeToProxy, fakeObject, container, out result))
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
        /// <param name="container">A fake object container the proxy generator can use to get arguments for constructor.</param>
        /// <param name="generatedProxy">An object containing the proxy if generation was successful.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        private bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IFakeObjectContainer container, out ProxyResult result)
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
        /// <param name="generatedProxy">An object containing the proxy if generation was successful.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        private bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor, out ProxyResult result)
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

                return TryResolveConstructorAndGenerateProxy(typeToProxy, fakeObject, out result);
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
                        catch (TargetInvocationException)
                        {

                        }
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
                    else if (TryCreateProxiedArgument(argumentType, container, out resolvedArgument))
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

            private bool TryCreateProxiedArgument(Type type, IFakeObjectContainer container, out object argument)
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
}