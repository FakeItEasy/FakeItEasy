namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System.Linq;
    using System.Reflection;
    using Core;

    internal class CastleDynamicProxyInterceptionValidator
    {
        private readonly MethodInfoManager methodInfoManager;

        public CastleDynamicProxyInterceptionValidator(MethodInfoManager methodInfoManager)
        {
            this.methodInfoManager = methodInfoManager;
        }

        public virtual bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            var invokedMethod = this.GetInvokedMethod(method, callTarget);

            failReason = GetReasonForWhyMethodCanNotBeIntercepted(invokedMethod);
            return failReason == null;
        }

        private static string GetReasonForWhyMethodCanNotBeIntercepted(MethodInfo method)
        {
            if (method.IsFinal)
            {
                return "Sealed methods can not be intercepted.";
            }

            if (method.IsStatic)
            {
                if (method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false).Any())
                {
                    return "Extension methods can not be intercepted since they're static.";
                }
                else
                {
                    return "Static methods can not be intercepted.";
                }
            }

            if (!method.IsVirtual)
            {
                return "Non virtual methods can not be intercepted.";    
            }

            return null;
        }

        private MethodInfo GetInvokedMethod(MethodInfo method, object callTarget)
        {
            var invokedMehtod = method;

            if (callTarget != null)
            {
                invokedMehtod = this.methodInfoManager.GetMethodOnTypeThatWillBeInvokedByMethodInfo(
                    callTarget.GetType(), method);
            }
            return invokedMehtod;
        }
    }
}