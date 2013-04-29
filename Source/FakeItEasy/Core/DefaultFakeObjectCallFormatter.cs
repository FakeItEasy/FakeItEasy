namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// The default implementation of the IFakeObjectCallFormatter interface.
    /// </summary>
    internal class DefaultFakeObjectCallFormatter
        : IFakeObjectCallFormatter
    {
        private readonly ArgumentValueFormatter argumentValueFormatter;
        private readonly IFakeManagerAccessor fakeManagerAccessor;

        public DefaultFakeObjectCallFormatter(ArgumentValueFormatter argumentValueFormatter, IFakeManagerAccessor fakeManagerAccessor)
        {
            this.argumentValueFormatter = argumentValueFormatter;
            this.fakeManagerAccessor = fakeManagerAccessor;
        }

        /// <summary>
        /// Gets a human readable description of the specified
        /// fake object call.
        /// </summary>
        /// <param name="call">The call to get a description for.</param>
        /// <returns>A description of the call.</returns>
        public string GetDescription(IFakeObjectCall call)
        {
            var builder = new StringBuilder();

            builder
                .Append(this.fakeManagerAccessor.GetFakeManager(call.FakedObject).FakeObjectType)
                .Append(".");

            AppendMethodName(builder, call.Method);

            this.AppendArgumentsList(builder, call);

            return builder.ToString();
        }

        private static ArgumentValueInfo[] GetArgumentsForArgumentsList(ArgumentValueInfo[] allArguments, MethodInfo method)
        {
            if (IsPropertySetter(method))
            {
                return allArguments.Take(allArguments.Length - 1).ToArray();
            }

            return allArguments;
        }

        private static void AppendArgumentListPrefix(StringBuilder builder, MethodInfo method)
        {
            if (IsPropertyGetterOrSetter(method))
            {
                builder.Append("[");
            }
            else
            {
                builder.Append("(");
            }
        }

        private static void AppendArgumentListSuffix(StringBuilder builder, MethodInfo method)
        {
            if (IsPropertyGetterOrSetter(method))
            {
                builder.Append("]");
            }
            else
            {
                builder.Append(")");
            }
        }

        private static void AppendMethodName(StringBuilder builder, MethodInfo method)
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

        private static bool IsPropertyGetterOrSetter(MethodInfo method)
        {
            return IsPropertyGetter(method) || IsPropertySetter(method);
        }

        private static bool IsPropertySetter(MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
        }

        private static bool IsPropertyGetter(MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
        }

        private static void AppendArgumentSeparator(StringBuilder builder, int argumentIndex, int totalNumberOfArguments)
        {
            if (totalNumberOfArguments > 2)
            {
                if (argumentIndex > 0)
                {
                    builder.Append(",");
                }

                builder.AppendLine();
                builder.Append("    ");
                return;
            }

            if (argumentIndex > 0)
            {
                builder.Append(", ");
            }
        }

        private static ArgumentValueInfo[] GetArgumentValueInfos(IFakeObjectCall call)
        {
            return
                (from argument in call.Method.GetParameters().Zip(call.Arguments)
                 select new ArgumentValueInfo
                            {
                                ArgumentName = argument.Item1.Name,
                                ArgumentValue = argument.Item2
                            }).ToArray();
        }

        private void AppendArgumentsList(StringBuilder builder, IFakeObjectCall call)
        {
            var allArguments = GetArgumentValueInfos(call);
            var argumentsForArgumentList = GetArgumentsForArgumentsList(allArguments, call.Method);

            if (argumentsForArgumentList.Length > 0 || !IsPropertyGetterOrSetter(call.Method))
            {
                AppendArgumentListPrefix(builder, call.Method);

                this.AppendArguments(builder, argumentsForArgumentList);

                AppendArgumentListSuffix(builder, call.Method);
            }

            if (IsPropertySetter(call.Method))
            {
                builder.Append(" = ");
                builder.Append(this.argumentValueFormatter.GetArgumentValueAsString(allArguments[allArguments.Length - 1].ArgumentValue));
            }
        }

        private void AppendArgumentValue(StringBuilder builder, ArgumentValueInfo argument)
        {
            builder
                .Append(argument.ArgumentName)
                .Append(": ")
                .Append(this.GetArgumentValueAsString(argument.ArgumentValue));
        }

        private string GetArgumentValueAsString(object argumentValue)
        {
            return this.argumentValueFormatter.GetArgumentValueAsString(argumentValue);
        }

        private void AppendArguments(StringBuilder builder, IEnumerable<ArgumentValueInfo> arguments)
        {
            var totalNumberOfArguments = arguments.Count();
            var callIndex = 0;
            foreach (var argument in arguments)
            {
                AppendArgumentSeparator(builder, callIndex, totalNumberOfArguments);
                this.AppendArgumentValue(builder, argument);
                callIndex++;
            }
        }

        private struct ArgumentValueInfo
        {
            public object ArgumentValue { get; set; }

            public string ArgumentName { get; set; }
        }
    }
}