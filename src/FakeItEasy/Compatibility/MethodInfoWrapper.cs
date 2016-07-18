namespace FakeItEasy.Compatibility
{
    using System;
    using System.Reflection;

    /// <summary>
    /// This is a wrapper class that stores the ReflectedType for a MethodInfo.
    /// Until ReflectedType is back in RTM (tracked by CoreFX issue 5381), this
    /// wrapper is used to temporarily provide the needed ReflectedType.
    /// </summary>
    internal class MethodInfoWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInfoWrapper" /> class.
        /// </summary>
        /// <param name="methodInfo">The <see cref="System.Reflection.MethodInfo"/> instance to wrap.</param>
        /// <param name="type">The owning type of the <see cref="System.Reflection.MethodInfo"/> instance.</param>
        public MethodInfoWrapper(MethodInfo methodInfo, Type type)
        {
            this.Method = methodInfo;
            this.ReflectedType = type;
        }

        /// <summary>
        /// Gets the wrapped MethodInfo instance.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the reflected type of the wrapped MethodInfo instance.
        /// </summary>
        public Type ReflectedType { get; }
    }
}
