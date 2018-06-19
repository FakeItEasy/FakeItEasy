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

            ProxyGeneratorResult result;
            if (proxyOptions.ArgumentsForConstructor != null)
            {
                result = this.GenerateProxy(typeOfFake, proxyOptions, proxyOptions.ArgumentsForConstructor);

                if (!result.ProxyWasSuccessfullyGenerated)
                {
                    if (throwOnFailure)
                    {
                        this.thrower.ThrowFailedToGenerateProxyWithArgumentsForConstructor(typeOfFake, result.ReasonForFailure);
                    }

                    return null;
                }

                return result.GeneratedProxy;
            }

            result = this.TryCreateFakeWithDummyArgumentsForConstructor(typeOfFake, proxyOptions, session, resolver, throwOnFailure);

            return result?.GeneratedProxy;
        }

        private static IEnumerable<Type[]> GetUsableParameterTypeListsInOrder(Type type)
        {
            // Always try the parameterless constructor first, and indicate it by using a null list of parameter types, since
            // a null set of constructor arguments works for faking types with a parameterless constructor and is required
            // when faking an interface.
            yield return null;

            foreach (var parameterTypeList in type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(c => c.GetParameters())
                .Where(pa => pa.Length > 0)
                .OrderByDescending(pa => pa.Length)
                .Select(pa => pa.Select(p => p.ParameterType).ToArray()))
            {
                yield return parameterTypeList;
            }
        }

        private static ResolvedConstructor ResolveConstructorArguments(Type[] parameterTypes, DummyCreationSession session, IDummyValueResolver resolver)
        {
            if (parameterTypes == null)
            {
                return new ResolvedConstructor();
            }

            var resolvedArguments = new List<ResolvedArgument>();

            foreach (var parameterType in parameterTypes)
            {
                bool wasResolved;
                object result = null;

                try
                {
                    wasResolved = resolver.TryResolveDummyValue(session, parameterType, out result);
                }
                catch
                {
                    wasResolved = false;
                }

                var resolvedArgument = new ResolvedArgument
                                           {
                                               WasResolved = wasResolved,
                                               ResolvedValue = result,
                                               ArgumentType = parameterType
                                           };

                resolvedArguments.Add(resolvedArgument);
            }

            return new ResolvedConstructor
                       {
                           Arguments = resolvedArguments.ToArray()
                       };
        }

        private ProxyGeneratorResult TryCreateFakeWithDummyArgumentsForConstructor(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver, bool throwOnFailure)
        {
            // Save the constructors as we try them. Avoids eager evaluation and double evaluation
            // of constructors enumerable.
            var consideredConstructors = new List<ResolvedConstructor>();
            foreach (var parameterTypes in GetUsableParameterTypeListsInOrder(typeOfFake))
            {
                var constructor = ResolveConstructorArguments(parameterTypes, session, resolver);
                if (constructor.WasSuccessfullyResolved)
                {
                    var result = this.GenerateProxy(typeOfFake, proxyOptions, constructor.Arguments?.Select(x => x.ResolvedValue));

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
                this.thrower.ThrowFailedToGenerateProxyWithResolvedConstructors(typeOfFake, consideredConstructors);
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
