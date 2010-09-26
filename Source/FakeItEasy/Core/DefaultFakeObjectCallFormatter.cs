namespace FakeItEasy.Core
{
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The default implementation of the IFakeObjectCallFormatter interface.
    /// </summary>
    internal class DefaultFakeObjectCallFormatter
        : IFakeObjectCallFormatter
    {
        private ArgumentValueFormatter argumentValueFormatter;
        
        public DefaultFakeObjectCallFormatter(ArgumentValueFormatter argumentValueFormatter)
        {
            this.argumentValueFormatter = argumentValueFormatter;
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
                .Append(Fake.GetFakeManager(call.FakedObject).FakeObjectType)
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

        private void AppendArgumentValue(StringBuilder builder, ArgumentValueInfo argument)
        {
            builder
                .Append(argument.ArgumentName)
                .Append(": ")
                .Append(this.GetArgumentValueAsString(argument.ArgumentValue));
        }

        private static ArgumentValueInfo[] GetArgumentValueInfos(IFakeObjectCall call)
        {
            return
                (from argument in call.Method.GetParameters().Zip(call.Arguments)
                 select new ArgumentValueInfo
                 {
                     ArgumentName = argument.Value1.Name,
                     ArgumentValue = argument.Value2
                 }).ToArray();
        }

        private string GetArgumentValueAsString(object argumentValue)
        {
            return this.argumentValueFormatter.GetArgumentValueAsString(argumentValue);
        }

        private void AppendArguments(StringBuilder builder, IFakeObjectCall call)
        {
            var arguments = GetArgumentValueInfos(call);

            for (int i = 0; i < arguments.Length; i++)
            {
                var argument = arguments[i];

                AppendArgumentSeparator(builder, i, arguments.Length);
                this.AppendArgumentValue(builder, argument);
            }
        }

        private struct ArgumentValueInfo
        {
            public object ArgumentValue { get; set; }
            
            public string ArgumentName { get; set; }
        }
    }
}