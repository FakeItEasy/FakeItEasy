namespace FakeItEasy.Core
{
    using FakeItEasy.Core.Creation;

    /// <summary>
    /// Attatches a fake manager to the proxy so that intercepted
    /// calls can be configured.
    /// </summary>
    internal interface IFakeManagerAttacher
    {
        void AttachFakeObjectToProxy(object proxy, ICallInterceptedEventRaiser eventRaiser);
    }
}
