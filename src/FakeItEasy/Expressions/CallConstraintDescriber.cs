namespace FakeItEasy.Expressions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Core;

    /// <summary>
    /// Describes a call constraint, as defined by a method and list of argument constraints.
    /// </summary>
    internal class CallConstraintDescriber
    {
        private readonly StringBuilderOutputWriter.Factory outputWriterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallConstraintDescriber" /> class.
        /// </summary>
        /// <param name="outputWriterFactory">The output writer factory to use.</param>
        public CallConstraintDescriber(StringBuilderOutputWriter.Factory outputWriterFactory)
        {
            this.outputWriterFactory = outputWriterFactory;
        }

        /// <summary>
        /// Gets a human readable description of the call constraint
        /// matcher.
        /// </summary>
        /// <param name="method">The method to describe.</param>
        /// <param name="argumentConstraints">The argument constraints applied to the method.</param>
        /// <returns>A human readable description of the call constraint.</returns>
        public string GetDescriptionOfMatchingCall(MethodInfo method, IEnumerable<IArgumentConstraint> argumentConstraints)
        {
            var result = new StringBuilder();

            result.Append(method.DeclaringType);
            result.Append(".");
            AppendMethodName(result, method);

            this.AppendArgumentsListString(result, method, argumentConstraints);

            return result.ToString();
        }

        private static void AppendMethodName(StringBuilder result, MethodInfo method)
        {
            result.Append(method.IsPropertyGetterOrSetter() ? method.Name.Substring(4) : method.Name);
            result.Append(method.GetGenericArgumentsString());
        }

        private static IList<IArgumentConstraint> GetArgumentConstraintsForArgumentsList(MethodInfo method, IEnumerable<IArgumentConstraint> argumentConstraints)
        {
            var list = argumentConstraints.ToList();
            if (method.IsPropertySetter())
            {
                list.RemoveAt(list.Count - 1);
            }

            return list;
        }

        private static void AppendArgumentListPrefix(StringBuilder builder, MethodInfo method) =>
            builder.Append(method.IsPropertyGetterOrSetter() ? '[' : '(');

        private static void AppendArgumentListSuffix(StringBuilder builder, MethodInfo method) =>
            builder.Append(method.IsPropertyGetterOrSetter() ? ']' : ')');

        private void AppendArgumentsListString(StringBuilder result, MethodInfo method, IEnumerable<IArgumentConstraint> argumentConstraints)
        {
            var constraints = GetArgumentConstraintsForArgumentsList(method, argumentConstraints);
            var stringBuilderOutputWriter = this.outputWriterFactory(result);
            if (constraints.Any() || !method.IsPropertyGetterOrSetter())
            {
                AppendArgumentListPrefix(result, method);
                int index = 0;
                var parameters = method.GetParameters();

                foreach (var constraint in constraints)
                {
                    if (index > 0)
                    {
                        result.Append(", ");
                    }

                    var parameter = parameters[index];
                    result.Append(parameter.Name + ": ");
                    constraint.WriteDescription(stringBuilderOutputWriter);
                    index++;
                }

                AppendArgumentListSuffix(result, method);
            }

            if (method.IsPropertySetter())
            {
                result.Append(" = ");
                var valueConstraint = argumentConstraints.Last();
                valueConstraint.WriteDescription(stringBuilderOutputWriter);
            }
        }
    }
}
