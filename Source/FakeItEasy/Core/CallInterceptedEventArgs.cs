namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Serializable]
    public class CallInterceptedEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public CallInterceptedEventArgs(IWritableFakeObjectCall call)
        {
            Guard.IsNotNull(call, "call");

            this.Call = call;
        }

        public IWritableFakeObjectCall Call
        {
            get;
            private set;
        }
    }
}
