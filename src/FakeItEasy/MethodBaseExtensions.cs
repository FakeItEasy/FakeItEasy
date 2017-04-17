namespace FakeItEasy
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Provides extension methods for <see cref="MethodBase"/>.
    /// </summary>
    internal static class MethodBaseExtensions
    {
        public static string GetGenericArgumentsCSharp(this MethodBase method)
        {
            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length == 0)
            {
                return string.Empty;
            }

            return string.Concat("<", string.Join(", ", genericArguments.Select(type => type.FullNameCSharp()).ToArray()), ">");
        }

        public static bool IsPropertyGetterOrSetter(this MethodBase method)
        {
            return method.IsPropertyGetter() || method.IsPropertySetter();
        }

        public static bool IsPropertySetter(this MethodBase method)
        {
            return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
        }

        public static bool IsPropertyGetter(this MethodBase method)
        {
            return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
        }

        public static string GetDescription(this MethodBase method)
        {
            var builder = new StringBuilder();

            builder
                .Append(method.DeclaringType.FullNameCSharp())
                .Append(".");

            AppendMethodName(builder, method);

            AppendParameterList(builder, method);

            return builder.ToString();
        }

        private static void AppendMethodName(StringBuilder builder, MethodBase method)
        {
            if (IsPropertyGetterOrSetter(method))
            {
                builder.Append(method.Name.Substring(4));
            }
            else
            {
                builder.Append(method.Name);
            }

            builder.Append(method.GetGenericArgumentsCSharp());
        }

        private static void AppendParameterList(StringBuilder builder, MethodBase method)
        {
            var parameters = method.GetParameters();

            if (parameters.Length > 0 || !method.IsPropertyGetterOrSetter())
            {
                AppendParameterListPrefix(builder, method);

                AppendParameters(builder, parameters);

                AppendParameterListSuffix(builder, method);
            }
        }

        private static void AppendParameterListPrefix(StringBuilder builder, MethodBase method)
        {
            if (method.IsPropertyGetterOrSetter())
            {
                builder.Append("[");
            }
            else
            {
                builder.Append("(");
            }
        }

        private static void AppendParameterListSuffix(StringBuilder builder, MethodBase method)
        {
            if (method.IsPropertyGetterOrSetter())
            {
                builder.Append("]");
            }
            else
            {
                builder.Append(")");
            }
        }

        private static void AppendParameters(StringBuilder builder, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                AppendParameterSeparator(builder, i);
                builder
                    .Append(parameters[i].ParameterType.FullNameCSharp())
                    .Append(" ")
                    .Append(parameters[i].Name);
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
