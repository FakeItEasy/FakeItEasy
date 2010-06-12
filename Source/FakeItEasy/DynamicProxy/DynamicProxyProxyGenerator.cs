namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
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
        internal static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static Type[] interfacesToImplement = new Type[] { typeof(IFakedProxy), typeof(ICanInterceptObjectMembers) };
        private IFakeCreationSession session;
        
        public DynamicProxyProxyGenerator(IFakeCreationSession session)
        {
            this.session = session;
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
        public ProxyResult GenerateProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeObject, IEnumerable<object> argumentsForConstructor)
        {
            if (typeToProxy.IsSealed)
            {
                return new DynamicProxyResult(typeToProxy, "The type is sealed.");
            }
            
            AssertThatArgumentsForConstructorAreNotSpecifiedForInterfaceType(typeToProxy, argumentsForConstructor);

            return this.DoGenerateProxy(typeToProxy, additionalInterfacesToImplement, fakeObject, argumentsForConstructor);
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

        private static Type[] GetAllInterfacesToImplement(IEnumerable<Type> additionalInterfacesToImplement)
        {
            if (additionalInterfacesToImplement == null)
            {
                additionalInterfacesToImplement = Enumerable.Empty<Type>();
            }

            return interfacesToImplement.Concat(additionalInterfacesToImplement).ToArray();
        }

        private static FakeObjectInterceptor CreateFakeObjectInterceptor(FakeManager fakeManager)
        {
            return new FakeObjectInterceptor()
            {
                FakeManager = fakeManager
            };
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try-methods.")]
        private static bool TryGenerateProxyUsingCandidateConstructorArguments(
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

        private static void AppendDescriptionsForEachConstructor(StringBuilder message, IEnumerable<ConstructorAndArgumentsInfo> constructors)
        {
            foreach (var constructor in constructors)
            {
                AppendConstructorVisibility(message, constructor.Constructor);
                AppendConstructorArgumentsList(message, constructor);

                message.AppendLine(")");
            }
        }

        private static void AppendConstructorArgumentsList(StringBuilder message, ConstructorAndArgumentsInfo constructor)
        {
            bool firstArgument = true;

            foreach (var argument in constructor.Arguments)
            {
                if (!firstArgument)
                {
                    message.Append(", ");
                }
                else
                {
                    firstArgument = false;
                }

                AppendArgumentDescription(message, argument);
            }
        }

        private static void AppendArgumentDescription(StringBuilder message, ArgumentInfo argument)
        {
            if (!argument.WasSuccessfullyResolved)
            {
                message.Append("*");
            }

            message.Append(argument.TypeOfArgument);
        }

        private static void AppendConstructorVisibility(StringBuilder message, ConstructorInfo constructor)
        {
            if (constructor.IsPublic)
            {
                message.Append("public     (");
            }
            else if (constructor.IsAssembly)
            {
                message.Append("internal   (");
            }
            else if (constructor.IsFamily)
            {
                message.Append("protected  (");
            }
        }

        private ProxyResult DoGenerateProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeObject, IEnumerable<object> argumentsForConstructor)
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
            if (!TryGenerateProxyUsingCandidateConstructorArguments(typeToProxy, additionalInterfacesToImplement, fakeObjectInterceptor, candidateConstructorArguments, out result))
            {
                result = this.CreateFailedProxyResult(typeToProxy);
            }

            return result;
        }

        private DynamicProxyResult CreateFailedProxyResult(Type typeToProxy)
        {
            string errorMessage = this.CreateErrorMessageForFailedProxyResult(typeToProxy);
            return new DynamicProxyResult(typeToProxy, errorMessage);
        }

        private string CreateErrorMessageForFailedProxyResult(Type typeToProxy)
        {
            var message = new StringBuilder();

            message.AppendLine("The type has no default constructor and none of the available constructors listed below can be resolved:");
            message.AppendLine();

            AppendDescriptionsForEachConstructor(message, this.session.ConstructorResolver.ListAllConstructors(typeToProxy));

            message.AppendLine();
            message.AppendLine("* The types marked with with a star (*) can not be faked. Register these types in the current");
            message.Append("IFakeObjectContainer in order to generate a fake of this type.");

            return message.ToString();
        }

        private IEnumerable<IEnumerable<object>> CreateConstructorArgumentCandidateSets(Type typeToProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (argumentsForConstructor != null)
            {
                return new[] { argumentsForConstructor };
            }
            
            return this.ResolveArgumentSetsForConstructor(typeToProxy);
        }

        private IEnumerable<IEnumerable<object>> ResolveArgumentSetsForConstructor(Type typeToProxy)
        {
            return
                from constructor in this.session.ConstructorResolver.ListAllConstructors(typeToProxy)
                where constructor.Arguments.All(x => x.WasSuccessfullyResolved)
                orderby constructor.Constructor.GetParameters().Length descending
                select constructor.ArgumentsToUse;
        }

        [Serializable]
        private class FakeObjectInterceptor
            : IInterceptor
        {
            private static readonly MethodInfo getFakeManagerMethod = typeof(IFakedProxy).GetProperty("FakeManager").GetGetMethod();

            public FakeManager FakeManager { get; set; }

            [DebuggerStepThrough]
            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.Equals(getFakeManagerMethod))
                {
                    invocation.ReturnValue = this.FakeManager;
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
            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Virtual for testability only, should probably be fixed but is a breaking change.")]
            public DynamicProxyResult(Type typeOfProxy)
                : base(typeOfProxy)
            {
                this.ProxyWasSuccessfullyCreated = true;
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Virtual for testability only, should probably be fixed but is a breaking change.")]
            public DynamicProxyResult(Type typeToProxy, string errorMessage)
                : this(typeToProxy)
            {
                this.ProxyWasSuccessfullyCreated = false;
                this.ErrorMessage = errorMessage;
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;

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
        }
    }
}