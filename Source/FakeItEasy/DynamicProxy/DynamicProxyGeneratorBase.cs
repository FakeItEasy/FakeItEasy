namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Creation;

    internal abstract class DynamicProxyGeneratorBase
        : IProxyGenerator
    {
        private IFakeCreationSession session;

        protected DynamicProxyGeneratorBase(IFakeCreationSession session)
        {
            this.session = session;
        }

        protected interface IInterceptionCallback
        {
            void Invoke(IWritableFakeObjectCall interceptedCall);
        }

        protected virtual IEnumerable<Type> InterfacesThatAllProxiesShouldImplement
        {
            get
            {
                return new[] { typeof(IFakedProxy) };
            }
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
                var result = this.CreateProxyResult(typeToProxy);
                result.SetFailure("The type is sealed.");
                return result;
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

        protected abstract IFakedProxy GenerateInterfaceProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeManager, IInterceptionCallback interceptionCallback);

        private IEnumerable<Type> GetAllInterfacesToImplement(IEnumerable<Type> additionalInterfacesToImplement)
        {
            if (additionalInterfacesToImplement == null)
            {
                additionalInterfacesToImplement = Enumerable.Empty<Type>();
            }

            return this.InterfacesThatAllProxiesShouldImplement.Concat(additionalInterfacesToImplement);
        }

        protected abstract bool TryGenerateClassProxy(
            Type typeToProxy,
            IEnumerable<Type> additionalInterfacesToImplement,
            FakeManager fakeManager,
            IEnumerable<object> argumentsForConstructor,
            IInterceptionCallback interceptionCallback,
            out IFakedProxy proxy);

        private static void AssertThatArgumentsForConstructorAreNotSpecifiedForInterfaceType(Type typeToProxy, IEnumerable<object> argumentsForConstructor)
        {
            if (typeToProxy.IsInterface && argumentsForConstructor != null)
            {
                throw new ArgumentException(ExceptionMessages.ArgumentsForConstructorOnInterfaceType, "argumentsForConstructor");
            }
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

        private DynamicProxyResult CreateProxyResult(Type typeOfProxy)
        {
            return new DynamicProxyResult(typeOfProxy);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try-methods.")]
        private bool TryGenerateProxyUsingCandidateConstructorArguments(
            Type typeToProxy,
            IEnumerable<Type> additionalInterfacesToImplement,
            FakeManager fakeManager,
            IEnumerable<IEnumerable<object>> argumentsForConstructor,
            out DynamicProxyResult proxyResult)
        {
            var outputProxyResult = new DynamicProxyResult(typeToProxy);

            foreach (var argumentSet in argumentsForConstructor)
            {
                IFakedProxy proxy = null;
                if (this.TryGenerateClassProxy(typeToProxy, this.GetAllInterfacesToImplement(additionalInterfacesToImplement), fakeManager, argumentSet, outputProxyResult, out proxy))
                {
                    outputProxyResult.Proxy = proxy;
                    proxyResult = outputProxyResult;
                    return true;
                }
            }

            proxyResult = null;
            return false;
        }

        private ProxyResult DoGenerateProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeManager, IEnumerable<object> argumentsForConstructor)
        {
            if (typeToProxy.IsInterface)
            {
                return this.CreateInterfaceProxyResult(typeToProxy, additionalInterfacesToImplement, fakeManager);
            }

            return this.CreateClassProxy(typeToProxy, additionalInterfacesToImplement, fakeManager, argumentsForConstructor);
        }

        private DynamicProxyResult CreateInterfaceProxyResult(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeManager)
        {
            var result = new DynamicProxyResult(typeToProxy);

            result.Proxy = this.GenerateInterfaceProxy(typeToProxy, this.GetAllInterfacesToImplement(additionalInterfacesToImplement), fakeManager, result);
            result.ProxyWasSuccessfullyCreated = true;

            return result;
        }

        private DynamicProxyResult CreateClassProxy(Type typeToProxy, IEnumerable<Type> additionalInterfacesToImplement, FakeManager fakeManager, IEnumerable<object> argumentsForConstructor)
        {
            var candidateConstructorArguments = this.CreateConstructorArgumentCandidateSets(typeToProxy, argumentsForConstructor);

            DynamicProxyResult result = null;
            if (!this.TryGenerateProxyUsingCandidateConstructorArguments(typeToProxy, additionalInterfacesToImplement, fakeManager, candidateConstructorArguments, out result))
            {
                result = this.CreateFailedProxyResult(typeToProxy);
            }

            return result;
        }

        private DynamicProxyResult CreateFailedProxyResult(Type typeToProxy)
        {
            string errorMessage = this.CreateErrorMessageForFailedProxyResult(typeToProxy);
            var result = new DynamicProxyResult(typeToProxy);
            result.SetFailure(errorMessage);
            return result;
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
        private class DynamicProxyResult
            : ProxyResult, IInterceptionCallback
        {
            public DynamicProxyResult(Type typeOfProxy)
                : base(typeOfProxy)
            {
                this.ProxyWasSuccessfullyCreated = true;
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;

            public new IFakedProxy Proxy
            {
                get
                {
                    return base.Proxy;
                }

                set
                {
                    base.Proxy = value;
                }
            }

            public void SetFailure(string errorMessage)
            {
                this.ErrorMessage = errorMessage;
                this.ProxyWasSuccessfullyCreated = false;
            }

            public void Invoke(IWritableFakeObjectCall interceptedCall)
            {
                var handler = this.CallWasIntercepted;
                if (handler != null)
                {
                    this.CallWasIntercepted(this, new CallInterceptedEventArgs(interceptedCall));
                }
            }
        }
    }
}