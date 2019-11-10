namespace FakeItEasy.Creation.DelegateProxies
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    internal class DelegateProxyInterceptionValidator : IMethodInterceptionValidator
    {
        public virtual bool MethodCanBeInterceptedOnInstance(MethodInfo method, object? callTarget, [NotNullWhen(false)]out string? failReason)
        {
            Guard.AgainstNull(method, nameof(method));

            if (method.Name != "Invoke")
            {
                failReason = "Only the Invoke method can be intercepted on delegates.";
                return false;
            }

            failReason = null;
            return true;
        }
    }
}
