namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class FakeObjectCreator
    {
        private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
        private readonly IProxyGenerator proxyGenerator;
        private readonly IExceptionThrower thrower;
        private readonly ConcurrentDictionary<Type, Type[]> parameterTypesCache;

        public FakeObjectCreator(IProxyGenerator proxyGenerator, IExceptionThrower thrower, FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
        {
            this.proxyGenerator = proxyGenerator;
            this.thrower = thrower;
            this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
            this.parameterTypesCache = new ConcurrentDictionary<Type, Type[]>();
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
            // Always try the parameterless constructor first, since it works for faking interfaces as well as classes.
            yield return Type.EmptyTypes;

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
            var resolvedArguments = new ResolvedArgument[parameterTypes.Length];

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                var parameterType = parameterTypes[i];
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

                resolvedArguments[i] = new ResolvedArgument
                {
                    WasResolved = wasResolved,
                    ResolvedValue = result,
                    ArgumentType = parameterType
                };
            }

            return new ResolvedConstructor(resolvedArguments);
        }

        private static IEnumerable<object> GetArgumentsForConstructor(ResolvedConstructor constructor)
        {
            // Interface proxy creation requires a null argumentsForConstructor, and null also works
            // for parameterless class constructors, so reduce an empty argument list to null.
            return constructor.Arguments.Any()
                ? constructor.Arguments.Select(x => x.ResolvedValue)
                : null;
        }

        private ProxyGeneratorResult TryCreateFakeWithDummyArgumentsForConstructor(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver, bool throwOnFailure)
        {
            // Save the constructors as we try them. Avoids eager evaluation and double evaluation
            // of constructors enumerable.
            var consideredConstructors = new List<ResolvedConstructor>();

            if (this.parameterTypesCache.TryGetValue(typeOfFake, out Type[] cachedParameterTypes))
            {
                var constructor = ResolveConstructorArguments(cachedParameterTypes, session, resolver);
                if (constructor.WasSuccessfullyResolved)
                {
                    var argumentsForConstructor = GetArgumentsForConstructor(constructor);
                    var result = this.GenerateProxy(typeOfFake, proxyOptions, argumentsForConstructor);

                    if (result.ProxyWasSuccessfullyGenerated)
                    {
                        return result;
                    }

                    constructor.ReasonForFailure = result.ReasonForFailure;
                }

                consideredConstructors.Add(constructor);
            }
            else
            {
                foreach (var parameterTypes in GetUsableParameterTypeListsInOrder(typeOfFake))
                {
                    var constructor = ResolveConstructorArguments(parameterTypes, session, resolver);
                    if (constructor.WasSuccessfullyResolved)
                    {
                        var argumentsForConstructor = GetArgumentsForConstructor(constructor);
                        var result = this.GenerateProxy(typeOfFake, proxyOptions, argumentsForConstructor);

                        if (result.ProxyWasSuccessfullyGenerated)
                        {
                            this.parameterTypesCache[typeOfFake] = parameterTypes;
                            return result;
                        }

                        constructor.ReasonForFailure = result.ReasonForFailure;
                    }

                    consideredConstructors.Add(constructor);
                }
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
