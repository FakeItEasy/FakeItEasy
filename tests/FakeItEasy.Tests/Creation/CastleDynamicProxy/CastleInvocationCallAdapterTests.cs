namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
    using Castle.DynamicProxy;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using Xunit;

    using static FakeItEasy.Tests.TestHelpers.ExpressionHelper;

    public class CastleInvocationCallAdapterTests
    {
        [Fact]
        public void CallBaseMethod_should_call_Proceed_on_invocation()
        {
            var invocation = A.Fake<IInvocation>();

            A.CallTo(() => invocation.Arguments).Returns(Array.Empty<object>());
            A.CallTo(() => invocation.Method).Returns(GetMethodInfo<IFoo>(x => x.Bar()));

            var adapter = new CastleInvocationCallAdapter(invocation);

            adapter.CallBaseMethod();

            A.CallTo(() => invocation.Proceed()).MustHaveHappened();
        }

        [Fact]
        public void SetArgumentValue_sets_the_argument_value_of_the_invocation()
        {
            var invocation = A.Fake<IInvocation>();
            A.CallTo(() => invocation.Arguments).Returns(Array.Empty<object>());
            A.CallTo(() => invocation.Method).Returns(GetMethodInfo<IFoo>(x => x.Bar()));

            var adapter = new CastleInvocationCallAdapter(invocation);

            adapter.SetArgumentValue(0, "test");

            A.CallTo(() => invocation.SetArgumentValue(0, "test")).MustHaveHappened();
        }
    }
}
