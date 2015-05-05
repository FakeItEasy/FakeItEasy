namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    
    /// <summary>
    /// Handles comparisons of instances of <see cref="MethodInfo"/>.
    /// </summary>
    internal class MethodInfoManager
    {
        private static readonly Dictionary<TypeMethodInfoPair, MethodInfo> MethodCache = new Dictionary<TypeMethodInfoPair, MethodInfo>();

        /// <summary>
        /// Gets a value indicating whether the two instances of <see cref="MethodInfo"/> would invoke the same method
        /// if invoked on an instance of the target type.
        /// </summary>
        /// <param name="target">The type of target for invocation.</param>
        /// <param name="first">The first <see cref="MethodInfo"/>.</param>
        /// <param name="second">The second <see cref="MethodInfo"/>.</param>
        /// <returns>True if the same method would be invoked.</returns>
        public virtual bool WillInvokeSameMethodOnTarget(Type target, MethodInfo first, MethodInfo second)
        {
            if (first == second)
            {
                return true;
            }

            var methodInvokedByFirst = this.GetMethodOnTypeThatWillBeInvokedByMethodInfo(target, first);
            var methodInvokedBySecond = this.GetMethodOnTypeThatWillBeInvokedByMethodInfo(target, second);

            return methodInvokedByFirst != null && methodInvokedBySecond != null && methodInvokedByFirst.Equals(methodInvokedBySecond);
        }

        public virtual MethodInfo GetMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            MethodInfo result = null;
            var key = new TypeMethodInfoPair { Type = type, MethodInfo = method };

            lock (MethodCache)
            {
                if (!MethodCache.TryGetValue(key, out result))
                {
                    result = FindMethodOnTypeThatWillBeInvokedByMethodInfo(type, method);
                    MethodCache.Add(key, result);
                }
            }

            return result;
        }

        private static bool HasSameBaseMethod(MethodInfo first, MethodInfo second)
        {
            var baseOfFirst = GetBaseDefinition(first);
            var baseOfSecond = GetBaseDefinition(second);

            return IsSameMethod(baseOfFirst, baseOfSecond);
        }

        private static MethodInfo GetBaseDefinition(MethodInfo method)
        {
            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                method = method.GetGenericMethodDefinition();
            }

            return method.GetBaseDefinition();
        }

        private static bool IsSameMethod(MethodInfo first, MethodInfo second)
        {
            return first.DeclaringType == second.DeclaringType
                   && first.MetadataToken == second.MetadataToken
                   && first.Module == second.Module
                   && first.GetGenericArguments().SequenceEqual(second.GetGenericArguments());
        }

        private static MethodInfo FindMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            var result =
                (from typeMethod in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                 where HasSameBaseMethod(typeMethod, method)
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
            var reflectedType = method.ReflectedType;

            if (reflectedType.IsInterface)
            {
                return null;
            }

            var allInterfaces =
                from i in type.GetInterfaces()
                where TypeImplementsInterface(reflectedType, i)
                select i;

            foreach (var interfaceType in allInterfaces)
            {
                var interfaceMap = reflectedType.GetInterfaceMap(interfaceType);

                var foundMethod =
                    (from methodTargetPair in interfaceMap.InterfaceMethods.Zip(interfaceMap.TargetMethods)
                     where HasSameBaseMethod(EnsureNonGeneric(method), EnsureNonGeneric(methodTargetPair.Item2))
                     select MakeGeneric(methodTargetPair.Item1, method)).FirstOrDefault();

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
                 where HasSameBaseMethod(EnsureNonGeneric(methodTargetPair.Item1), EnsureNonGeneric(method))
                 select MakeGeneric(methodTargetPair.Item2, method)).First();
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
            public MethodInfo MethodInfo;
            public Type Type;

            public override int GetHashCode()
            {
                return this.Type.GetHashCode() ^ this.MethodInfo.GetHashCode();
            }

            [SuppressMessage("Microsoft.Usage", "CA2231:OverloadOperatorEqualsOnOverridingValueTypeEquals", Justification = "The type is used privately only.")]
            public override bool Equals(object obj)
            {
                var other = (TypeMethodInfoPair)obj;

                return this.Type.Equals(other.Type) && this.MethodInfo == other.MethodInfo;
            }
        }
    }
}
