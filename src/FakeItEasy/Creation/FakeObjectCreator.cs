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
        private readonly ConcurrentDictionary<Type, Type[]> parameterTypesCache;

        public FakeObjectCreator(IProxyGenerator proxyGenerator, FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
        {
            this.proxyGenerator = proxyGenerator;
            this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
            this.parameterTypesCache = new ConcurrentDictionary<Type, Type[]>();
        }

        public CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver)
        {
            if (!this.proxyGenerator.CanGenerateProxy(typeOfFake, out string reasonCannotGenerate))
            {
                return CreationResult.FailedToCreate(typeOfFake, reasonCannotGenerate);
            }

            if (proxyOptions.ArgumentsForConstructor != null)
            {
                var proxyGeneratorResult = this.GenerateProxy(typeOfFake, proxyOptions, proxyOptions.ArgumentsForConstructor);

                if (!proxyGeneratorResult.ProxyWasSuccessfullyGenerated)
                {
                    return CreationResult.FailedToCreate(typeOfFake, proxyGeneratorResult.ReasonForFailure);
                }

                return CreationResult.SuccessfullyCreated(proxyGeneratorResult.GeneratedProxy);
            }

            return this.TryCreateFakeWithDummyArgumentsForConstructor(typeOfFake, proxyOptions, session, resolver);
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

                var resolvedArgument = new ResolvedArgument { ArgumentType = parameterType };
                try
                {
                    var creationResult = resolver.TryResolveDummyValue(session, parameterType);
                    resolvedArgument.WasResolved = creationResult.WasSuccessful;
                    if (creationResult.WasSuccessful)
                    {
                        resolvedArgument.ResolvedValue = creationResult.GetResultAsDummy();
                    }
                }
                catch
                {
                    resolvedArgument.WasResolved = false;
                }

                resolvedArguments[i] = resolvedArgument;
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

        private CreationResult TryCreateFakeWithDummyArgumentsForConstructor(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver)
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
                        return CreationResult.SuccessfullyCreated(result.GeneratedProxy);
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
                            return CreationResult.SuccessfullyCreated(result.GeneratedProxy);
                        }

                        constructor.ReasonForFailure = result.ReasonForFailure;
                    }

                    consideredConstructors.Add(constructor);
                }
            }

            return CreationResult.FailedToCreate(typeOfFake, consideredConstructors);
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
