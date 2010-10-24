namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Reflection;

    public class ReflectionHelper
    {
        public static PropertyInfo VirtualProperty
        {
            get
            {
                return typeof(Foo).GetProperty("VirtualProperty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
        }

        
    }

    public class MethodInfoDummyDefinition : DummyDefinition<MethodInfo>
    {
        protected override MethodInfo CreateDummy()
        {
            return typeof (object).GetMethod("ToString", new Type[] {});
        }
    }

}
