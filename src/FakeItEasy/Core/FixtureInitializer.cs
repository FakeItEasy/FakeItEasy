namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Sdk;

    [Obsolete("Test fixture initialization will be removed in version 5.0.0.")]
    internal static class FixtureInitializer
    {
        public static void InitializeFakes(object fixture)
        {
            var fakesCreatedForTypes = new Dictionary<Type, object>();
            InitializeSut(fixture, fakesCreatedForTypes);

            InitializeFakes(fixture, fakesCreatedForTypes);
        }

        private static SettableMemberInfo GetSutSetter(object fixture)
        {
            var allSettersTaggedUnderTest = GetMembersTaggedWithAttribute(fixture, typeof(UnderTestAttribute));

            if (CountIsNoMoreThanOne(allSettersTaggedUnderTest))
            {
                throw new InvalidOperationException($"A fake fixture can only contain one member marked with {nameof(UnderTestAttribute)}.");
            }

            return allSettersTaggedUnderTest.SingleOrDefault();
        }

        private static bool CountIsNoMoreThanOne<T>(IEnumerable<T> collection)
        {
            return collection.Cast<object>().Skip(1).FirstOrDefault() != null;
        }

        private static IEnumerable<SettableMemberInfo> GetMembersTaggedWithAttribute(object getMembersFor, Type attributeType)
        {
            return
                from member in getMembersFor.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                where MemberIsTaggedWithAttribute(member, attributeType)
                select GetSetterForMember(getMembersFor, member);
        }

        private static SettableMemberInfo GetSetterForMember(object instance, MemberInfo member)
        {
            var property = member as PropertyInfo;
            if (property != null)
            {
                return new SettableMemberInfo
                           {
                               MemberType = property.PropertyType,
                               Setter = x => property.GetSetMethod(nonPublic: true).Invoke(instance, new[] { x })
                           };
            }

            var field = member as FieldInfo;
            if (field != null)
            {
                return new SettableMemberInfo
                           {
                               MemberType = field.FieldType,
                               Setter = x => field.SetValue(instance, x)
                           };
            }

            return null;
        }

        private static bool MemberIsTaggedWithAttribute(MemberInfo member, Type attributeType)
        {
            return (from attribute in member.GetCustomAttributes(attributeType, false)
                    select attribute).Any();
        }

        private static void InitializeFakes(object fixture, Dictionary<Type, object> fakesUsedToCreateSut)
        {
            var settersForTaggedMembers = GetMembersTaggedWithAttribute(fixture, typeof(FakeAttribute));

            foreach (var setter in settersForTaggedMembers)
            {
                object fake = null;

                if (!fakesUsedToCreateSut.TryGetValue(setter.MemberType, out fake))
                {
                    fake = Create.Fake(setter.MemberType);
                }

                setter.Setter(fake);
            }
        }

        private static void InitializeSut(object fixture, Dictionary<Type, object> fakesCreatedForTypes)
        {
            var setter = GetSutSetter(fixture);
            if (setter != null)
            {
                var sut = CreateSut(setter.MemberType, fakesCreatedForTypes);
                setter.Setter(sut);
            }
        }

        private static object CreateSut(Type typeOfSut, Dictionary<Type, object> fakesCreatedForTypes)
        {
            var constructorSignature = from parameter in GetWidestConstructor(typeOfSut).GetParameters()
                                       select parameter.ParameterType;

            var resolvedArguments = ResolveArguments(constructorSignature, fakesCreatedForTypes);

            var argumentsArray = constructorSignature.Select(x => resolvedArguments[x]).ToArray();

            return Activator.CreateInstance(typeOfSut, argumentsArray);
        }

        private static ConstructorInfo GetWidestConstructor(Type type)
        {
            return type.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
        }

        private static Dictionary<Type, object> ResolveArguments(IEnumerable<Type> constructorSignature, Dictionary<Type, object> fakesCreatedForTypes)
        {
            return constructorSignature
                .Distinct()
                .ToDictionary(key => key, value => CreateFake(value, fakesCreatedForTypes));
        }

        private static object CreateFake(Type typeOfFake, Dictionary<Type, object> fakesCreatedForTypes)
        {
            return fakesCreatedForTypes[typeOfFake] = Create.Fake(typeOfFake);
        }

        private class SettableMemberInfo
        {
            public Type MemberType { get; set; }

            public Action<object> Setter { get; set; }
        }
    }
}
