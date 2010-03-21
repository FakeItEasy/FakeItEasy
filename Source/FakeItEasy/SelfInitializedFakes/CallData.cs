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
        /// <summary>
        /// Initializes a new instance of the <see cref="CallData"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="outputArguments">The output arguments.</param>
        /// <param name="returnValue">The return value.</param>
        public CallData(MethodInfo method, IEnumerable<object> outputArguments, object returnValue)
        {
            this.Method = method;
            this.OutputArguments = outputArguments.ToArray();
            this.ReturnValue = returnValue;
        }

        /// <summary>
        /// Gets the method that was called.
        /// </summary>
        /// <value>The method.</value>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Gets the output arguments of the call.
        /// </summary>
        /// <value>The output arguments.</value>
        public IEnumerable<object> OutputArguments { get; private set; }

        /// <summary>
        /// Gets the return value of the call.
        /// </summary>
        /// <value>The return value.</value>
        public object ReturnValue { get; private set; }
    }
}