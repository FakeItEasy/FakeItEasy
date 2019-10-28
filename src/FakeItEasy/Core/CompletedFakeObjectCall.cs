namespace FakeItEasy.Core
{
    using System.Reflection;
    using FakeItEasy.Configuration;

    internal class CompletedFakeObjectCall : ICompletedFakeObjectCall
    {
        public CompletedFakeObjectCall(IInterceptedFakeObjectCall interceptedCall, object[] arguments)
        {
            this.FakedObject = interceptedCall.FakedObject;
            this.Method = interceptedCall.Method;
            this.Arguments = new ArgumentCollection(arguments, this.Method);
            this.ArgumentsAfterCall = interceptedCall.Arguments;
            this.SequenceNumber = SequenceNumberManager.GetNextSequenceNumber();
        }

        public object? ReturnValue { get; set; }

        public MethodInfo Method { get; }

        public ArgumentCollection Arguments { get; }

        public ArgumentCollection ArgumentsAfterCall { get; }

        public object FakedObject { get; }

        public int SequenceNumber { get; }
    }
}