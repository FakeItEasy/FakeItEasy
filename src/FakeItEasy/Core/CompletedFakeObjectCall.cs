namespace FakeItEasy.Core
{
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Configuration;

    internal class CompletedFakeObjectCall : ICompletedFakeObjectCall
    {
        public CompletedFakeObjectCall(object fakedObject, MethodInfo method, object[] arguments, object returnValue)
        {
            this.FakedObject = fakedObject;
            this.Method = method;
            this.Arguments = new ArgumentCollection(arguments, method);
            this.ReturnValue = returnValue;
        }

        public object ReturnValue { get; }

        public MethodInfo Method { get; }

        public ArgumentCollection Arguments { get; }

        public object FakedObject { get; }
    }
}