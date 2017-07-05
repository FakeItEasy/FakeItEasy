namespace FakeItEasy.Core
{
    using System.Reflection;
    using FakeItEasy.Configuration;

    internal class CompletedFakeObjectCall : ICompletedFakeObjectCall
    {
        public CompletedFakeObjectCall(IInterceptedFakeObjectCall interceptedCall, object[] arguments, object returnValue)
        {
            this.FakedObject = interceptedCall.FakedObject;
            this.Method = interceptedCall.Method;
            this.Arguments = new ArgumentCollection(arguments, this.Method);
            this.ArgumentsAfterCall = interceptedCall.Arguments;
            this.ReturnValue = returnValue;
        }

        public object ReturnValue { get; }

        public MethodInfo Method { get; }

        public ArgumentCollection Arguments { get; }

        public ArgumentCollection ArgumentsAfterCall { get; }

        public object FakedObject { get; }
    }
}