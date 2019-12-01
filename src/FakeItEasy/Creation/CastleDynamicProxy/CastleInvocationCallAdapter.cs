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
    internal class CastleInvocationCallAdapter
        : InterceptedFakeObjectCall
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
            var savedArguments = new object[invocation.Arguments.Length];
            Array.Copy(invocation.Arguments, savedArguments, savedArguments.Length);
            this.originalArguments = savedArguments;
            this.Method = invocation.Method;
            this.Arguments = new ArgumentCollection(invocation.Arguments, invocation.Method);
        }

        /// <summary>
        /// Gets the method that's called.
        /// </summary>
        public override MethodInfo Method { get; }

        /// <summary>
        /// Gets the arguments used in the call.
        /// </summary>
        public override ArgumentCollection Arguments { get; }

        /// <summary>
        /// Gets the faked object the call is performed on.
        /// </summary>
        public override object FakedObject => this.invocation.Proxy;

        public override object? ReturnValue
        {
            get => this.invocation.ReturnValue;
            set => this.invocation.ReturnValue = value;
        }

        /// <summary>
        /// Returns a completed call suitable for being recorded.
        /// </summary>
        /// <returns>A completed fake object call.</returns>
        public override CompletedFakeObjectCall ToCompletedCall()
        {
            return new CompletedFakeObjectCall(
                this,
                this.originalArguments);
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
        public override void SetArgumentValue(int index, object? value)
        {
            this.invocation.SetArgumentValue(index, value);
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
