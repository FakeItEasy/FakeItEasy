namespace FakeItEasy.Tests.TestHelpers
{
    using System.Reflection;
    using static FakeItEasy.Tests.TestHelpers.ExpressionHelper;

    public class MethodInfoDummyFactory : DummyFactory<MethodInfo>
    {
        protected override MethodInfo Create() =>
            GetMethodInfo<object>(x => x.ToString());
    }
}
