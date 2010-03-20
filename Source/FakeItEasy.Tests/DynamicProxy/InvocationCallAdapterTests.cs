using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Core.Interceptor;
using FakeItEasy.Core;
using System.Reflection;
using FakeItEasy.DynamicProxy;

namespace FakeItEasy.Tests.DynamicProxy
{
    [TestFixture]
    public class InvocationCallAdapterTests
    {
        [Test]
        public void CallBaseMethod_should_call_Proceed_on_invokation()
        {
            var invokation = A.Fake<IInvocation>();

            A.CallTo(() => invokation.Arguments).Returns(new object[] { });
            A.CallTo(() => invokation.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new InvocationCallAdapter(invokation);

            adapter.CallBaseMethod();

            A.CallTo(() => invokation.Proceed()).MustHaveHappened(Repeated.Once);
        }

        [Test]
        public void SetArgumentValue_sets_the_argument_value_of_the_invokation()
        {
            var invocation = A.Fake<IInvocation>();
            Configure.Fake(invocation).CallsTo(x => x.Arguments).Returns(new object[] { });
            Configure.Fake(invocation).CallsTo(x => x.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new InvocationCallAdapter(invocation);

            adapter.SetArgumentValue(0, "test");

            A.CallTo(() => invocation.SetArgumentValue(0, "test")).MustHaveHappened(Repeated.Once);
        }

        private MethodInfo[] interfaceMethods = new MethodInfo[]
            {
                typeof(ICanInterceptObjectMembers).GetMethod("Equals", new Type[] { typeof(object) }),
                typeof(ICanInterceptObjectMembers).GetMethod("GetHashCode", new Type[] { }),
                typeof(ICanInterceptObjectMembers).GetMethod("ToString", new Type[] { })
            };

        private MethodInfo[] objectMethods = new MethodInfo[]
            {
                typeof(object).GetMethod("Equals", new Type[] { typeof(object) }),
                typeof(object).GetMethod("GetHashCode", new Type[] { }),
                typeof(object).GetMethod("ToString", new Type[] { })
            };

        [Test]
        [Sequential]
        public void Method_should_rewrite_method_from_ICanInterceptObjectMembers_to_mapped_object_method_when_called(
            [ValueSource("interfaceMethods")] MethodInfo interfaceMethod, 
            [ValueSource("objectMethods")] MethodInfo objectMethod)
        {
            var invocation = A.Fake<IInvocation>();
            Configure.Fake(invocation)
                .CallsTo(x => x.Arguments).Returns(interfaceMethod.GetParameters());
            Configure.Fake(invocation)
                .CallsTo(x => x.Method).Returns(interfaceMethod);

            var adapter = new InvocationCallAdapter(invocation);

            Assert.That(adapter.Method, Is.EqualTo(objectMethod));
        }
    }
}
