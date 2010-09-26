namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Handles comparisons of MethodInfos.
    /// </summary>
    internal class MethodInfoManager
    {
        private static Dictionary<TypeMethodInfoPair, MethodInfo> methodCache = new Dictionary<TypeMethodInfoPair, MethodInfo>();

        /// <summary>
        /// Gets a value indicating if the two method infos would invoke the same method
        /// if invoked on an instance of the target type.
        /// </summary>
        /// <param name="target">The type of target for invokation.</param>
        /// <param name="first">The first MethodInfo.</param>
        /// <param name="second">The second MethodInfo.</param>
        /// <returns>True if the same method would be invoked.</returns>
        public virtual bool WillInvokeSameMethodOnTarget(Type target, MethodInfo first, MethodInfo second)
        {
            if (first.Equals(second))
            {
                return true;
            }

            var methodInvokedByFirst = GetMethodOnTypeThatWillBeInvokedByMethodInfo(target, first);
            var methodInvokedBySecond = GetMethodOnTypeThatWillBeInvokedByMethodInfo(target, second);

            return methodInvokedByFirst != null && methodInvokedBySecond != null && methodInvokedByFirst.Equals(methodInvokedBySecond);
        }

        [DebuggerStepThrough]
        private static bool IsSameMethod(MethodInfo first, MethodInfo second)
        {
            return first.GetBaseDefinition().Equals(second.GetBaseDefinition())
                && first.GetGenericArguments().SequenceEqual(second.GetGenericArguments());
        }

        private static MethodInfo GetMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            MethodInfo result = null;
            var key = new TypeMethodInfoPair { Type = type, MethodInfo = method };

            lock (methodCache)
            {
                if (!methodCache.TryGetValue(key, out result))
                {
                    result = FindMethodOnTypeThatWillBeInvokedByMethodInfo(type, method);
                    methodCache.Add(key, result);
                }
            }

            return result;
        }

        private static MethodInfo FindMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            MethodInfo result =
                (from typeMethod in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                 where IsSameMethod(typeMethod, method)
                 select MakeGeneric(typeMethod, method)).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            result = GetMethodOnTypeThatImplementsInterfaceMethod(type, method);

            if (result != null)
            {
                return result;
            }

            return GetMethodOnInterfaceTypeImplementedByMethod(type, method);
        }

        private static MethodInfo GetMethodOnInterfaceTypeImplementedByMethod(Type type, MethodInfo method)
        {
            var allInterfaces =
                from i in type.GetInterfaces()
                where TypeImplementsInterface(method.ReflectedType, i)
                select i;

            foreach (var interfaceType in allInterfaces)
            {
                var interfaceMap = method.ReflectedType.GetInterfaceMap(interfaceType);

                var foundMethod =
                    (from methodTargetPair in interfaceMap.InterfaceMethods.Zip(interfaceMap.TargetMethods)
                     where IsSameMethod(EnsureNonGeneric(method), EnsureNonGeneric(methodTargetPair.Value2))
                     select MakeGeneric(methodTargetPair.Value1, method)).FirstOrDefault();

                if (foundMethod != null)
                {
                    return GetMethodOnTypeThatImplementsInterfaceMethod(type, foundMethod);
                }
            }

            return null;
        }

        private static MethodInfo GetMethodOnTypeThatImplementsInterfaceMethod(Type type, MethodInfo method)
        {
            var baseDefinition = method.GetBaseDefinition();

            if (!baseDefinition.DeclaringType.IsInterface || !TypeImplementsInterface(type, baseDefinition.DeclaringType))
            {
                return null;
            }

            var interfaceMap = type.GetInterfaceMap(baseDefinition.DeclaringType);

            return
                (from methodTargetPair in interfaceMap.InterfaceMethods.Zip(interfaceMap.TargetMethods)
                 where IsSameMethod(EnsureNonGeneric(methodTargetPair.Value1), EnsureNonGeneric(method))
                 select MakeGeneric(methodTargetPair.Value2, method)).First();
        }

        private static MethodInfo EnsureNonGeneric(MethodInfo methodInfo)
        {
            return methodInfo.IsGenericMethod ? methodInfo.GetGenericMethodDefinition() : methodInfo;
        }

        private static MethodInfo MakeGeneric(MethodInfo methodToMakeGeneric, MethodInfo originalMethod)
        {
            if (!methodToMakeGeneric.IsGenericMethodDefinition)
            {
                return methodToMakeGeneric;
            }

            return methodToMakeGeneric.MakeGenericMethod(originalMethod.GetGenericArguments());
        }

        private static bool TypeImplementsInterface(Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(x => x.Equals(interfaceType));
        }

        private struct TypeMethodInfoPair
        {
            public Type Type;
            public MethodInfo MethodInfo;

            public override int GetHashCode()
            {
                return this.Type.GetHashCode() ^ this.MethodInfo.GetHashCode();
            }

            [SuppressMessage("Microsoft.Usage", "CA2231:OverloadOperatorEqualsOnOverridingValueTypeEquals", Justification = "The type is used privately only.")]
            public override bool Equals(object obj)
            {
                var other = (TypeMethodInfoPair)obj;

                return this.Type.Equals(other.Type) && this.MethodInfo.Equals(other.MethodInfo);
            }
        }
    }
}
