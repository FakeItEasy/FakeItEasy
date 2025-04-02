namespace FakeItEasy.Creation.CastleDynamicProxy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    internal class CastleDynamicProxyInterceptionValidator : IMethodInterceptionValidator
    {
        private const string NonVirtualMethodsCantBeIntercepted =
            "Non-virtual or sealed members can not be intercepted. Only interface members and non-sealed virtual, overriding, and abstract members can be intercepted.";

        private readonly MethodInfoManager methodInfoManager;

        public CastleDynamicProxyInterceptionValidator(MethodInfoManager methodInfoManager)
        {
            this.methodInfoManager = methodInfoManager;
        }

        public bool MethodCanBeInterceptedOnInstance(MethodInfo method, object? callTarget, [NotNullWhen(false)] out string? failReason)
        {
            if (method.DeclaringType!.IsInterface && !method.IsVirtual)
            {
                failReason = NonVirtualMethodsCantBeIntercepted;
                return false;
            }

            var invokedMethod = this.GetInvokedMethod(method, callTarget);

            failReason = GetReasonForWhyMethodCanNotBeIntercepted(invokedMethod);
            return failReason is null;
        }

        private static string? GetReasonForWhyMethodCanNotBeIntercepted(MethodInfo method)
        {
            if (Castle.DynamicProxy.ProxyUtil.IsProxyType(method.DeclaringType))
            {
                return null;
            }

            if (method.IsFinal)
            {
#if LACKS_STRING_CONTAINS_COMPARISONTYPE
                var explicitImplementation = method.Name.Contains('.');
#else
                var explicitImplementation = method.Name.Contains('.', StringComparison.Ordinal);
#endif
                if (explicitImplementation)
                {
                    return "The base type implements this interface method explicitly. In order to be able to intercept this method, the fake must specify that it implements this interface in the fake creation options.";
                }
                else
                {
                    return NonVirtualMethodsCantBeIntercepted;
                }
            }

            if (method.IsStatic)
            {
                if (method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false).Length == 0)
                {
                    return "Static methods can not be intercepted.";
                }
                else
                {
                    return "Extension methods can not be intercepted since they're static.";
                }
            }

            if (!method.IsVirtual)
            {
                return NonVirtualMethodsCantBeIntercepted;
            }

            if (!Castle.DynamicProxy.ProxyUtil.IsAccessible(method, out var message))
            {
                return message;
            }

            return null;
        }

        private MethodInfo GetInvokedMethod(MethodInfo method, object? callTarget)
        {
            if (callTarget is not null)
            {
                return this.methodInfoManager.GetMethodOnTypeThatWillBeInvokedByMethodInfo(callTarget.GetType(), method)!;
            }

            return method;
        }
    }
}
