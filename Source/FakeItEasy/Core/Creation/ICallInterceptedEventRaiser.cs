namespace FakeItEasy.Core.Creation
{
    using System;

    public interface ICallInterceptedEventRaiser
    {
        event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
    }
}
