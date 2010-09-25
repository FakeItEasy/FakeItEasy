using System;
using System.Reflection;
using Castle.DynamicProxy;
using FakeItEasy.Creation.CastleDynamicProxy;
using NUnit.Framework;

namespace FakeItEasy.Tests.Creation.CastleDynamicProxy
{
    [TestFixture]
    public class CastleInvocationCallAdapterTests
    {
        [Test]
        public void CallBaseMethod_should_call_Proceed_on_invokation()
        {
            var invokation = A.Fake<IInvocation>();

            A.CallTo(() => invokation.Arguments).Returns(new object[] { });
            A.CallTo(() => invokation.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new CastleInvocationCallAdapter(invokation);

            adapter.CallBaseMethod();

            A.CallTo(() => invokation.Proceed()).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void SetArgumentValue_sets_the_argument_value_of_the_invokation()
        {
            var invocation = A.Fake<IInvocation>();
            A.CallTo(() => invocation.Arguments).Returns(new object[] { });
            A.CallTo(() => invocation.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new CastleInvocationCallAdapter(invocation);

            adapter.SetArgumentValue(0, "test");

            A.CallTo(() => invocation.SetArgumentValue(0, "test")).MustHaveHappened(Repeated.Once);
        }

        private MethodInfo[] objectMethods = new MethodInfo[]
            {
                typeof(object).GetMethod("Equals", new Type[] { typeof(object) }),
                typeof(object).GetMethod("GetHashCode", new Type[] { }),
                typeof(object).GetMethod("ToString", new Type[] { })
            };
    }
}
