namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Creation;

    internal class DefaultSutInitializer
        : ISutInitializer
    {
        private readonly IFakeAndDummyManager fakeManager;

        public DefaultSutInitializer(IFakeAndDummyManager fakeManager)
        {
            this.fakeManager = fakeManager;
        }

        public object CreateSut(Type typeOfSut, Action<Type, object> onFakeCreated)
        {
            var constructorSignature = from parameter in GetWidestConstructor(typeOfSut).GetParameters()
                                       select parameter.ParameterType;

            var resolvedArguments = this.ResolveArguments(constructorSignature, onFakeCreated);

            var argumentsArray = constructorSignature.Select(x => resolvedArguments[x]).ToArray();
            
            return Activator.CreateInstance(typeOfSut, argumentsArray);
        }

        private static ConstructorInfo GetWidestConstructor(Type type)
        {
            return type.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
        }

        private Dictionary<Type, object> ResolveArguments(IEnumerable<Type> constructorSignature, Action<Type, object> onFakeCreated)
        {
            return constructorSignature
                .Distinct()
                .ToDictionary(key => key, value => this.CreateFake(value, onFakeCreated));
        }

        private object CreateFake(Type typeOfFake, Action<Type, object> onFakeCreated)
        {
            var result = this.fakeManager.CreateFake(typeOfFake, new FakeOptions());
            onFakeCreated.Invoke(typeOfFake, result);
            return result;
        }
    }
}