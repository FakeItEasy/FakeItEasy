namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy.Sdk;

    internal static class DefaultSutInitializer
    {
        public static object CreateSut(Type typeOfSut, Action<Type, object> onFakeCreated)
        {
            var constructorSignature = from parameter in GetWidestConstructor(typeOfSut).GetParameters()
                                       select parameter.ParameterType;

            var resolvedArguments = ResolveArguments(constructorSignature, onFakeCreated);

            var argumentsArray = constructorSignature.Select(x => resolvedArguments[x]).ToArray();

            return Activator.CreateInstance(typeOfSut, argumentsArray);
        }

        private static ConstructorInfo GetWidestConstructor(Type type)
        {
            return type.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
        }

        private static Dictionary<Type, object> ResolveArguments(IEnumerable<Type> constructorSignature, Action<Type, object> onFakeCreated)
        {
            return constructorSignature
                .Distinct()
                .ToDictionary(key => key, value => CreateFake(value, onFakeCreated));
        }

        private static object CreateFake(Type typeOfFake, Action<Type, object> onFakeCreated)
        {
            var result = Create.Fake(typeOfFake);
            onFakeCreated.Invoke(typeOfFake, result);
            return result;
        }
    }
}
