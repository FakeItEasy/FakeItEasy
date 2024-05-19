namespace FakeItEasy.Creation
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using FakeItEasy.Core;

    internal class DummyValueResolver : IDummyValueResolver
    {
        private readonly ResolveStrategy[] strategies;
        private readonly ConcurrentDictionary<Type, ResolveStrategy> strategyCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyValueResolver"/> class.
        /// </summary>
        /// <param name="dummyFactory">The dummy factory.</param>
        /// <param name="fakeObjectCreator">The fake object creator.</param>
        /// <param name="proxyOptionsFactory">The proxy options factory.</param>
        public DummyValueResolver(DynamicDummyFactory dummyFactory, IFakeObjectCreator fakeObjectCreator, IProxyOptionsFactory proxyOptionsFactory)
        {
            this.strategyCache = new ConcurrentDictionary<Type, ResolveStrategy>();
            this.strategies = new ResolveStrategy[]
                {
                    new ResolveVoidByReturningNullStrategy(),
                    new ResolveFromDummyFactoryStrategy(dummyFactory),
                    new ResolveByCreatingTaskStrategy(),
                    new ResolveByCreatingLazyStrategy(),
                    new ResolveByCreatingTupleStrategy(),
                    new ResolveByActivatingValueTypeStrategy(),
                    new ResolveByCreatingFakeStrategy(fakeObjectCreator, proxyOptionsFactory),
                    new ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy()
                };
        }

        public CreationResult TryResolveDummyValue(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext)
        {
            // Make sure we're not already resolving typeOfDummy. It may seem that we could skip this check when we have
            // a cached resolution strategy in strategyCache, but it's necessary in case multiple threads are involved and
            // typeOfDummy has a constructor that takes typeOfDummy as a parameter.
            // In that situation, perhaps this thread starts trying to resolve a Dummy using the constructor that takes a
            // typeOfDummy. Meanwhile another thread does the same thing, but gets more processing time and eventually fails to
            // use that constructor, but then it succeeds in making a typeOfDummy using a different constructor. Then the strategy
            // is cached. This thread sees the cached strategy, creates a typeOfDummy for the constructor parameter, and then
            // uses it to make the "outer" typeOfDummy. Then we'd have Dummies created via two different constructors, which
            // might have different behavior. This is essentially the problem that arose in issue 1639.
            if (!resolutionContext.TryBeginToResolve(typeOfDummy))
            {
                return FailedCreationResult.ForDummy(typeOfDummy, "Recursive dependency detected. Already resolving " + typeOfDummy + '.');
            }

            try
            {
                return this.strategyCache.TryGetValue(typeOfDummy, out ResolveStrategy? cachedStrategy)
                    ? cachedStrategy.TryCreateDummyValue(typeOfDummy, this, resolutionContext)
                    : this.TryResolveDummyValueWithAllAvailableStrategies(typeOfDummy, resolutionContext);
            }
            finally
            {
                resolutionContext.EndResolve(typeOfDummy);
            }
        }

        private CreationResult TryResolveDummyValueWithAllAvailableStrategies(
            Type typeOfDummy,
            LoopDetectingResolutionContext resolutionContext)
        {
            var failedCreationResults = new List<FailedCreationResult>();
            foreach (var strategy in this.strategies)
            {
                switch (strategy.TryCreateDummyValue(typeOfDummy, this, resolutionContext))
                {
                    case SuccessfulCreationResult successfulCreationResult:
                        this.strategyCache.TryAdd(typeOfDummy, strategy);
                        return successfulCreationResult;
                    case FailedCreationResult failedCreationResult:
                        if (failedCreationResult.IsBlocking)
                        {
                            this.strategyCache.TryAdd(typeOfDummy, new UnableToResolveStrategy(failedCreationResult));
                            return failedCreationResult;
                        }

                        failedCreationResults.Add(failedCreationResult);
                        break;
                }
            }

            var overallFailedCreationResult = FailedCreationResult.Merge(failedCreationResults);
            this.strategyCache.TryAdd(typeOfDummy, new UnableToResolveStrategy(overallFailedCreationResult));
            return overallFailedCreationResult;
        }

        private class ResolveByActivatingValueTypeStrategy : ResolveStrategy
        {
            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (typeOfDummy.IsValueType)
                {
#if !TRY_ISBYREF_CREATION
                    if (typeOfDummy.IsByRefLike)
                    {
                        return FailedCreationResult.BlockingForDummy(typeOfDummy, "It is byref-like.");
                    }
#endif
                    return new SuccessfulCreationResult(Activator.CreateInstance(typeOfDummy));
                }

                return FailedCreationResult.ForDummy(typeOfDummy, "It is not a value type.");
            }
        }

        private class ResolveByCreatingFakeStrategy : ResolveStrategy
        {
            public ResolveByCreatingFakeStrategy(IFakeObjectCreator fakeCreator, IProxyOptionsFactory proxyOptionsFactory)
            {
                this.FakeCreator = fakeCreator;
                this.ProxyOptionsFactory = proxyOptionsFactory;
            }

            private IFakeObjectCreator FakeCreator { get; }

            private IProxyOptionsFactory ProxyOptionsFactory { get; }

            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                var proxyOptions = this.ProxyOptionsFactory.BuildProxyOptions(typeOfDummy, null);
                return this.FakeCreator.CreateFakeWithoutLoopDetection(
                    typeOfDummy,
                    proxyOptions,
                    resolver,
                    resolutionContext);
            }
        }

        private class ResolveByCreatingTaskStrategy : ResolveStrategy
        {
            private static readonly MethodInfo GenericFromResultMethodDefinition = CreateGenericFromResultMethodDefinition();

            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (typeOfDummy == typeof(Task))
                {
                    return new SuccessfulCreationResult(TaskHelper.CompletedTask);
                }

                if (typeOfDummy.IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var typeOfTaskResult = typeOfDummy.GetGenericArguments()[0];
                    var creationResult = resolver.TryResolveDummyValue(typeOfTaskResult, resolutionContext);
                    object? taskResult = creationResult.WasSuccessful
                        ? creationResult.Result
                        : typeOfTaskResult.GetDefaultValue();

                    var method = GenericFromResultMethodDefinition.MakeGenericMethod(typeOfTaskResult);
                    return new SuccessfulCreationResult(method.Invoke(null, new[] { taskResult }));
                }

                if (typeOfDummy.FullName == "System.Threading.Tasks.ValueTask")
                {
                    return new SuccessfulCreationResult(typeOfDummy.GetDefaultValue());
                }

                if (typeOfDummy.IsGenericType &&
                    !typeOfDummy.IsGenericTypeDefinition &&
                    typeOfDummy.FullName is string fullName &&
                    fullName.StartsWith("System.Threading.Tasks.ValueTask`", StringComparison.Ordinal))
                {
                    var typeOfTaskResult = typeOfDummy.GetGenericArguments()[0];
                    var creationResult = resolver.TryResolveDummyValue(typeOfTaskResult, resolutionContext);
                    object? taskResult = creationResult.WasSuccessful
                        ? creationResult.Result
                        : typeOfTaskResult.GetDefaultValue();

                    var ctor = typeOfDummy.GetConstructor(new[] { typeOfTaskResult })!;
                    return new SuccessfulCreationResult(ctor.Invoke(new[] { taskResult }));
                }

                return FailedCreationResult.ForDummy(typeOfDummy, "It is not a Task.");
            }

            private static MethodInfo CreateGenericFromResultMethodDefinition()
            {
                Expression<Action> templateExpression = () => TaskHelper.FromResult(new object());
                var templateMethod = ((MethodCallExpression)templateExpression.Body).Method;
                return templateMethod.GetGenericMethodDefinition();
            }
        }

        private class ResolveByCreatingLazyStrategy : ResolveStrategy
        {
            private static readonly MethodInfo CreateLazyDummyGenericDefinition =
                typeof(ResolveByCreatingLazyStrategy).GetMethod(
                    nameof(CreateLazyDummy),
                    BindingFlags.Static | BindingFlags.NonPublic)!;

            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (typeOfDummy.IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Lazy<>))
                {
                    var typeOfLazyResult = typeOfDummy.GetGenericArguments()[0];
                    var method = CreateLazyDummyGenericDefinition.MakeGenericMethod(typeOfLazyResult);
                    var dummy = method.Invoke(null, new object[] { resolver });
                    return new SuccessfulCreationResult(dummy);
                }

                return FailedCreationResult.ForDummy(typeOfDummy, "It is not a Lazy.");
            }

            private static Lazy<T> CreateLazyDummy<T>(IDummyValueResolver resolver)
            {
                return new Lazy<T>(() =>
                {
                    var creationResult = resolver.TryResolveDummyValue(typeof(T), new LoopDetectingResolutionContext());
                    return creationResult.WasSuccessful
                        ? (T)creationResult.Result!
                        : default!;
                });
            }
        }

        private class ResolveByCreatingTupleStrategy : ResolveStrategy
        {
            public override CreationResult TryCreateDummyValue(Type typeOfDummy, IDummyValueResolver resolver, LoopDetectingResolutionContext resolutionContext)
            {
                if (IsTuple(typeOfDummy))
                {
                    var argTypes = typeOfDummy.GetGenericArguments();
                    var args = new object?[argTypes.Length];
                    for (int i = 0; i < argTypes.Length; i++)
                    {
                        var argType = argTypes[i];
                        var creationResult = resolver.TryResolveDummyValue(argType, resolutionContext);
                        args[i] = creationResult.WasSuccessful
                            ? creationResult.Result
                            : argType.GetDefaultValue();
                    }

                    var dummy = Activator.CreateInstance(typeOfDummy, args);
                    return new SuccessfulCreationResult(dummy);
                }

                return FailedCreationResult.ForDummy(typeOfDummy, "It is not a tuple.");
            }

            private static bool IsTuple(Type type) =>
                type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.FullName is string fullName
                && (fullName.StartsWith("System.Tuple`", StringComparison.Ordinal) ||
                    fullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal));
        }

        private class ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy : ResolveStrategy
        {
            private readonly ConcurrentDictionary<Type, ConstructorInfo> cachedConstructors = new ConcurrentDictionary<Type, ConstructorInfo>();

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try method.")]
            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (typeof(Delegate).IsAssignableFrom(typeOfDummy))
                {
                    return FailedCreationResult.ForDummy(typeOfDummy, "It is a Delegate.");
                }

                if (typeOfDummy.IsAbstract)
                {
                    return FailedCreationResult.ForDummy(typeOfDummy, "It is abstract.");
                }

                // Save the constructors as we try them. Avoids eager evaluation and double evaluation
                // of constructors enumerable.
                var consideredConstructors = new List<ResolvedConstructor>();

                if (this.cachedConstructors.TryGetValue(typeOfDummy, out ConstructorInfo? cachedConstructor))
                {
                    var resolvedConstructor = new ResolvedConstructor(
                        cachedConstructor.GetParameters().Select(pi => pi.ParameterType),
                        resolver,
                        resolutionContext);
                    if (resolvedConstructor.WasSuccessfullyResolved)
                    {
                        if (TryCreateDummyValueUsingConstructor(cachedConstructor, resolvedConstructor, out object? result))
                        {
                            return new SuccessfulCreationResult(result);
                        }

                        consideredConstructors.Add(resolvedConstructor);
                    }
                }
                else
                {
                    foreach (var constructor in GetConstructorsInOrder(typeOfDummy))
                    {
                        var resolvedConstructor = new ResolvedConstructor(
                            constructor.GetParameters().Select(pi => pi.ParameterType),
                            resolver,
                            resolutionContext);

                        if (resolvedConstructor.WasSuccessfullyResolved &&
                            TryCreateDummyValueUsingConstructor(constructor, resolvedConstructor, out object? result))
                        {
                            this.cachedConstructors.TryAdd(typeOfDummy, constructor);
                            return new SuccessfulCreationResult(result);
                        }

                        consideredConstructors.Add(resolvedConstructor);
                    }
                }

                if (consideredConstructors.Count == 0)
                {
                    return FailedCreationResult.ForDummy(typeOfDummy, "It has no public constructors.");
                }

                return FailedCreationResult.ForDummy(typeOfDummy, consideredConstructors);
            }

            private static IEnumerable<ConstructorInfo> GetConstructorsInOrder(Type type)
            {
                return type.GetConstructors().OrderBy(x => x.GetParameters().Length).Reverse();
            }

            private static bool TryCreateDummyValueUsingConstructor(ConstructorInfo constructor, ResolvedConstructor resolvedConstructor, out object? result)
            {
                try
                {
                    result = constructor.Invoke(resolvedConstructor.Arguments.Select(a => a.ResolvedValue).ToArray());
                    return true;
                }
                catch (TargetInvocationException e)
                {
                    result = default;
                    if (e.InnerException?.Message is string message)
                    {
                        resolvedConstructor.ReasonForFailure = message;
                    }

                    return false;
                }
            }
        }

        private class ResolveFromDummyFactoryStrategy : ResolveStrategy
        {
            public ResolveFromDummyFactoryStrategy(DynamicDummyFactory dummyFactory)
            {
                this.DummyFactory = dummyFactory;
            }

            private DynamicDummyFactory DummyFactory { get; }

            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                var success = this.DummyFactory.TryCreateDummyObject(typeOfDummy, out object? result);
                return success
                    ? new SuccessfulCreationResult(result)
                    : FailedCreationResult.ForDummy(typeOfDummy, "No Dummy Factory produced a result.");
            }
        }

        private class ResolveVoidByReturningNullStrategy : ResolveStrategy
        {
            public override CreationResult TryCreateDummyValue(Type typeOfDummy, IDummyValueResolver resolver, LoopDetectingResolutionContext resolutionContext)
            {
                if (typeOfDummy == typeof(void))
                {
                    return new SuccessfulCreationResult(null);
                }

                return FailedCreationResult.ForDummy(typeOfDummy, "It is not void.");
            }
        }

        private abstract class ResolveStrategy
        {
            public abstract CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext);
        }

        private class UnableToResolveStrategy : ResolveStrategy
        {
            private readonly CreationResult creationResult;

            public UnableToResolveStrategy(CreationResult creationResult)
            {
                this.creationResult = creationResult;
            }

            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                return this.creationResult;
            }
        }
    }
}
