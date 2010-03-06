namespace FakeItEasy.DynamicProxy
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using FakeItEasy.Core;
using System;

    /// <summary>
    /// An adapter that adapts an <see cref="IInvocation" /> to a <see cref="IFakeObjectCall" />.
    /// </summary>
    [Serializable]
    internal class InvocationCallAdapter
        : IWritableFakeObjectCall, ICompletedFakeObjectCall
    {
        private IInvocation Invocation;
        
        private static readonly Dictionary<MethodInfo, MethodInfo> objectMembersMap = new Dictionary<MethodInfo, MethodInfo>
        {
            { typeof(ICanInterceptObjectMembers).GetMethod("Equals", new[] { typeof(object) }), typeof(object).GetMethod("Equals", new[] { typeof(object) }) },
            { typeof(ICanInterceptObjectMembers).GetMethod("GetHashCode"), typeof(object).GetMethod("GetHashCode") },
            { typeof(ICanInterceptObjectMembers).GetMethod("ToString"), typeof(object).GetMethod("ToString") }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationCallAdapter"/> class.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        [DebuggerStepThrough]
        public InvocationCallAdapter(IInvocation invocation)
        {
            this.Invocation = invocation;
            this.Method = RewriteMappedMethod(invocation);

            this.Arguments = new ArgumentCollection(invocation.Arguments, this.Method);
        }


        /// <summary>
        /// The method that's called.
        /// </summary>
        public MethodInfo Method
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the return value of the call.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        [DebuggerStepThrough]
        public void SetReturnValue(object returnValue)
        {
            this.Invocation.ReturnValue = returnValue;
        }

        /// <summary>
        /// The arguments used in the call.
        /// </summary>
        public ArgumentCollection Arguments
        {
            get;
            private set;
        }

        /// <summary>
        /// The value set to be returned from the call.
        /// </summary>
        public object ReturnValue
        {
            get { return this.Invocation.ReturnValue; }
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

        /// <summary>
        /// The faked object the call is performed on.
        /// </summary>
        /// <value></value>
        public object FakedObject
        {
            get { return this.Invocation.Proxy; }
        }


        /// <summary>
        /// Freezes the call so that it can no longer be modified.
        /// </summary>
        /// <returns>A completed fake object call.</returns>
        public ICompletedFakeObjectCall AsReadOnly()
        {
            return this;
        }

        /// <summary>
        /// Calls the base method, should not be used with interface types.
        /// </summary>
        public void CallBaseMethod()
        {
            this.Invocation.Proceed();
        }

        /// <summary>
        /// Sets the specified value to the argument at the specified index.
        /// </summary>
        /// <param name="index">The index of the argument to set the value to.</param>
        /// <param name="value">The value to set to the argument.</param>
        public void SetArgumentValue(int index, object value)
        {
            this.Invocation.SetArgumentValue(index, value);
        }

        private static MethodInfo RewriteMappedMethod(IInvocation invocation)
        {
            MethodInfo result;

            if (!objectMembersMap.TryGetValue(invocation.Method, out result))
            {
                result = invocation.Method;
            }

            return result;
        }
    }
}
