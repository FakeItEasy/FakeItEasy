namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;
    using FakeItEasy.Core;

    /// <summary>
    /// The default implementation of the <see cref="IConstructorResolver" /> interface.
    /// </summary>
    internal class DefaultConstructorResolver
        : IConstructorResolver
    {
        private IFakeCreationSession session;

        public DefaultConstructorResolver(IFakeCreationSession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets all the accessible constructor for the type along with dummy arguments
        /// where they can be resolved.
        /// </summary>
        /// <param name="type">The type to list constructors for.</param>
        /// <returns>A collection of constructors.</returns>
        public IEnumerable<ConstructorAndArgumentsInfo> ListAllConstructors(Type type)
        {
            var constructors = this.GetConstructorsCallableByProxy(type);

            foreach (var constructor in constructors)
            {
                IEnumerable<ArgumentInfo> arguments = this.TryToResolveAllArguments(GetConstructorParameterTypes(constructor));
                yield return new ConstructorAndArgumentsInfo()
                {
                    Constructor = constructor,
                    Arguments = arguments
                };
            }
        }

        private static IEnumerable<Type> GetConstructorParameterTypes(ConstructorInfo constructor)
        {
            return constructor.GetParameters().Select(x => x.ParameterType);
        }

        private IEnumerable<ConstructorInfo> GetConstructorsCallableByProxy(Type type)
        {
            return
                from constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where !constructor.IsPrivate
                select constructor;
        }

        private bool TryResolveDummyValueOfType(Type typeOfValue, out object dummyValue)
        {
            return this.session.DummyCreator.TryCreateDummyValue(typeOfValue, out dummyValue);
        }

        private IEnumerable<ArgumentInfo> TryToResolveAllArguments(IEnumerable<Type> types)
        {
            return
                from parameterType in types
                select this.CreateArgumentInfo(parameterType);
        }

        private ArgumentInfo CreateArgumentInfo(Type typeOfArgument)
        {
            object argumentValue = null;
            var wasResolved = this.TryResolveDummyValueOfType(typeOfArgument, out argumentValue);

            return new ArgumentInfo
            {
                WasSuccessfullyResolved = wasResolved,
                ResolvedValue = argumentValue,
                TypeOfArgument = typeOfArgument
            };
        }
    }
}