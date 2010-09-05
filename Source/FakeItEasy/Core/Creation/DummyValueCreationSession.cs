namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    internal class DummyValueCreationSession
        : IDummyValueCreationSession
    {
        private IFakeObjectContainer container;
        private IFakeObjectCreator fakeObjectCreator;
        private HashSet<Type> isInProcessOfResolving;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyValueCreationSession"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="fakeObjectCreator">The fake object creator.</param>
        public DummyValueCreationSession(IFakeObjectContainer container, IFakeObjectCreator fakeObjectCreator)
        {
            this.isInProcessOfResolving = new HashSet<Type>();
            this.container = container;
            this.fakeObjectCreator = fakeObjectCreator;
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
            if (this.container.TryCreateFakeObject(typeOfDummy, out result))
            {
                return true;
            }

            if (this.fakeObjectCreator.TryCreateFakeObject(typeOfDummy, this, out result))
            {
                return true;
            }

            if (this.TryActivateValueType(typeOfDummy, out result))
            {
                return true;
            }

            if (this.TryInstantiateClassUsingDummyValuesForConstructorArguments(typeOfDummy, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        private bool TryActivateValueType(Type typeOfDummy, out object result)
        {
            if (typeOfDummy.IsValueType)
            {
                result = Activator.CreateInstance(typeOfDummy);
                return true;
            }

            result = null;
            return false;
        }

        private bool TryInstantiateClassUsingDummyValuesForConstructorArguments(Type typeOfDummy, out object result)
        {
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

                if (!this.TryResolveDummyValue(type, out resolvedType))
                {
                    return null;
                }

                result.Add(resolvedType);
            }

            return result;
        }
    }
}