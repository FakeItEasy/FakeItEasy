namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents an event that happens when a call has been intercepted by a proxy.
    /// </summary>
    [Serializable]
    public class CallInterceptedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallInterceptedEventArgs"/> class.
        /// </summary>
        /// <param name="call">The call.</param>
        [DebuggerStepThrough]
        public CallInterceptedEventArgs(IWritableFakeObjectCall call)
        {
            Guard.IsNotNull(call, "call");

            this.Call = call;
        }

        /// <summary>
        /// Gets the call that was intercepted.
        /// </summary>
        /// <value>The call.</value>
        public IWritableFakeObjectCall Call
        {
            get;
            private set;
        }
    }
}
