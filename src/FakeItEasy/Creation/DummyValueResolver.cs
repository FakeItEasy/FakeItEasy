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
                    new ResolveByCreatingTaskStrategy(this),
                    new ResolveByCreatingLazyStrategy(this),
                    new ResolveByCreatingFakeStrategy(fakeObjectCreator, this),
                    new ResolveByActivatingValueTypeStrategy(),
                    new ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy(this)
                };
        }

        public bool TryResolveDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
        {
            result = default(object);
            if (!session.TryBeginToResolveType(typeOfDummy))
            {
                return false;
            }

            if (this.TryResolveDummyValueWithAllAvailableStrategies(session, typeOfDummy, out result))
            {
                session.OnSuccessfulResolve(typeOfDummy);
                return true;
            }

            return false;
        }

        private bool TryResolveDummyValueWithAllAvailableStrategies(DummyCreationSession session, Type typeOfDummy, out object result)
        {
            result = default(object);

            ResolveStrategy cachedStrategy;
            if (this.strategyCache.TryGetValue(typeOfDummy, out cachedStrategy))
            {
                return cachedStrategy.TryCreateDummyValue(session, typeOfDummy, out result);
            }

            foreach (var strategy in this.strategies)
            {
                if (strategy.TryCreateDummyValue(session, typeOfDummy, out result))
                {
                    this.strategyCache.TryAdd(typeOfDummy, strategy);
                    return true;
                }
            }

            this.strategyCache.TryAdd(typeOfDummy, new UnableToResolveStrategy());
            return false;
        }

        private class ResolveByActivatingValueTypeStrategy : ResolveStrategy
        {
            public override bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
            {
                result = default(object);

                if (typeOfDummy.GetTypeInfo().IsValueType && typeOfDummy != typeof(void))
                {
                    result = Activator.CreateInstance(typeOfDummy);
                    return true;
                }

                return false;
            }
        }

        private class ResolveByCreatingFakeStrategy : ResolveStrategy
        {
            public ResolveByCreatingFakeStrategy(IFakeObjectCreator fakeCreator, DummyValueResolver resolver)
            {
                this.FakeCreator = fakeCreator;
                this.Resolver = resolver;
            }

            private IFakeObjectCreator FakeCreator { get; }

            private DummyValueResolver Resolver { get; }

            public override bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
            {
                return this.FakeCreator.TryCreateFakeObject(session, typeOfDummy, this.Resolver, out result);
            }
        }

        private class ResolveByCreatingTaskStrategy : ResolveStrategy
        {
            private static readonly MethodInfo GenericFromResultMethodDefinition = CreateGenericFromResultMethodDefinition();

            public ResolveByCreatingTaskStrategy(DummyValueResolver resolver)
            {
                this.Resolver = resolver;
            }

            private DummyValueResolver Resolver { get; }

            public override bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
            {
                result = default(object);

                if (typeOfDummy == typeof(Task))
                {
                    result = TaskHelper.FromResult(default(object));
                    return true;
                }

                if (typeOfDummy.GetTypeInfo().IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var typeOfTaskResult = typeOfDummy.GetGenericArguments()[0];
                    object taskResult;
                    if (!this.Resolver.TryResolveDummyValue(session, typeOfTaskResult, out taskResult))
                    {
                        taskResult = typeOfTaskResult.GetDefaultValue();
                    }

                    var method = GenericFromResultMethodDefinition.MakeGenericMethod(typeOfTaskResult);
                    result = method.Invoke(null, new[] { taskResult });
                    return true;
                }

                return false;
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
            public ResolveByCreatingLazyStrategy(DummyValueResolver resolver)
            {
                this.Resolver = resolver;
            }

            private DummyValueResolver Resolver { get; }

            public override bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
            {
                result = default(object);

                if (typeOfDummy.GetTypeInfo().IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Lazy<>))
                {
                    var typeOfLazyResult = typeOfDummy.GetGenericArguments()[0];
                    object lazyResult;
                    if (!this.Resolver.TryResolveDummyValue(session, typeOfLazyResult, out lazyResult))
                    {
                        lazyResult = typeOfLazyResult.GetDefaultValue();
                    }

                    var funcType = typeof(Func<>).MakeGenericType(typeOfLazyResult);

                    var method = CreateGenericFromResultMethodDefinition().MakeGenericMethod(typeOfLazyResult);
                    var func = method.Invoke(null, new[] { lazyResult });
                    result = typeOfDummy.GetConstructor(new[] { funcType, typeof(bool) }).Invoke(new[] { func, true });
                    return true;
                }

                return false;
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
            public ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy(DummyValueResolver resolver)
            {
                this.Resolver = resolver;
            }

            private DummyValueResolver Resolver { get; }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try method.")]
            public override bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
            {
                result = default(object);
                if (typeof(Delegate).IsAssignableFrom(typeOfDummy) || typeOfDummy.GetTypeInfo().IsAbstract)
                {
                    return false;
                }

                foreach (var constructor in GetConstructorsInOrder(typeOfDummy))
                {
                    var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType);
                    var resolvedArguments = this.ResolveAllTypes(session, parameterTypes);

                    if (resolvedArguments != null)
                    {
                        try
                        {
                            result = Activator.CreateInstance(typeOfDummy, resolvedArguments.ToArray());
                            return true;
                        }
                        catch
                        {
                        }
                    }
                }

                return false;
            }

            private static IEnumerable<ConstructorInfo> GetConstructorsInOrder(Type type)
            {
                return type.GetConstructors().OrderBy(x => x.GetParameters().Length).Reverse();
            }

            private IEnumerable<object> ResolveAllTypes(DummyCreationSession session, IEnumerable<Type> types)
            {
                var result = new List<object>();

                foreach (var type in types)
                {
                    object resolvedType;

                    if (!this.Resolver.TryResolveDummyValue(session, type, out resolvedType))
                    {
                        return null;
                    }

                    result.Add(resolvedType);
                }

                return result;
            }
        }

        private class ResolveFromDummyFactoryStrategy : ResolveStrategy
        {
            public ResolveFromDummyFactoryStrategy(DynamicDummyFactory dummyFactory)
            {
                this.DummyFactory = dummyFactory;
            }

            private DynamicDummyFactory DummyFactory { get; }

            public override bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
            {
                return this.DummyFactory.TryCreateDummyObject(typeOfDummy, out result);
            }
        }

        private abstract class ResolveStrategy
        {
            public abstract bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result);
        }

        private class UnableToResolveStrategy : ResolveStrategy
        {
            public override bool TryCreateDummyValue(DummyCreationSession session, Type typeOfDummy, out object result)
            {
                result = default(object);
                return false;
            }
        }
    }
}
