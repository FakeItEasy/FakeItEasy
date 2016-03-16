namespace FakeItEasy.Compatibility
{
    using System;
    using System.Reflection;

    /// <summary>
    /// This is a wrapper class that stores the ReflectedType for a MethodInfo.
    /// Until ReflectedType is back in RTM (tracked by CoreFX issue 5381), this
    /// wrapper is used to temporarily provide the needed ReflectedType.
    /// </summary>
    public class MethodInfoWrapper
    {
        public MethodInfoWrapper(MethodInfo methodInfo, Type type)
        {
            this.Method = methodInfo;
            this.ReflectedType = type;
        }

        public MethodInfo Method { get; }

        public Type ReflectedType { get; }
    }
}
