﻿namespace FakeItEasy.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class DummyValueCreationSession
        : IDummyValueCreationSession
    {
        private static readonly Logger Logger = Log.GetLogger<DummyValueCreationSession>();

        private readonly ResolveStrategy[] availableStrategies;
        private readonly HashSet<Type> isInProcessOfResolving;
        private readonly Dictionary<Type, ResolveStrategy> strategyToUseForType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyValueCreationSession"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="fakeObjectCreator">The fake object creator.</param>
        public DummyValueCreationSession(IFakeObjectContainer container, IFakeObjectCreator fakeObjectCreator)
        {
            this.isInProcessOfResolving = new HashSet<Type>();
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
                Logger.Debug("Using cached strategy {0} for type {1}.", cachedStrategy.GetType(), typeOfDummy);
                return cachedStrategy.TryCreateDummyValue(typeOfDummy, out result);
            }

            for (var i = 0; i < this.availableStrategies.Length; i++)
            {
                if (this.availableStrategies[i].TryCreateDummyValue(typeOfDummy, out result))
                {
                    Logger.Debug("Using strategy {0} for type {1}.", this.availableStrategies[i].GetType(), typeOfDummy);
                    this.strategyToUseForType.Add(typeOfDummy, this.availableStrategies[i]);
                    return true;
                }
            }

            this.strategyToUseForType.Add(typeOfDummy, new UnableToResolveStrategy());
            result = null;
            return false;
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

        private class ResolveByCreatingFakeStrategy
            : ResolveStrategy
        {
            public IFakeObjectCreator FakeCreator { get; set; }

            public DummyValueCreationSession Session { get; set; }

            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                return this.FakeCreator.TryCreateFakeObject(typeOfDummy, this.Session, out result);
            }
        }

        private class ResolveByInstantiatingClassUsingDummyValuesAsConstructorArgumentsStrategy
            : ResolveStrategy
        {
            public DummyValueCreationSession Session { get; set; }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Appropriate in try method.")]
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                if (typeof(Delegate).IsAssignableFrom(typeOfDummy))
                {
                    result = null;
                    return false;
                }

                foreach (var constructor in GetConstructorsInOrder(typeOfDummy))
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

        private class ResolveFromContainerSrategy
            : ResolveStrategy
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

        private class UnableToResolveStrategy
            : ResolveStrategy
        {
            public override bool TryCreateDummyValue(Type typeOfDummy, out object result)
            {
                result = null;
                return false;
            }
        }
    }
}