namespace FakeItEasy.Tests.TestHelpers
{
    using System;
    using FakeItEasy.Core;
    using FakeItEasy.Core.Generation;

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
