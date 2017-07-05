namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Castle.DynamicProxy;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// An adapter that adapts an <see cref="IInvocation" /> to a <see cref="IFakeObjectCall" />.
    /// </summary>
#if FEATURE_BINARY_SERIALIZATION
    [Serializable]
#endif
    internal class CastleInvocationCallAdapter
        : IInterceptedFakeObjectCall
    {
        private readonly IInvocation invocation;
        private readonly object[] originalArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CastleInvocationCallAdapter"/> class.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        [DebuggerStepThrough]
        public CastleInvocationCallAdapter(IInvocation invocation)
        {
            this.invocation = invocation;
            this.originalArguments = invocation.Arguments.ToArray();
            this.Method = invocation.Method;
            this.Arguments = new ArgumentCollection(invocation.Arguments, this.Method);
        }

        /// <summary>
        /// Gets the method that's called.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the arguments used in the call.
        /// </summary>
        public ArgumentCollection Arguments { get; }

        /// <summary>
        /// Gets the faked object the call is performed on.
        /// </summary>
        public object FakedObject => this.invocation.Proxy;

        /// <summary>
        /// Freezes the call so that it can no longer be modified.
        /// </summary>
        /// <returns>A completed fake object call.</returns>
        public ICompletedFakeObjectCall AsReadOnly()
        {
            return new CompletedFakeObjectCall(
                this,
                this.originalArguments,
                this.invocation.ReturnValue);
        }

        /// <summary>
        /// Calls the base method, should not be used with interface types.
        /// </summary>
        public void CallBaseMethod()
        {
            this.invocation.Proceed();
        }

        /// <summary>
        /// Sets the specified value to the argument at the specified index.
        /// </summary>
        /// <param name="index">The index of the argument to set the value to.</param>
        /// <param name="value">The value to set to the argument.</param>
        public void SetArgumentValue(int index, object value)
        {
            this.invocation.SetArgumentValue(index, value);
        }

        /// <summary>
        /// Sets the return value of the call.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        [DebuggerStepThrough]
        public void SetReturnValue(object returnValue)
        {
            this.invocation.ReturnValue = returnValue;
        }

        /// <summary>
        /// Returns a description of the call.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetDescription();
        }
    }
}
