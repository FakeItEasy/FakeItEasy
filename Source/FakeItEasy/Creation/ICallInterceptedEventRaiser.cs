namespace FakeItEasy.Creation
{
    using System;
    using FakeItEasy.Core;

    public interface ICallInterceptedEventRaiser
    {
        event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
    }
}
