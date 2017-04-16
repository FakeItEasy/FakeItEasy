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

        private static ResolvedConstructor[] ResolveConstructors(Type typeOfFake, DummyCreationSession session, IDummyValueResolver resolver)
        {
            return (from constructor in GetUsableConstructorsInOrder(typeOfFake)
                    let constructorAndArguments = ResolveConstructorArguments(constructor, session, resolver)
                    select constructorAndArguments).ToArray();
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
                object result;

                var resolvedArgument = new ResolvedArgument
                                           {
                                               WasResolved = resolver.TryResolveDummyValue(session, argument.ParameterType, out result),
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

            foreach (var constructor in constructors.Where(x => x.WasSuccessfullyResolved))
            {
                var result = this.GenerateProxy(typeOfFake, proxyOptions, constructor.Arguments.Select(x => x.ResolvedValue));

                if (result.ProxyWasSuccessfullyGenerated)
                {
                    return result;
                }

                constructor.ReasonForFailure = result.ReasonForFailure;
            }

            if (throwOnFailure)
            {
                this.thrower.ThrowFailedToGenerateProxyWithResolvedConstructors(typeOfFake, failReasonForDefaultConstructor, constructors);
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
