namespace FakeItEasy.Core
{
    using System;
    using System.Linq;
    using System.Text;
    
    internal class DefaultFakeObjectCallFormatter
            : IFakeObjectCallFormatter
    {
        public string GetDescription(IFakeObjectCall call)
        {
            var builder = new StringBuilder();

            builder
                .Append(call.FakedObject.GetType().FullName)
                .Append(".")
                .Append(call.Method.Name)
                .Append("(");

            AppendArguments(builder, call);

            builder.Append(")");

            return builder.ToString();
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

        private static void AppendArgumentValue(StringBuilder builder, ArgumentValueInfo argument)
        {
            builder
                .Append(argument.ArgumentName)
                .Append(": ")
                .Append(argument.ArgumentType.FullName)
                .Append(" = ")
                .Append(GetArgumentValueAsString(argument.ArgumentValue));
        }

        private static ArgumentValueInfo[] GetArgumentValueInfos(IFakeObjectCall call)
        {
            return
                (from argument in call.Method.GetParameters().Zip(call.Arguments)
                 select new ArgumentValueInfo
                 {
                     ArgumentName = argument.First.Name,
                     ArgumentType = argument.First.ParameterType,
                     ArgumentValue = argument.Second
                 }).ToArray();
        }

        private static string GetArgumentValueAsString(object argumentValue)
        {
            if (argumentValue == null)
            {
                return "<NULL>";
            }

            var stringValue = argumentValue as string;

            if (stringValue != null)
            {
                if (stringValue.Length == 0)
                {
                    return "string.Empty";
                }

                stringValue = string.Concat("\"", stringValue, "\"");
            }
            else
            {
                stringValue = argumentValue.ToString();
            }

            if (stringValue.Length > 40)
            {
                stringValue = stringValue.Substring(0, 40) + "...";
            }

            return stringValue;
        }

        private static void AppendArguments(StringBuilder builder, IFakeObjectCall call)
        {
            var arguments = GetArgumentValueInfos(call);

            for (int i = 0; i < arguments.Length; i++)
            {
                var argument = arguments[i];

                AppendArgumentSeparator(builder, i, arguments.Length);
                AppendArgumentValue(builder, argument);
            }
        }

        private struct ArgumentValueInfo
        {
            public object ArgumentValue { get; set; }
            
            public Type ArgumentType { get; set; }
            
            public string ArgumentName { get; set; }
        }
    }
}
