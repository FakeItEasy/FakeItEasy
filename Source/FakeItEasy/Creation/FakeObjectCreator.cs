namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class FakeObjectCreator
    {
        private static readonly Logger logger = Log.GetLogger<FakeObjectCreator>();
        private IProxyGenerator proxyGenerator;
        private IExceptionThrower thrower;
        private IFakeManagerAccessor fakeManagerAttacher;
        private IFakeObjectConfigurer configurer;

        public FakeObjectCreator(IProxyGenerator proxyGenerator, IExceptionThrower thrower, IFakeManagerAccessor fakeManagerAttacher, IFakeObjectConfigurer configurer)
        {
            this.proxyGenerator = proxyGenerator;
            this.thrower = thrower;
            this.fakeManagerAttacher = fakeManagerAttacher;
            this.configurer = configurer;
        }

        public object CreateFake(Type typeOfFake, FakeOptions fakeOptions, IDummyValueCreationSession session, bool throwOnFailure)
        {
            var result = this.proxyGenerator.GenerateProxy(typeOfFake, fakeOptions.AdditionalInterfacesToImplement, fakeOptions.ArgumentsForConstructor);

            if (throwOnFailure)
            {
                AssertThatProxyWasGeneratedWhenArgumentsForConstructorAreSpecified(typeOfFake, result, fakeOptions);
            }

            if (!result.ProxyWasSuccessfullyGenerated && fakeOptions.ArgumentsForConstructor == null)
            {
                result = this.TryCreateFakeWithDummyArgumentsForConstructor(typeOfFake, fakeOptions, session, result.ReasonForFailure, throwOnFailure);
            }

            if (result != null)
            {
                this.fakeManagerAttacher.AttachFakeManagerToProxy(typeOfFake, result.GeneratedProxy, result.CallInterceptedEventRaiser);
                this.configurer.ConfigureFake(typeOfFake, result.GeneratedProxy);
                return result.GeneratedProxy;
            }

            return null;
        }

        private void AssertThatProxyWasGeneratedWhenArgumentsForConstructorAreSpecified(Type typeOfFake, ProxyGeneratorResult result, FakeOptions fakeOptions)
        {
            if (!result.ProxyWasSuccessfullyGenerated && fakeOptions.ArgumentsForConstructor != null)
            {
                this.thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeOfFake, result.ReasonForFailure);
            }
        }

        private ProxyGeneratorResult TryCreateFakeWithDummyArgumentsForConstructor(Type typeOfFake, FakeOptions fakeOptions, IDummyValueCreationSession session, string failReasonForDefaultConstructor, bool throwOnFailure)
        {
            var constructors = this.ResolveConstructors(typeOfFake, session);
                
            foreach (var constructor in constructors.Where(x => x.WasSuccessfullyResolved))
            {
                logger.Debug("Trying with constructor with {0} arguments.", constructor.Arguments.Length);

                var result = this.proxyGenerator.GenerateProxy(typeOfFake, fakeOptions.AdditionalInterfacesToImplement, constructor.Arguments.Select(x => x.ResolvedValue));

                if (result.ProxyWasSuccessfullyGenerated)
                {
                    return result;
                }
                else
                {
                    logger.Debug("Setting reason for failure of constructor to {0}.", result.ReasonForFailure);
                    constructor.ReasonForFailure = result.ReasonForFailure;
                }
            }

            if (throwOnFailure)
            {
                this.thrower.ThrowFailedToGenerateProxyWithResolvedConstructors(typeOfFake, failReasonForDefaultConstructor, constructors);
            }

            return null;
        }

        private ResolvedConstructor[] ResolveConstructors(Type typeOfFake, IDummyValueCreationSession session)
        {
            logger.Debug("Resolving constructors for type {0}.", typeOfFake);

            return (from constructor in GetUsableConstructorsInOrder(typeOfFake)
                    let constructorAndArguments = ResolveConstructorArguments(constructor, session)
                    select constructorAndArguments).ToArray();
        }

        private static IEnumerable<ConstructorInfo> GetUsableConstructorsInOrder(Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetParameters().Length > 0)
                .OrderByDescending(x => x.GetParameters().Length);
        }

        private static ResolvedConstructor ResolveConstructorArguments(ConstructorInfo constructor, IDummyValueCreationSession session)
        {
            logger.Debug("Beginning to resolve constructor with {0} arguments.", constructor.GetParameters().Length);

            var resolvedArguments = new List<ResolvedArgument>();

            foreach (var argument in constructor.GetParameters())
            {
                object result = null;

                var resolvedArgument = new ResolvedArgument
                {
                    WasResolved = session.TryResolveDummyValue(argument.ParameterType, out result),
                    ResolvedValue = result,
                    ArgumentType = argument.ParameterType
                };

                logger.Debug("Was able to resolve {0}: {1}.", argument.ParameterType, resolvedArgument.WasResolved);
                resolvedArguments.Add(resolvedArgument);
            }

            return new ResolvedConstructor
            {
                Arguments = resolvedArguments.ToArray()
            };
        }
    }
}