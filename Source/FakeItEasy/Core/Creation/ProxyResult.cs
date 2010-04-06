namespace FakeItEasy.Core.Creation
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a result from an IProxyGenerator.
    /// </summary>
    [Serializable]
    public abstract class ProxyResult
    {
        private IFakedProxy proxyField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyResult"/> class.
        /// </summary>
        /// <param name="proxiedType">Type of the proxied.</param>
        protected ProxyResult(Type proxiedType)
        {
            Guard.AgainstNull(proxiedType, "proxiedType");

            this.ProxiedType = proxiedType;
        }

        /// <summary>
        /// Gets the type of the generated proxy.
        /// </summary>
        /// <value>The type of the generated proxy.</value>
        public Type ProxiedType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the proxy was successfully created.
        /// </summary>
        /// <value>
        /// 	<c>true</c> If proxy was successfully created; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ProxyWasSuccessfullyCreated
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets an error message when the proxy was not successfully created. 
        /// </summary>
        /// <value>The error message.</value>
        public virtual string ErrorMessage
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the generated proxy when successfully generated.
        /// </summary>
        /// <value>The generated proxy.</value>
        public virtual IFakedProxy Proxy
        {
            [DebuggerStepThrough]
            get
            {
                return this.proxyField;
            }
            [DebuggerStepThrough]
            protected set
            {
                Guard.AgainstNull(value, "value");

                if (!this.ProxiedType.IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException("The specified proxy is not of the correct type.");
                }

                this.proxyField = value;
            }
        }

        /// <summary>
        /// Occurs when a call to the proxy was intercepted.
        /// </summary>
        public abstract event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
    }
}