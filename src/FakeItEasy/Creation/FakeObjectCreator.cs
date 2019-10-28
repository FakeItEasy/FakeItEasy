namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using FakeItEasy.Creation.DelegateProxies;

    internal class FakeObjectCreator : IFakeObjectCreator, IMethodInterceptionValidator
    {
        private readonly DefaultCreationStrategy defaultCreationStrategy;
        private readonly DelegateCreationStrategy delegateCreationStrategy;

        public FakeObjectCreator(
            FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory,
            IMethodInterceptionValidator castleMethodInterceptionValidator,
            IMethodInterceptionValidator delegateMethodInterceptionValidator)
        {
            this.defaultCreationStrategy = new DefaultCreationStrategy(castleMethodInterceptionValidator, fakeCallProcessorProviderFactory);
            this.delegateCreationStrategy = new DelegateCreationStrategy(delegateMethodInterceptionValidator, fakeCallProcessorProviderFactory);
        }

        public CreationResult CreateFake(
            Type typeOfFake,
            IProxyOptions proxyOptions,
            IDummyValueResolver resolver,
            LoopDetectingResolutionContext resolutionContext)
        {
            if (!resolutionContext.TryBeginToResolve(typeOfFake))
            {
                return CreationResult.FailedToCreateFake(typeOfFake, "Recursive dependency detected. Already resolving " + typeOfFake + '.');
            }

            try
            {
                return this.CreateFakeWithoutLoopDetection(
                    typeOfFake,
                    proxyOptions,
                    resolver,
                    resolutionContext);
            }
            finally
            {
                resolutionContext.EndResolve(typeOfFake);
            }
        }

        public CreationResult CreateFakeWithoutLoopDetection(
            Type typeOfFake,
            IProxyOptions proxyOptions,
            IDummyValueResolver resolver,
            LoopDetectingResolutionContext resolutionContext)
        {
            if (typeOfFake.GetTypeInfo().IsInterface)
            {
                return this.defaultCreationStrategy.CreateFakeInterface(typeOfFake, proxyOptions);
            }

            return DelegateCreationStrategy.IsResponsibleForCreating(typeOfFake)
                ? this.delegateCreationStrategy.CreateFake(typeOfFake, proxyOptions)
                : this.defaultCreationStrategy.CreateFake(typeOfFake, proxyOptions, resolver, resolutionContext);
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Seems appropriate here.")]
        public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason) =>
            callTarget is object && DelegateCreationStrategy.IsResponsibleForCreating(callTarget.GetType())
                ? this.delegateCreationStrategy.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason)
                : this.defaultCreationStrategy.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason);

        private class DelegateCreationStrategy
        {
            private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
            private readonly IMethodInterceptionValidator methodInterceptionValidator;

            public DelegateCreationStrategy(
                IMethodInterceptionValidator methodInterceptionValidator,
                FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
            {
                this.methodInterceptionValidator = methodInterceptionValidator;
                this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
            }

            public static bool IsResponsibleForCreating(Type typeOfFake) => typeof(Delegate).IsAssignableFrom(typeOfFake);

            public CreationResult CreateFake(Type typeOfFake, IProxyOptions proxyOptions)
            {
                if (proxyOptions.Attributes.Any())
                {
                    return CreationResult.FailedToCreateFake(typeOfFake, "Faked delegates cannot have custom attributes applied to them.");
                }

                if (proxyOptions.ArgumentsForConstructor is object && proxyOptions.ArgumentsForConstructor.Any())
                {
                    return CreationResult.FailedToCreateFake(typeOfFake, "Faked delegates cannot be made using explicit constructor arguments.");
                }

                if (proxyOptions.AdditionalInterfacesToImplement.Any())
                {
                    return CreationResult.FailedToCreateFake(typeOfFake, "Faked delegates cannot be made to implement additional interfaces.");
                }

                var fakeCallProcessorProvider = this.fakeCallProcessorProviderFactory(typeOfFake, proxyOptions);
                var proxyGeneratorResult = DelegateProxyGenerator.GenerateProxy(typeOfFake, fakeCallProcessorProvider);

                return proxyGeneratorResult.ProxyWasSuccessfullyGenerated
                    ? CreationResult.SuccessfullyCreated(proxyGeneratorResult.GeneratedProxy)
                    : CreationResult.FailedToCreateFake(typeOfFake, proxyGeneratorResult.ReasonForFailure);
            }

            public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason) =>
                this.methodInterceptionValidator.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason);
        }

        private class DefaultCreationStrategy
        {
            private readonly FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory;
            private readonly IMethodInterceptionValidator methodInterceptionValidator;
            private readonly ConcurrentDictionary<Type, Type[]> parameterTypesCache;

            public DefaultCreationStrategy(
                IMethodInterceptionValidator methodInterceptionValidator,
                FakeCallProcessorProvider.Factory fakeCallProcessorProviderFactory)
            {
                this.methodInterceptionValidator = methodInterceptionValidator;
                this.fakeCallProcessorProviderFactory = fakeCallProcessorProviderFactory;
                this.parameterTypesCache = new ConcurrentDictionary<Type, Type[]>();
            }

            public CreationResult CreateFakeInterface(Type typeOfFake, IProxyOptions proxyOptions)
            {
                if (proxyOptions.ArgumentsForConstructor is object)
                {
                    throw new ArgumentException(DynamicProxyMessages.ArgumentsForConstructorOnInterfaceType);
                }

                var fakeCallProcessorProvider = this.fakeCallProcessorProviderFactory(typeOfFake, proxyOptions);
                var proxyGeneratorResult = CastleDynamicProxyGenerator.GenerateInterfaceProxy(
                    typeOfFake,
                    proxyOptions.AdditionalInterfacesToImplement,
                    proxyOptions.Attributes,
                    fakeCallProcessorProvider);
                return proxyGeneratorResult.ProxyWasSuccessfullyGenerated
                    ? CreationResult.SuccessfullyCreated(proxyGeneratorResult.GeneratedProxy)
                    : CreationResult.FailedToCreateFake(typeOfFake, proxyGeneratorResult.ReasonForFailure);
            }

            public CreationResult CreateFake(
                Type typeOfFake,
                IProxyOptions proxyOptions,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (!CastleDynamicProxyGenerator.CanGenerateProxy(typeOfFake, out string reasonCannotGenerate))
                {
                    return CreationResult.FailedToCreateFake(typeOfFake, reasonCannotGenerate);
                }

                if (proxyOptions.ArgumentsForConstructor is object)
                {
                    var proxyGeneratorResult = this.GenerateProxy(typeOfFake, proxyOptions, proxyOptions.ArgumentsForConstructor);

                    return proxyGeneratorResult.ProxyWasSuccessfullyGenerated
                        ? CreationResult.SuccessfullyCreated(proxyGeneratorResult.GeneratedProxy)
                        : CreationResult.FailedToCreateFake(typeOfFake, proxyGeneratorResult.ReasonForFailure);
                }

                return this.TryCreateFakeWithDummyArgumentsForConstructor(
                    typeOfFake,
                    proxyOptions,
                    resolver,
                    resolutionContext);
            }

            public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason) =>
                this.methodInterceptionValidator.MethodCanBeInterceptedOnInstance(method, callTarget, out failReason);

            private static IEnumerable<Type[]> GetUsableParameterTypeListsInOrder(Type type)
            {
                var allConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // Always try the parameterless constructor even if there are no constructors on the type. Some proxy generators
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

            private static IEnumerable<object?> GetArgumentsForConstructor(ResolvedConstructor constructor) =>
                constructor.Arguments.Select(x => x.ResolvedValue);

            private CreationResult TryCreateFakeWithDummyArgumentsForConstructor(
                Type typeOfFake,
                IProxyOptions proxyOptions,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                // Save the constructors as we try them. Avoids eager evaluation and double evaluation
                // of constructors enumerable.
                var consideredConstructors = new List<ResolvedConstructor>();

                if (this.parameterTypesCache.TryGetValue(typeOfFake, out Type[] cachedParameterTypes))
                {
                    var constructor = new ResolvedConstructor(cachedParameterTypes, resolver, resolutionContext);
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
                        var constructor = new ResolvedConstructor(parameterTypes, resolver, resolutionContext);
                        if (constructor.WasSuccessfullyResolved)
                        {
                            var argumentsForConstructor = GetArgumentsForConstructor(constructor);
                            var result = this.GenerateProxy(typeOfFake, proxyOptions, argumentsForConstructor);

                            if (result.ProxyWasSuccessfullyGenerated)
                            {
                                this.parameterTypesCache.TryAdd(typeOfFake, parameterTypes);
                                return CreationResult.SuccessfullyCreated(result.GeneratedProxy);
                            }

                            constructor.ReasonForFailure = result.ReasonForFailure;
                        }

                        consideredConstructors.Add(constructor);
                    }
                }

                return CreationResult.FailedToCreateFake(typeOfFake, consideredConstructors);
            }

            private ProxyGeneratorResult GenerateProxy(Type typeOfFake, IProxyOptions proxyOptions, IEnumerable<object?> argumentsForConstructor)
            {
                var fakeCallProcessorProvider = this.fakeCallProcessorProviderFactory(typeOfFake, proxyOptions);

                return CastleDynamicProxyGenerator.GenerateClassProxy(
                    typeOfFake,
                    proxyOptions.AdditionalInterfacesToImplement,
                    argumentsForConstructor,
                    proxyOptions.Attributes,
                    fakeCallProcessorProvider);
            }
        }
    }
}
