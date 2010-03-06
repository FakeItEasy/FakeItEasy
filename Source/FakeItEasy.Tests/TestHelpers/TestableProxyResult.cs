using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Core;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests.TestHelpers
{
    internal class TestableProxyResult
               : ProxyResult
    {
        public TestableProxyResult(Type type, IFakedProxy proxy)
            : base(type)
        {
            this.Proxy = proxy;
            this.ProxyWasSuccessfullyCreated = true;
        }

        public void RaiseCallWasIntercepted(IWritableFakeObjectCall call)
        {
            var handler = this.CallWasIntercepted;
            if (handler != null)
            {
                handler(this, new CallInterceptedEventArgs(call));
            }
        }

        public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
    }
}
