namespace FakeItEasy.SelfInitializedFakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// DTO for recorded calls.
    /// </summary>
    [Serializable]
    public class CallData
    {
        public CallData(MethodInfo method, IEnumerable<object> outputArguments, object returnValue)
        {
            this.Method = method;
            //this.InputArguments = inputArguments.ToArray();
            this.OutputArguments = outputArguments.ToArray();
            this.ReturnValue = returnValue;
        }

        public MethodInfo Method { get; private set; }
        //public IEnumerable<object> InputArguments { get; private set; }
        public IEnumerable<object> OutputArguments { get; private set; }
        public object ReturnValue { get; private set; }
    }
}