namespace FakeItEasy.Core
{
    using System;
    using FakeItEasy.Creation;
    using System.Linq;
    using System.Reflection;

    internal interface IFixtureInitializer
    {
        void InitializeFakes(object fixture);
    }

    internal class DefaultFixtureInitializer
        : IFixtureInitializer
    {
        private IFakeAndDummyManager fakeAndDummyManager;

        public DefaultFixtureInitializer(IFakeAndDummyManager fakeAndDummyManager)
        {
            this.fakeAndDummyManager = fakeAndDummyManager;
        }

        public void InitializeFakes(object fixture)
        {
            var settersForTaggedMembers =
                from member in fixture.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                where MemberIsTaggedWithFakeAttribute(member)
                select GetSetterForMember(fixture, member);

            foreach (var sette in settersForTaggedMembers)
            {
                var fake = this.fakeAndDummyManager.CreateFake(sette.MemberType, FakeOptions.Empty);

                sette.Setter.Invoke(fake);
            }
        }

        private static SettableMemberInfo GetSetterForMember(object fixture, MemberInfo member)
        {
            var property = member as PropertyInfo;
            if (property != null)
            {
                return new SettableMemberInfo
                {
                    MemberType = property.PropertyType,
                    Setter = x => property.GetSetMethod(nonPublic: true).Invoke(fixture, new object[] { x })
                };
            }

            var field = member as FieldInfo;
            if (field != null)
            {
                return new SettableMemberInfo
                {
                    MemberType = field.FieldType,
                    Setter = x => field.SetValue(fixture, x)
                };
            }

            return null;
        }

        private class SettableMemberInfo
        {
            public Type MemberType;

            public Action<object> Setter;
        }

        private static bool MemberIsTaggedWithFakeAttribute(MemberInfo member)
        {
            return (from attribute in member.GetCustomAttributes(typeof(FakeAttribute), false)
                    select attribute).Any();
        }
    }
}
