namespace FakeItEasy.Core
{
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Configuration;

    internal class CompletedFakeObjectCall : ICompletedFakeObjectCall
    {
        public CompletedFakeObjectCall(IInterceptedFakeObjectCall interceptedCall, object[] arguments, object returnValue)
        {
            this.FakedObject = interceptedCall.FakedObject;
            this.Method = interceptedCall.Method;
            this.Arguments = new ArgumentCollection(arguments, this.Method);
            this.ReturnValue = returnValue;
        }

        public object ReturnValue { get; }

        public MethodInfo Method { get; }

        public ArgumentCollection Arguments { get; }

        public object FakedObject { get; }
    }
}