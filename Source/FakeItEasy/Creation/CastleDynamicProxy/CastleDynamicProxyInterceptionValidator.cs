namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
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
            if (IsDynamicProxyType(method.DeclaringType))
            {
                return null;
            }

            if (method.IsFinal)
            {
                var explicitImplementation = method.Name.Contains('.');
                if (!explicitImplementation)
                {
                    return "Sealed methods can not be intercepted.";
                }
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

            if (!Castle.DynamicProxy.Internal.InternalsUtil.IsAccessible(method))
            {
                return string.Concat(
                    "Methods not accessible to ",
                    Castle.DynamicProxy.ModuleScope.DEFAULT_ASSEMBLY_NAME,
                    " can not be intercepted.");
            }

            return null;
        }

        private static bool IsDynamicProxyType(Type declaringType)
        {
            return declaringType != null &&
                   declaringType.Assembly.Name() == Castle.DynamicProxy.ModuleScope.DEFAULT_ASSEMBLY_NAME;
        }

        private MethodInfo GetInvokedMethod(MethodInfo method, object callTarget)
        {
            var invokedMethod = method;

            if (callTarget != null)
            {
                invokedMethod = this.methodInfoManager.GetMethodOnTypeThatWillBeInvokedByMethodInfo(
                    callTarget.GetType(), method);
            }

            return invokedMethod;
        }
    }
}