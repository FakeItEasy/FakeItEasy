namespace FakeItEasy.Creation.DelegateProxies
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
    internal class CastleInvocationDelegateCallAdapter
        : InterceptedFakeObjectCall
    {
        private readonly IInvocation invocation;
        private readonly Delegate theDelegate;
        private readonly object[] originalArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CastleInvocationDelegateCallAdapter"/> class.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="theDelegate">The delegate teh invocation was made on.</param>
        [DebuggerStepThrough]
        public CastleInvocationDelegateCallAdapter(IInvocation invocation, Delegate theDelegate)
        {
            this.invocation = invocation;
            this.theDelegate = theDelegate;
            this.originalArguments = this.invocation.Arguments.ToArray();
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
        public override object FakedObject => this.theDelegate;

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
            throw new FakeConfigurationException(ExceptionMessages.DelegateCannotCallBaseMethod);
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
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetDescription();
        }
    }
}
