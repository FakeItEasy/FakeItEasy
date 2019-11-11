namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class CastleDynamicProxyInterceptionValidator : IMethodInterceptionValidator
    {
        private readonly MethodInfoManager methodInfoManager;

        public CastleDynamicProxyInterceptionValidator(MethodInfoManager methodInfoManager)
        {
            this.methodInfoManager = methodInfoManager;
        }

        public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object callTarget, out string failReason)
        {
            var invokedMethod = this.GetInvokedMethod(method, callTarget);

            failReason = GetReasonForWhyMethodCanNotBeIntercepted(invokedMethod);
            return failReason is null;
        }

        private static string GetReasonForWhyMethodCanNotBeIntercepted(MethodInfo method)
        {
            if (Castle.DynamicProxy.ProxyUtil.IsProxyType(method.DeclaringType))
            {
                return null;
            }

            if (method.IsFinal)
            {
#if FEATURE_STRING_CONTAINS_COMPARISONTYPE
                var explicitImplementation = method.Name.Contains('.', StringComparison.Ordinal);
#else
                var explicitImplementation = method.Name.Contains('.');
#endif
                if (!explicitImplementation)
                {
                    return "Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.";
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
                return "Non-virtual members can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.";
            }

            if (!Castle.DynamicProxy.ProxyUtil.IsAccessible(method, out var message))
            {
                return message;
            }

            return null;
        }

        private MethodInfo GetInvokedMethod(MethodInfo method, object callTarget)
        {
            if (callTarget is object)
            {
                return this.methodInfoManager.GetMethodOnTypeThatWillBeInvokedByMethodInfo(callTarget.GetType(), method) !;
            }

            return method;
        }
    }
}
