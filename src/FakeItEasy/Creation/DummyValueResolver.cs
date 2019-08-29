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
                    new ResolveByActivatingValueTypeStrategy(),
                    new ResolveByCreatingFakeStrategy(fakeObjectCreator),
                    new ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy()
                };
        }

        public CreationResult TryResolveDummyValue(Type typeOfDummy, LoopDetectingResolutionContext resolutionContext)
        {
            if (this.strategyCache.TryGetValue(typeOfDummy, out ResolveStrategy cachedStrategy))
            {
                return cachedStrategy.TryCreateDummyValue(typeOfDummy, this, resolutionContext);
            }

            if (!resolutionContext.TryBeginToResolve(typeOfDummy))
            {
                return CreationResult.FailedToCreateDummy(typeOfDummy, "Recursive dependency detected. Already resolving " + typeOfDummy + '.');
            }

            var creationResult = this.TryResolveDummyValueWithAllAvailableStrategies(typeOfDummy, resolutionContext);
            if (creationResult.WasSuccessful)
            {
                resolutionContext.OnSuccessfulResolve(typeOfDummy);
            }

            return creationResult;
        }

        private CreationResult TryResolveDummyValueWithAllAvailableStrategies(
            Type typeOfDummy,
            LoopDetectingResolutionContext resolutionContext)
        {
            CreationResult creationResult = null;
            foreach (var strategy in this.strategies)
            {
                var thisCreationResult = strategy.TryCreateDummyValue(typeOfDummy, this, resolutionContext);
                if (thisCreationResult.WasSuccessful)
                {
                    this.strategyCache.TryAdd(typeOfDummy, strategy);
                    return thisCreationResult;
                }

                creationResult = CreationResult.MergeIntoDummyResult(creationResult, thisCreationResult);
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
                    object taskResult = creationResult.WasSuccessful
                        ? creationResult.Result
                        : typeOfTaskResult.GetDefaultValue();

                    var method = GenericFromResultMethodDefinition.MakeGenericMethod(typeOfTaskResult);
                    return CreationResult.SuccessfullyCreated(method.Invoke(null, new[] { taskResult }));
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
            public override CreationResult TryCreateDummyValue(
                Type typeOfDummy,
                IDummyValueResolver resolver,
                LoopDetectingResolutionContext resolutionContext)
            {
                if (typeOfDummy.GetTypeInfo().IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Lazy<>))
                {
                    var typeOfLazyResult = typeOfDummy.GetGenericArguments()[0];
                    var creationResult = resolver.TryResolveDummyValue(typeOfLazyResult, resolutionContext);
                    object lazyResult = creationResult.WasSuccessful
                        ? creationResult.Result
                        : typeOfLazyResult.GetDefaultValue();

                    var funcType = typeof(Func<>).MakeGenericType(typeOfLazyResult);

                    var method = CreateGenericFromResultMethodDefinition().MakeGenericMethod(typeOfLazyResult);
                    var func = method.Invoke(null, new[] { lazyResult });
                    var dummy = typeOfDummy.GetConstructor(new[] { funcType, typeof(bool) }).Invoke(new[] { func, true });
                    return CreationResult.SuccessfullyCreated(dummy);
                }

                return CreationResult.FailedToCreateDummy(typeOfDummy, "It is not a Lazy.");
            }

            private static MethodInfo CreateGenericFromResultMethodDefinition()
            {
                Expression<Action> templateExpression = () => CreateFunc<object>(null);
                var templateMethod = ((MethodCallExpression)templateExpression.Body).Method;
                return templateMethod.GetGenericMethodDefinition();
            }

            private static Func<T> CreateFunc<T>(T value)
            {
                return () => value;
            }
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
                        if (TryCreateDummyValueUsingConstructor(cachedConstructor, resolvedConstructor, out object result))
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

                        if (resolvedConstructor.WasSuccessfullyResolved && TryCreateDummyValueUsingConstructor(constructor, resolvedConstructor, out object result))
                        {
                            this.cachedConstructors[typeOfDummy] = constructor;
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

            private static bool TryCreateDummyValueUsingConstructor(ConstructorInfo constructor, ResolvedConstructor resolvedConstructor, out object result)
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
                var success = this.DummyFactory.TryCreateDummyObject(typeOfDummy, out object result);
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
