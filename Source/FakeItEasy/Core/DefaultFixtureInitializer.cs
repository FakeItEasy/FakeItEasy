namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Creation;

    internal class DefaultFixtureInitializer
        : IFixtureInitializer
    {
        private readonly IFakeAndDummyManager fakeAndDummyManager;
        private readonly ISutInitializer sutInitializer;

        public DefaultFixtureInitializer(IFakeAndDummyManager fakeAndDummyManager, ISutInitializer sutInitializer)
        {
            this.fakeAndDummyManager = fakeAndDummyManager;
            this.sutInitializer = sutInitializer;
        }

        public void InitializeFakes(object fixture)
        {
            var fakesUsedToCreateSut = this.InitializeSut(fixture);

            this.InitializeFakes(fixture, fakesUsedToCreateSut);
        }

        private static SettableMemberInfo GetSutSetter(object fixture)
        {
            var allSettersTaggedUnderTest = GetMembersTaggedWithAttribute(fixture, typeof(UnderTestAttribute));
            
            if (CountIsNoMoreThanOne(allSettersTaggedUnderTest))
            {
                throw new InvalidOperationException("A fake fixture can only contain one member marked \"under test\".");
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

        private void InitializeFakes(object fixture, Dictionary<Type, object> fakesUsedToCreateSut)
        {
            var settersForTaggedMembers = GetMembersTaggedWithAttribute(fixture, typeof(FakeAttribute));

            foreach (var setter in settersForTaggedMembers)
            {
                object fake = null;

                if (!fakesUsedToCreateSut.TryGetValue(setter.MemberType, out fake))
                {
                    fake = this.fakeAndDummyManager.CreateFake(setter.MemberType, new FakeOptions());
                }

                setter.Setter(fake);
            }
        }

        private Dictionary<Type, object> InitializeSut(object fixture)
        {
            var result = new Dictionary<Type, object>();

            var setter = GetSutSetter(fixture);
            if (setter != null)
            {
                var sut = this.sutInitializer.CreateSut(setter.MemberType, result.Add);
                setter.Setter(sut);
            }

            return result;
        }

        private class SettableMemberInfo
        {
            public Type MemberType { get; set; }

            public Action<object> Setter { get; set; }
        }
    }
}