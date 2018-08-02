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
        private readonly ICreationStrategy[] strategies;

        public FakeObjectCreator(
            FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory,
            IProxyGenerator castleDynamicProxyGenerator,
            IProxyGenerator delegateProxyGenerator)
        {
            this.strategies = new ICreationStrategy[]
            {
                new DelegateCreationStrategy(delegateProxyGenerator, fakeCallProcessorProviderFactory),
                new DefaultCreationStrategy(castleDynamicProxyGenerator, fakeCallProcessorProviderFactory),
            };
        }

        /// <summary>
        /// Has the ability to create fakes of one or more types. The contract (which may need to be adjusted if additional strategies are
        /// added) is that clients will check <see cref="IsResponsibleForCreating"/> before attempting <see cref="CreateFake"/>.
        /// It is assumed that one strategy is a default or "catch-all" strategy that will attempt to create fakes of any types
        /// that the other strategies pass on.
        /// </summary>
        private interface ICreationStrategy
        {
            /// <summary>
            /// Indicates whether this strategy should be used to attempt to create a fake <paramref name="typeOfFake"/>.
            /// Success does not indicate that <see cref="CreateFake"/> will pass, but does mean that this is the best (only) strategy
            /// that's suitable to make the attempt.
            /// </summary>
            /// <param name="typeOfFake">The type that might be faked.</param>
            /// <returns><c>true</c> if this strategy can be used to create fake <paramref name="typeOfFake"/>s.</returns>
            bool IsResponsibleForCreating(Type typeOfFake);

            /// <summary>
            /// Creates a fake <paramref name="typeOfFake"/>.
            /// </summary>
            /// <param name="typeOfFake">The type of fake to create.</param>
            /// <param name="proxyOptions">Customizations to be applied to the fake.</param>
            /// <param name="session">Records all dummies being created by the current call chain.</param>
            /// <param name="resolver">A source of dummy values, should any be needed.</param>
            /// <returns>A creation result indicating success (and including the fake value) or failure.</returns>
            CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver);
        }

        public CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver) =>
            this.strategies.First(s => s.IsResponsibleForCreating(typeOfFake)).CreateFake(typeOfFake, proxyOptions, session, resolver);

        private class DelegateCreationStrategy : ICreationStrategy
        {
            private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
            private readonly IProxyGenerator proxyGenerator;

            public DelegateCreationStrategy(IProxyGenerator proxyGenerator, FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
            {
                this.proxyGenerator = proxyGenerator;
                this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
            }

            public bool IsResponsibleForCreating(Type typeOfFake) => this.proxyGenerator.CanGenerateProxy(typeOfFake, out _);

            public CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions, DummyCreationSession session, IDummyValueResolver resolver)
            {
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

        private class DefaultCreationStrategy : ICreationStrategy
        {
            private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
            private readonly IProxyGenerator proxyGenerator;
            private readonly ConcurrentDictionary<Type, Type[]> parameterTypesCache;

            public DefaultCreationStrategy(IProxyGenerator proxyGenerator, FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
            {
                this.proxyGenerator = proxyGenerator;
                this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
                this.parameterTypesCache = new ConcurrentDictionary<Type, Type[]>();
            }

            public bool IsResponsibleForCreating(Type typeOfFake) => true;

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
