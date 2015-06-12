namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
#if NET40
    using System.Linq.Expressions;
#endif
    using System.Reflection;
#if NET40
    using System.Threading.Tasks;
#endif
    using FakeItEasy.Core;

    internal class DummyValueCreationSession : IDummyValueCreationSession
    {
        private readonly ResolveStrategy[] strategies;
        private readonly HashSet<Type> typesCurrentlyBeingResolved;
        private readonly Dictionary<Type, ResolveStrategy> strategyCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyValueCreationSession"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="fakeObjectCreator">The fake object creator.</param>
        public DummyValueCreationSession(IFakeObjectContainer container, IFakeObjectCreator fakeObjectCreator)
        {
            this.typesCurrentlyBeingResolved = new HashSet<Type>();
            this.strategyCache = new Dictionary<Type, ResolveStrategy>();
            this.strategies = new ResolveStrategy[]
                {
                    new ResolveFromContainerStrategy { Container = container }, 
#if NET40
                    new ResolveByCreatingTaskStrategy { Session = this }, 
                    new ResolveByCreatingLazyStrategy { Session = this },
#endif
                    new ResolveByCreatingFakeStrategy { FakeCreator = fakeObjectCreator, Session = this }, 
                    new ResolveByActivatingValueTypeStrategy(), 
                    new ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy { Session = this }
                };
        }

        public bool TryResolveDummyValue(Type typeOfDummy, out object result)
        {
            result = default(object);
            if (!this.EnsureThatResolvedTypeIsNotRecursive(typeOfDummy))
            {
                return false;
            }

            if (this.TryResolveDummyValueWithAllAvailableStrategies(typeOfDummy, out result))
            {
                this.OnSuccessfulResolve(typeOfDummy);
                return true;
            }

            return false;
        }

        private bool EnsureThatResolvedTypeIsNotRecursive(Type typeOfDummy)
        {
            if (this.typesCurrentlyBeingResolved.Contains(typeOfDummy))
            {
                return false;
            }

            this.typesCurrentlyBeingResolved.Add(typeOfDummy);
            return true;
        }

        private void OnSuccessfulResolve(Type typeOfDummy)
        {
            this.typesCurrentlyBeingResolved.Remove(typeOfDummy);
        }

        private bool TryResolveDummyValueWithAllAvailableStrategies(Type typeOfDummy, out object result)
        {
            result = default(object);

            ResolveStrategy cachedStrategy;
            if (this.strategyCache.TryGetValue(typeOfDummy, out cachedStrategy))
            {
                return cachedStrategy.TryCreateDummyValue(typeOfDummy, out result);
            }

            foreach (var strategy in this.strategies)
            {
                if (strategy.TryCreateDummyValue(typeOfDummy, out result))
                {
                    this.strategyCache.Add(typeOfDummy, strategy);
                    return true;
                }
            }

            this.strategyCache.Add(typeOfDummy, new UnableToResolveStrategy());
            return false;
        }

        private class ResolveByActivatingValueTypeStrategy : ResolveStrategy
        {
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = default(object);

                if (typeOfDummy.IsValueType && !typeOfDummy.Equals(typeof(void)))
                {
                    result = Activator.CreateInstance(typeOfDummy);
                    return true;
                }

                return false;
            }
        }

        private class ResolveByCreatingFakeStrategy : ResolveStrategy
        {
            public IFakeObjectCreator FakeCreator { get; set; }

            public DummyValueCreationSession Session { get; set; }

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                return this.FakeCreator.TryCreateFakeObject(typeOfDummy, this.Session, out result);
            }
        }

#if NET40
        private class ResolveByCreatingTaskStrategy : ResolveStrategy
        {
            private static readonly MethodInfo GenericFromResultMethodDefinition = CreateGenericFromResultMethodDefinition();

            public DummyValueCreationSession Session { get; set; }

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = default(object);

                if (typeOfDummy == typeof(Task))
                {
                    result = TaskHelper.FromResult(default(object));
                    return true;
                }

                if (typeOfDummy.IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var typeOfTaskResult = typeOfDummy.GetGenericArguments()[0];
                    object taskResult;
                    if (!this.Session.TryResolveDummyValue(typeOfTaskResult, out taskResult))
                    {
                        return false;
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
            public DummyValueCreationSession Session { get; set; }

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = default(object);

                if (typeOfDummy.IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Lazy<>))
                {
                    var typeOfLazyResult = typeOfDummy.GetGenericArguments()[0];
                    object lazyResult;
                    if (!this.Session.TryResolveDummyValue(typeOfLazyResult, out lazyResult))
                    {
                        return false;
                    }
                    
                    var funcType = typeof(Func<>).MakeGenericType(typeOfLazyResult);
        
                    var method = CreateGenericFromResultMethodDefinition().MakeGenericMethod(typeOfLazyResult);
                    var func = method.Invoke(null, new[] { lazyResult });
                    result = typeOfDummy.GetConstructor(new[] { funcType, typeof(bool) }).Invoke(new object[] { func, true });
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
#endif

        private class ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy : ResolveStrategy
        {
            public DummyValueCreationSession Session { get; set; }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try method.")]
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = default(object);
                if (typeof(Delegate).IsAssignableFrom(typeOfDummy) || typeOfDummy.IsAbstract)
                {
                    return false;
                }

                foreach (var constructor in GetConstructorsInOrder(typeOfDummy))
                {
                    var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType);
                    var resolvedArguments = this.ResolveAllTypes(parameterTypes);

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

            private IEnumerable<object> ResolveAllTypes(IEnumerable<Type> types)
            {
                var result = new List<object>();

                foreach (var type in types)
                {
                    object resolvedType = null;

                    if (!this.Session.TryResolveDummyValue(type, out resolvedType))
                    {
                        return null;
                    }

                    result.Add(resolvedType);
                }

                return result;
            }
        }

        private class ResolveFromContainerStrategy : ResolveStrategy
        {
            public IFakeObjectContainer Container { get; set; }

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                return this.Container.TryCreateDummyObject(typeOfDummy, out result);
            }
        }

        private abstract class ResolveStrategy
        {
            public abstract bool TryCreateDummyValue(Type typeOfDummy, out object result);
        }

        private class UnableToResolveStrategy : ResolveStrategy
        {
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = default(object);
                return false;
            }
        }
    }
}