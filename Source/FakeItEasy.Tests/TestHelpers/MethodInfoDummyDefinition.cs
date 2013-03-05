namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Reflection;

    public class MethodInfoDummyDefinition : DummyDefinition<MethodInfo>
    {
        protected override MethodInfo CreateDummy()
        {
            return typeof(object).GetMethod("ToString", new Type[] { });
        }
    }
}
