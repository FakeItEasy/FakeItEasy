namespace FakeItEasy.Creation
{
    using Castle.DynamicProxy;
    using FakeItEasy.Core;
    using FakeItEasy.Creation.CastleDynamicProxy;

    internal class ProxyInterceptor
        : IInterceptor
    {
        private readonly IFakeCallProcessorProvider fakeCallProcessorProvider;

        public ProxyInterceptor(IFakeCallProcessorProvider fakeCallProcessorProvider)
        {
            this.fakeCallProcessorProvider = fakeCallProcessorProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            Guard.AgainstNull(invocation);
            var call = new CastleInvocationCallAdapter(invocation);
            this.fakeCallProcessorProvider.Fetch(invocation.Proxy).Process(call);
        }
    }
}
