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
        private readonly ICreationStrategy delegateCreationStrategy;
        private readonly ICreationStrategy defaultCreationStrategy;

        public FakeObjectCreator(
            FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory,
            IProxyGenerator castleDynamicProxyGenerator,
            IProxyGenerator delegateProxyGenerator)
        {
            this.delegateCreationStrategy = new DelegateCreationStrategy(delegateProxyGenerator, fakeCallProcessorProviderFactory);
            this.defaultCreationStrategy = new CastleDynamicProxyCreationStrategy(castleDynamicProxyGenerator, fakeCallProcessorProviderFactory);
        }

        private interface ICreationStrategy
        {
            CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver);
        }

        public CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver)
        {
            var result = this.delegateCreationStrategy.CreateFake(typeOfFake, proxyOptions, session, resolver);
            if (result.WasSuccessful)
            {
                return result;
            }

            return this.defaultCreationStrategy.CreateFake(typeOfFake, proxyOptions, session, resolver);
        }

        private class DelegateCreationStrategy : ICreationStrategy
        {
            private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
            private readonly IProxyGenerator proxyGenerator;

            public DelegateCreationStrategy(IProxyGenerator proxyGenerator, FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
            {
                this.proxyGenerator = proxyGenerator;
                this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
            }

            public CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver)
            {
                if (!this.proxyGenerator.CanGenerateProxy(typeOfFake, out string reasonCannotGenerate))
                {
                    return CreationResult.FailedToCreateFake(typeOfFake, reasonCannotGenerate);
                }

                var fakeCallProcessorProvider = this.fakeCallProcessorProviderFactory(typeOfFake, proxyOptions);
                var proxyGeneratorResult = this.proxyGenerator.GenerateProxy(
                    typeOfFake,
                    proxyOptions.AdditionalInterfacesToImplement,
                    argumentsForConstructor: null,
                    proxyOptions.Attributes,
                    fakeCallProcessorProvider);

                return proxyGeneratorResult.ProxyWasSuccessfullyGenerated
                    ? CreationResult.SuccessfullyCreated(proxyGeneratorResult.GeneratedProxy)
                    : CreationResult.FailedToCreateFake(typeOfFake, proxyGeneratorResult.ReasonForFailure);
            }
        }

        private class CastleDynamicProxyCreationStrategy : ICreationStrategy
        {
            private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
            private readonly IProxyGenerator proxyGenerator;
            private readonly ConcurrentDictionary<Type, Type[]> parameterTypesCache;

            public CastleDynamicProxyCreationStrategy(IProxyGenerator proxyGenerator, FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
            {
                this.proxyGenerator = proxyGenerator;
                this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
                this.parameterTypesCache = new ConcurrentDictionary<Type, Type[]>();
            }

            public CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver)
            {
                if (!this.proxyGenerator.CanGenerateProxy(typeOfFake, out string reasonCannotGenerate))
                {
                    return CreationResult.FailedToCreateFake(typeOfFake, reasonCannotGenerate);
                }

                if (proxyOptions.ArgumentsForConstructor != null)
                {
                    var proxyGeneratorResult = this.GenerateProxy(typeOfFake, proxyOptions, proxyOptions.ArgumentsForConstructor);

                    return proxyGeneratorResult.ProxyWasSuccessfullyGenerated
                        ? CreationResult.SuccessfullyCreated(proxyGeneratorResult.GeneratedProxy)
                        : CreationResult.FailedToCreateFake(typeOfFake, proxyGeneratorResult.ReasonForFailure);
                }

                return this.TryCreateFakeWithDummyArgumentsForConstructor(typeOfFake, proxyOptions, session, resolver);
            }

            private static IEnumerable<Type[]> GetUsableParameterTypeListsInOrder(Type type)
            {
                var allConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // Always try the parameterless constructor even if there are no constuctors on the type. Some proxy generators
                // can proxy types without constructors, such as interfaces.
                if (!allConstructors.Any())
                {
                    yield return Type.EmptyTypes;
                    yield break;
                }

                // Offer up all the constructors as possibilities.
                // The 0-length constructor has always been tried first, which is advantageous because it's very easy to
                // resolve the arguments. On the other hand, it can result in a less-configurable, Fake with more concrete
                // (non-Faked) collaborators.
                foreach (var parameterTypeList in allConstructors
                    .Select(c => c.GetParameters())
                    .OrderByDescending(pa => pa.Length == 0 ? int.MaxValue : pa.Length)
                    .Select(pa => pa.Select(p => p.ParameterType).ToArray()))
                {
                    yield return parameterTypeList;
                }
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
                    var constructor = new ResolvedConstructor(cachedParameterTypes, session, resolver);
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
                        var constructor = new ResolvedConstructor(parameterTypes, session, resolver);
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

                return CreationResult.FailedToCreateFake(typeOfFake, consideredConstructors);
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
}
