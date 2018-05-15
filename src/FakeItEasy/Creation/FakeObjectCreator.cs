namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class FakeObjectCreator
    {
        private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
        private readonly IProxyGenerator proxyGenerator;
        private readonly IExceptionThrower thrower;

        public FakeObjectCreator(IProxyGenerator proxyGenerator, IExceptionThrower thrower, FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
        {
            this.proxyGenerator = proxyGenerator;
            this.thrower = thrower;
            this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
        }

        public object CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver, bool throwOnFailure)
        {
            if (!this.proxyGenerator.CanGenerateProxy(typeOfFake, out string reasonCannotGenerate))
            {
                if (throwOnFailure)
                {
                    this.thrower.ThrowFailedToGenerateProxyWithoutTryingConstructors(typeOfFake, reasonCannotGenerate);
                }

                return null;
            }

            var result = this.GenerateProxy(typeOfFake, proxyOptions, proxyOptions.ArgumentsForConstructor);

            if (throwOnFailure)
            {
                this.AssertThatProxyWasGeneratedWhenArgumentsForConstructorAreSpecified(typeOfFake, result, proxyOptions);
            }

            if (!result.ProxyWasSuccessfullyGenerated && proxyOptions.ArgumentsForConstructor == null)
            {
                result = this.TryCreateFakeWithDummyArgumentsForConstructor(typeOfFake, proxyOptions, session, resolver, result.ReasonForFailure, throwOnFailure);
            }

            return result != null ? result.GeneratedProxy : null;
        }

        private static IEnumerable<ResolvedConstructor> ResolveConstructors(Type typeOfFake, DummyCreationSession session, IDummyValueResolver resolver)
        {
            return GetUsableConstructorsInOrder(typeOfFake)
                .Select(constructor => ResolveConstructorArguments(constructor, session, resolver));
        }

        private static IEnumerable<ConstructorInfo> GetUsableConstructorsInOrder(Type type)
        {
            return type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetParameters().Length > 0)
                .OrderByDescending(x => x.GetParameters().Length);
        }

        private static ResolvedConstructor ResolveConstructorArguments(ConstructorInfo constructor, DummyCreationSession session, IDummyValueResolver resolver)
        {
            var resolvedArguments = new List<ResolvedArgument>();

            foreach (var argument in constructor.GetParameters())
            {
                bool wasResolved;
                object result = null;

                try
                {
                    wasResolved = resolver.TryResolveDummyValue(session, argument.ParameterType, out result);
                }
                catch
                {
                    wasResolved = false;
                }

                var resolvedArgument = new ResolvedArgument
                                           {
                                               WasResolved = wasResolved,
                                               ResolvedValue = result,
                                               ArgumentType = argument.ParameterType
                                           };

                resolvedArguments.Add(resolvedArgument);
            }

            return new ResolvedConstructor
                       {
                           Arguments = resolvedArguments.ToArray()
                       };
        }

        private void AssertThatProxyWasGeneratedWhenArgumentsForConstructorAreSpecified(Type typeOfFake, ProxyGeneratorResult result, IProxyOptions proxyOptions)
        {
            if (!result.ProxyWasSuccessfullyGenerated && proxyOptions.ArgumentsForConstructor != null)
            {
                this.thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeOfFake, result.ReasonForFailure);
            }
        }

        private ProxyGeneratorResult TryCreateFakeWithDummyArgumentsForConstructor(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver, string failReasonForDefaultConstructor, bool throwOnFailure)
        {
            var constructors = ResolveConstructors(typeOfFake, session, resolver);

            // Save the constructors as we try them. Avoids eager evaluation and double evaluation
            // of constructors enumerable.
            var consideredConstructors = new List<ResolvedConstructor>();
            foreach (var constructor in constructors)
            {
                if (constructor.WasSuccessfullyResolved)
                {
                    var result = this.GenerateProxy(typeOfFake, proxyOptions, constructor.Arguments.Select(x => x.ResolvedValue));

                    if (result.ProxyWasSuccessfullyGenerated)
                    {
                        return result;
                    }

                    constructor.ReasonForFailure = result.ReasonForFailure;
                }

                consideredConstructors.Add(constructor);
            }

            if (throwOnFailure)
            {
                this.thrower.ThrowFailedToGenerateProxyWithResolvedConstructors(typeOfFake, failReasonForDefaultConstructor, consideredConstructors);
            }

            return null;
        }

        private ProxyGeneratorResult GenerateProxy(Type typeOfFake, IProxyOptions proxyOptions, IEnumerable<object> argumentsForConstructor)
        {
            var fakeCallProcessorProvider = this.fakeCallProcessorProviderFactory(typeOfFake, proxyOptions);

            return this.proxyGenerator.GenerateProxy(
                    typeOfFake,
                    proxyOptions.AdditionalInterfacesToImplement,
                    argumentsForConstructor,
                    proxyOptions.Attributes,
                    fakeCallProcessorProvider);
        }
    }
}
