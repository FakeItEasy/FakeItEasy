namespace FakeItEasy
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Provides extension methods for <see cref="MethodInfo"/>.
    /// </summary>
    internal static class MethodInfoExtensions
    {
        private static readonly ConcurrentDictionary<MethodInfo, ObjectMethod> ObjectMethodMap = new();

        public static string GetGenericArgumentsString(this MethodInfo method)
        {
            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length == 0)
            {
                return string.Empty;
            }

            return string.Concat(
                "`",
                genericArguments.Length,
                "[",
                string.Join(",", genericArguments.AsEnumerable()),
                "]");
        }

        public static bool IsPropertyGetterOrSetter(this MethodInfo method)
        {
            return method.IsPropertyGetter() || method.IsPropertySetter();
        }

        public static bool IsPropertySetter(this MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
        }

        public static bool IsPropertyGetter(this MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
        }

        public static string GetDescription(this MethodInfo method)
        {
            var builder = new StringBuilder();

            builder
                .Append(method.DeclaringType)
                .Append('.');

            AppendMethodName(builder, method);

            AppendParameterList(builder, method);

            return builder.ToString();
        }

        public static bool HasSameBaseMethodAs(this MethodInfo first, MethodInfo second)
        {
            var baseOfFirst = GetBaseDefinition(first);
            var baseOfSecond = GetBaseDefinition(second);

            return baseOfFirst.IsSameMethodAs(baseOfSecond);
        }

        public static ObjectMethod GetObjectMethod(this MethodInfo method)
        {
            return ObjectMethodMap.GetOrAdd(method, CalculateObjectMethod);

            static ObjectMethod CalculateObjectMethod(MethodInfo method)
            {
                var baseDefinition = GetBaseDefinition(method);
                if (IsSameMethodAs(baseDefinition, ObjectMembers.EqualsMethod))
                {
                    return ObjectMethod.EqualsMethod;
                }

                if (IsSameMethodAs(baseDefinition, ObjectMembers.ToStringMethod))
                {
                    return ObjectMethod.ToStringMethod;
                }

                if (IsSameMethodAs(baseDefinition, ObjectMembers.GetHashCodeMethod))
                {
                    return ObjectMethod.GetHashCodeMethod;
                }

                return ObjectMethod.None;
            }
        }

        private static bool IsSameMethodAs(this MethodInfo method, MethodInfo otherMethod)
        {
            return method.DeclaringType == otherMethod.DeclaringType
                && method.MetadataToken == otherMethod.MetadataToken
                && method.Module == otherMethod.Module
                && method.GetGenericArguments().SequenceEqual(otherMethod.GetGenericArguments());
        }

        private static MethodInfo GetBaseDefinition(MethodInfo method)
        {
            if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
            {
                method = method.GetGenericMethodDefinition();
            }

            return method.GetBaseDefinition();
        }

        private static void AppendMethodName(StringBuilder builder, MethodInfo method)
        {
            if (IsPropertyGetterOrSetter(method))
            {
#pragma warning disable CA1846 // Prefer 'AsSpan' over 'Substring'
                builder.Append(method.Name.Substring(4));
#pragma warning restore CA1846 // Prefer 'AsSpan' over 'Substring'
            }
            else
            {
                builder.Append(method.Name);
            }

            builder.Append(method.GetGenericArgumentsString());
        }

        private static void AppendParameterList(StringBuilder builder, MethodInfo method)
        {
            var parameters = method.GetParameters();

            if (parameters.Length > 0 || !method.IsPropertyGetterOrSetter())
            {
                AppendParameterListPrefix(builder, method);

                AppendParameters(builder, parameters);

                AppendParameterListSuffix(builder, method);
            }
        }

        private static void AppendParameterListPrefix(StringBuilder builder, MethodInfo method)
        {
            if (method.IsPropertyGetterOrSetter())
            {
                builder.Append('[');
            }
            else
            {
                builder.Append('(');
            }
        }

        private static void AppendParameterListSuffix(StringBuilder builder, MethodInfo method)
        {
            if (method.IsPropertyGetterOrSetter())
            {
                builder.Append(']');
            }
            else
            {
                builder.Append(')');
            }
        }

        private static void AppendParameters(StringBuilder builder, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                AppendParameterSeparator(builder, i);
                builder.Append(parameters[i].ParameterType);

                var parameterName = parameters[i].Name;
                if (parameterName is not null)
                {
                    builder
                       .Append(' ')
                       .Append(parameterName);
                }
            }
        }

        private static void AppendParameterSeparator(StringBuilder builder, int argumentIndex)
        {
            if (argumentIndex > 0)
            {
                builder.Append(", ");
            }
        }
    }
}
