namespace FakeItEasy.Creation
{
    using System;
    using FakeItEasy.Core;

    /// <summary>
    /// An object that raises an event every time a call to a proxy has been intercepted.
    /// </summary>
    public interface ICallInterceptedEventRaiser
    {
        /// <summary>
        /// Raised when a call is intercepted.
        /// </summary>
        event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
    }
}
