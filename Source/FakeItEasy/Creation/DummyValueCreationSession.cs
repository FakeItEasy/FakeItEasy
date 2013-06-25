﻿namespace FakeItEasy.Creation
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
#if NET40
        private static readonly MethodInfo GenericFromResultMethodDefinition = CreateGenericFromResultMethodDefinition();
#endif
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
                    new ResolveFromContainerSrategy { Container = container }, 
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

#if NET40
            if (this.TryResolveDummyValueWithAllAvailableStrategiesAndTaskWrapper(typeOfDummy, out result))
#else
            if (this.TryResolveDummyValueWithAllAvailableStrategies(typeOfDummy, out result))
#endif
            {
                this.OnSuccessfulResolve(typeOfDummy);
                return true;
            }

            return false;
        }

#if NET40
        private static MethodInfo CreateGenericFromResultMethodDefinition()
        {
            Expression<Action> templateExpression = () => FromResult(new object());
            var templateMethod = (templateExpression.Body as MethodCallExpression).Method;
            return templateMethod.GetGenericMethodDefinition();
        }

        private static Task<T> FromResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }
#endif

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

#if NET40
        private bool TryResolveDummyValueWithAllAvailableStrategiesAndTaskWrapper(Type typeOfDummy, out object result)
        {
            result = default(object);

            if (typeOfDummy == typeof(Task))
            {
                var source = new TaskCompletionSource<object>();
                source.SetResult(default(object));
                result = source.Task;
                return true;
            }

            if (typeOfDummy.IsGenericType && typeOfDummy.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var typeOfTaskResult = typeOfDummy.GetGenericArguments()[0];
                object taskResult;
                if (!this.TryResolveDummyValueWithAllAvailableStrategies(typeOfTaskResult, out taskResult))
                {
                    return false;
                }

                var method = GenericFromResultMethodDefinition.MakeGenericMethod(typeOfTaskResult);
                result = method.Invoke(null, new[] { taskResult });
                return true;
            }

            return this.TryResolveDummyValueWithAllAvailableStrategies(typeOfDummy, out result);
        }
#endif

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

        private class ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy : ResolveStrategy
        {
            public DummyValueCreationSession Session { get; set; }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try method.")]
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = default(object);
                if (typeof(Delegate).IsAssignableFrom(typeOfDummy))
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

        private class ResolveFromContainerSrategy : ResolveStrategy
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