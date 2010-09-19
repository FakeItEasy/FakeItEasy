namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    internal class DummyValueCreationSession
        : IDummyValueCreationSession
    {
        private static readonly Logger logger = Log.GetLogger<DummyValueCreationSession>();

        private IFakeObjectCreator fakeObjectCreator;
        private HashSet<Type> isInProcessOfResolving;
        private ResolveStrategy[] availableStrategies;
        private Dictionary<Type, ResolveStrategy> strategyToUseForType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyValueCreationSession"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="fakeObjectCreator">The fake object creator.</param>
        public DummyValueCreationSession(IFakeObjectContainer container, IFakeObjectCreator fakeObjectCreator)
        {
            this.isInProcessOfResolving = new HashSet<Type>();
            this.fakeObjectCreator = fakeObjectCreator;
            this.strategyToUseForType = new Dictionary<Type, ResolveStrategy>();

            this.availableStrategies = new ResolveStrategy[] 
            {
                new ResolveFromContainerSrategy { Container = container },
                new ResolveByCreatingFakeStrategy { FakeCreator = fakeObjectCreator, Session = this },
                new ResolveByActivatingValueTypeStrategy(),
                new ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy { Session = this }
            };
        }

        public bool TryResolveDummyValue(Type typeOfDummy, out object result)
        {
            if (!this.EnsureThatResolvedTypeIsNotRecursive(typeOfDummy))
            {
                result = null;
                return false;
            }

            if (this.TryResolveDummyValueWithAllAvailableStrategies(typeOfDummy, out result))
            {
                this.OnSuccessfulResolve(typeOfDummy);
                return true;
            }

            result = null;
            return false;
        }

        private bool EnsureThatResolvedTypeIsNotRecursive(Type typeOfDummy)
        {
            if (this.isInProcessOfResolving.Contains(typeOfDummy))
            {
                return false;
            }

            this.isInProcessOfResolving.Add(typeOfDummy);
            return true;
        }

        private void OnSuccessfulResolve(Type typeOfDummy)
        {
            this.isInProcessOfResolving.Remove(typeOfDummy);
        }

        private bool TryResolveDummyValueWithAllAvailableStrategies(Type typeOfDummy, out object result)
        {
            ResolveStrategy cachedStrategy;
            if (this.strategyToUseForType.TryGetValue(typeOfDummy, out cachedStrategy))
            {
                logger.Debug("Using cached strategy {0} for type {1}.", cachedStrategy.GetType(), typeOfDummy);
                return cachedStrategy.TryCreateDummyValue(typeOfDummy, out result);
            }

            for (int i = 0; i < this.availableStrategies.Length; i++)
            {
                if (this.availableStrategies[i].TryCreateDummyValue(typeOfDummy, out result))
                {
                    logger.Debug("Using strategy {0} for type {1}.", this.availableStrategies[i].GetType(), typeOfDummy);
                    this.strategyToUseForType.Add(typeOfDummy, this.availableStrategies[i]);
                    return true;
                }
            }

            this.strategyToUseForType.Add(typeOfDummy, new UnableToResolveStrategy());
            result = null;
            return false;
        }

        #region Strategies
        private abstract class ResolveStrategy
        {
            public abstract bool TryCreateDummyValue(Type typeOfDummy, out object result);
        }

        private class ResolveFromContainerSrategy
            : ResolveStrategy
        {
            public IFakeObjectContainer Container;

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                return this.Container.TryCreateFakeObject(typeOfDummy, out result);
            }
        }

        private class ResolveByCreatingFakeStrategy
            : ResolveStrategy
        {
            public IFakeObjectCreator FakeCreator;
            public DummyValueCreationSession Session;

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                return this.FakeCreator.TryCreateFakeObject(typeOfDummy, this.Session, out result);
            }
        }

        private class ResolveByActivatingValueTypeStrategy
            : ResolveStrategy
        {
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                if (typeOfDummy.IsValueType && !typeOfDummy.Equals(typeof(void)))
                {
                    result = Activator.CreateInstance(typeOfDummy);
                    return true;
                }

                result = null;
                return false;
            }
        }

        private class ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy
            : ResolveStrategy
        {
            public DummyValueCreationSession Session;

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                if (typeof(Delegate).IsAssignableFrom(typeOfDummy))
                {
                    result = null;
                    return false;
                }

                foreach (var constructor in this.GetConstructorsInOrder(typeOfDummy))
                {
                    var argumentTypes = constructor.GetParameters().Select(x => x.ParameterType);
                    var resolvedArguments = this.ResolveAllTypes(argumentTypes);

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

                result = null;
                return false;
            }

            private IEnumerable<ConstructorInfo> GetConstructorsInOrder(Type type)
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

        private class UnableToResolveStrategy
            : ResolveStrategy
        {
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = null;
                return false;
            }
        }
        #endregion
    }
}