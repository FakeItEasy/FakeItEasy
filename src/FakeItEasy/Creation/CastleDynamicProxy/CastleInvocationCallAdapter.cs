namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Diagnostics;
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
        : InterceptedFakeObjectCall
    {
        private readonly IInvocation invocation;
#pragma warning disable CA2235 // Mark all non-serializable fields
        private readonly object[] originalArguments;
#pragma warning restore CA2235 // Mark all non-serializable fields

        /// <summary>
        /// Initializes a new instance of the <see cref="CastleInvocationCallAdapter"/> class.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        [DebuggerStepThrough]
        public CastleInvocationCallAdapter(IInvocation invocation)
        {
            this.invocation = invocation;
            var savedArguments = new object[invocation.Arguments.Length];
            Array.Copy(invocation.Arguments, savedArguments, savedArguments.Length);
            this.originalArguments = savedArguments;
            this.Method = invocation.Method;
            this.Arguments = new ArgumentCollection(invocation.Arguments, invocation.Method);
        }

        /// <summary>
        /// Gets the method that's called.
        /// </summary>
#pragma warning disable CA2235 // Mark all non-serializable fields
        public override MethodInfo Method { get; }
#pragma warning restore CA2235 // Mark all non-serializable fields

        /// <summary>
        /// Gets the arguments used in the call.
        /// </summary>
        public override ArgumentCollection Arguments { get; }

        /// <summary>
        /// Gets the faked object the call is performed on.
        /// </summary>
        public override object FakedObject => this.invocation.Proxy;

        /// <summary>
        /// Freezes the call so that it can no longer be modified.
        /// </summary>
        /// <returns>A completed fake object call.</returns>
        public override CompletedFakeObjectCall AsReadOnly()
        {
            return new CompletedFakeObjectCall(
                this,
                this.originalArguments,
                this.invocation.ReturnValue);
        }

        /// <summary>
        /// Calls the base method, should not be used with interface types.
        /// </summary>
        public override void CallBaseMethod()
        {
            this.invocation.Proceed();
        }

        /// <summary>
        /// Sets the specified value to the argument at the specified index.
        /// </summary>
        /// <param name="index">The index of the argument to set the value to.</param>
        /// <param name="value">The value to set to the argument.</param>
        public override void SetArgumentValue(int index, object value)
        {
            this.invocation.SetArgumentValue(index, value);
        }

        /// <summary>
        /// Sets the return value of the call.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        [DebuggerStepThrough]
        public override void SetReturnValue(object returnValue)
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
