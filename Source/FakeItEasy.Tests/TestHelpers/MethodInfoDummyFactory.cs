namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using System.Reflection;

    public class MethodInfoDummyFactory : DummyFactory<MethodInfo>
    {
        protected override MethodInfo CreateDummy()
        {
            return typeof(object).GetMethod("ToString", new Type[] { });
        }
    }
}
