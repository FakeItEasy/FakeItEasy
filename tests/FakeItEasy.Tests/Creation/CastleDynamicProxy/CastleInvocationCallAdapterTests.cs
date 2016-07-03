namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    using System;
#if FEATURE_NETCORE_REFLECTION
    using System.Reflection;
#endif
    using Castle.DynamicProxy;
    using FakeItEasy.Creation.CastleDynamicProxy;
    using Xunit;

    public class CastleInvocationCallAdapterTests
    {
        [Fact]
        public void CallBaseMethod_should_call_Proceed_on_invocation()
        {
            var invocation = A.Fake<IInvocation>();

            A.CallTo(() => invocation.Arguments).Returns(new object[] { });
            A.CallTo(() => invocation.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new CastleInvocationCallAdapter(invocation);

            adapter.CallBaseMethod();

            A.CallTo(() => invocation.Proceed()).MustHaveHappened();
        }

        [Fact]
        public void SetArgumentValue_sets_the_argument_value_of_the_invocation()
        {
            var invocation = A.Fake<IInvocation>();
            A.CallTo(() => invocation.Arguments).Returns(new object[] { });
            A.CallTo(() => invocation.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new CastleInvocationCallAdapter(invocation);

            adapter.SetArgumentValue(0, "test");

            A.CallTo(() => invocation.SetArgumentValue(0, "test")).MustHaveHappened();
        }
    }
}
