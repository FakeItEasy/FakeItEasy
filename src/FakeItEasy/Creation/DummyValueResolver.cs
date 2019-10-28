namespace FakeItEasy.Creation
{
    using System;
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
        public DummyValueResolver(DynamicDummyFactory dummyFactory, IFakeObjectCreator fakeObjectCreator)
        {
            this.strategyCache = new ConcurrentDictionary<Type, ResolveStrategy>();
            this.strategies = new ResolveStrategy[]
                {
                    new ResolveFromDummyFactoryStrategy(dummyFactory),
                    new ResolveByCreatingTaskStrategy(),
                    new ResolveByCreatingLazyStrategy(),
                    new ResolveByCreatingTupleStrategy(),
                    new ResolveByActivatingValueTypeStrategy(),
                    new ResolveByCreatingFakeStrategy(fakeObjectCreator),
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
                return CreationResult.FailedToCreateDummy(typeOfDummy, "Recursive dependency detected. Already resolving " + typeOfDummy + '.');
            }

            try
            {
                return this.strategyCache.TryGetValue(typeOfDummy, out ResolveStrategy cachedStrategy)
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
            CreationResult creationResult = CreationResult.Untried;
            foreach (var strategy in this.strategies)
            {
                var thisCreationResult = strategy.TryCreateDummyValue(typeOfDummy, this, resolutionContext);
                if (thisCreationResult.WasSuccessful)
                {
                    this.strategyCache.TryAdd(typeOfDummy, strategy);
                    return thisCreationResult;
                }

                creationResult = creationResult.MergeIntoDummyResult(thisCreationResult);
            }

            this.strategyCache.TryAdd(typeOfDummy, new UnableToResolveStrategy(creationResult));
            return creationResult;
        }

        private class ResolveByActivatingValueTypeStrategy : ResolveStrategy
        {
            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (typeOfDummy.GetTypeInfo().IsValueType && typeOfDummy != typeof(void))
                {
                    return CreationResult.SuccessfullyCreated(Activator.CreateInstance(typeOfDummy));
                }

                return CreationResult.FailedToCreateDummy(typeOfDummy, "It is not a value type.");
            }
        }

        private class ResolveByCreatingFakeStrategy : ResolveStrategy
        {
            public ResolveByCreatingFakeStrategy(IFakeObjectCreator fakeCreator)
            {
                this.FakeCreator = fakeCreator;
            }

            private IFakeObjectCreator FakeCreator { get; }

            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                return this.FakeCreator.CreateFakeWithoutLoopDetection(
                    typeOfDummy,
                    new ProxyOptions(),
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
                    return CreationResult.SuccessfullyCreated(TaskHelper.FromResult(default(object)));
                }

                if (typeOfDummy.GetTypeInfo().IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var typeOfTaskResult = typeOfDummy.GetGenericArguments()[0];
                    var creationResult = resolver.TryResolveDummyValue(typeOfTaskResult, resolutionContext);
                    object? taskResult = creationResult.WasSuccessful
                        ? creationResult.Result
                        : typeOfTaskResult.GetDefaultValue();

                    var method = GenericFromResultMethodDefinition.MakeGenericMethod(typeOfTaskResult);
                    return CreationResult.SuccessfullyCreated(method.Invoke(null, new[] { taskResult }));
                }

                if (typeOfDummy.FullName == "System.Threading.Tasks.ValueTask")
                {
                    return CreationResult.SuccessfullyCreated(typeOfDummy.GetDefaultValue());
                }

                if (typeOfDummy.GetTypeInfo().IsGenericType &&
                    !typeOfDummy.GetTypeInfo().IsGenericTypeDefinition &&
                    typeOfDummy.FullName.StartsWith("System.Threading.Tasks.ValueTask`", StringComparison.Ordinal))
                {
                    var typeOfTaskResult = typeOfDummy.GetGenericArguments()[0];
                    var creationResult = resolver.TryResolveDummyValue(typeOfTaskResult, resolutionContext);
                    object? taskResult = creationResult.WasSuccessful
                        ? creationResult.Result
                        : typeOfTaskResult.GetDefaultValue();

                    var ctor = typeOfDummy.GetConstructor(new[] { typeOfTaskResult });
                    return CreationResult.SuccessfullyCreated(ctor.Invoke(new[] { taskResult }));
                }

                return CreationResult.FailedToCreateDummy(typeOfDummy, "It is not a Task.");
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
                    BindingFlags.Static | BindingFlags.NonPublic);

            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (typeOfDummy.GetTypeInfo().IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Lazy<>))
                {
                    var typeOfLazyResult = typeOfDummy.GetGenericArguments()[0];
                    var method = CreateLazyDummyGenericDefinition.MakeGenericMethod(typeOfLazyResult);
                    var dummy = method.Invoke(null, new object[] { resolver });
                    return CreationResult.SuccessfullyCreated(dummy);
                }

                return CreationResult.FailedToCreateDummy(typeOfDummy, "It is not a Lazy.");
            }

            private static Lazy<T> CreateLazyDummy<T>(IDummyValueResolver resolver)
            {
                return new Lazy<T>(() =>
                {
                    var creationResult = resolver.TryResolveDummyValue(typeof(T), new LoopDetectingResolutionContext());
                    return creationResult.WasSuccessful
                        ? (T)creationResult.Result!
                        : default;
                });
            }
        }

        private class ResolveByCreatingTupleStrategy : ResolveStrategy
        {
            public override CreationResult TryCreateDummyValue(Type typeOfDummy, IDummyValueResolver resolver, LoopDetectingResolutionContext resolutionContext)
            {
                if (IsTuple(typeOfDummy))
                {
                    var argTypes = typeOfDummy.GetTypeInfo().GetGenericArguments();
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
                    return CreationResult.SuccessfullyCreated(dummy);
                }

                return CreationResult.FailedToCreateDummy(typeOfDummy, "It is not a tuple.");
            }

            private static bool IsTuple(Type type) =>
                type.GetTypeInfo().IsGenericType
                && !type.GetTypeInfo().IsGenericTypeDefinition
                && (type.FullName.StartsWith("System.Tuple`", StringComparison.Ordinal) ||
                    type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal));
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
                    return CreationResult.FailedToCreateDummy(typeOfDummy, "It is a Delegate.");
                }

                if (typeOfDummy.GetTypeInfo().IsAbstract)
                {
                    return CreationResult.FailedToCreateDummy(typeOfDummy, "It is abstract.");
                }

                // Save the constructors as we try them. Avoids eager evaluation and double evaluation
                // of constructors enumerable.
                var consideredConstructors = new List<ResolvedConstructor>();

                if (this.cachedConstructors.TryGetValue(typeOfDummy, out ConstructorInfo cachedConstructor))
                {
                    var resolvedConstructor = new ResolvedConstructor(
                        cachedConstructor.GetParameters().Select(pi => pi.ParameterType),
                        resolver,
                        resolutionContext);
                    if (resolvedConstructor.WasSuccessfullyResolved)
                    {
                        if (TryCreateDummyValueUsingConstructor(cachedConstructor, resolvedConstructor, out object? result))
                        {
                            return CreationResult.SuccessfullyCreated(result);
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
                            return CreationResult.SuccessfullyCreated(result);
                        }

                        consideredConstructors.Add(resolvedConstructor);
                    }
                }

                if (consideredConstructors.Any())
                {
                    return CreationResult.FailedToCreateDummy(typeOfDummy, consideredConstructors);
                }

                return CreationResult.FailedToCreateDummy(typeOfDummy, "It has no public constructors.");
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
                    resolvedConstructor.ReasonForFailure = e.InnerException.Message;
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
                    ? CreationResult.SuccessfullyCreated(result)
                    : CreationResult.FailedToCreateDummy(typeOfDummy, "No Dummy Factory produced a result.");
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
