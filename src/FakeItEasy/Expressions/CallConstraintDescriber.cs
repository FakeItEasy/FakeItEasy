namespace FakeItEasy.Expressions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy.Core;

    /// <summary>
    /// Describes a call constraint, as defined by a method and list of argument constraints.
    /// </summary>
    internal static class CallConstraintDescriber
    {
        /// <summary>
        /// Writes a human readable description of the call constraint
        /// matcher to the supplied writer.
        /// </summary>
        /// <param name="writer">The writer on which to describe the call.</param>
        /// <param name="method">The method to describe.</param>
        /// <param name="argumentConstraints">The argument constraints applied to the method.</param>
        public static void DescribeCallOn(IOutputWriter writer, MethodInfo method, IEnumerable<IArgumentConstraint> argumentConstraints)
        {
            writer.Write(method.DeclaringType);
            writer.Write(".");
            WriteMethodName(writer, method);

            WriteArgumentsListString(writer, method, argumentConstraints);
        }

        private static void WriteMethodName(IOutputWriter writer, MethodInfo method)
        {
            writer.Write(method.IsPropertyGetterOrSetter() ? method.Name.Substring(4) : method.Name);
            writer.Write(method.GetGenericArgumentsString());
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

        private static void WriteArgumentListPrefix(IOutputWriter writer, MethodInfo method) =>
            writer.Write(method.IsPropertyGetterOrSetter() ? '[' : '(');

        private static void WriteArgumentListSuffix(IOutputWriter writer, MethodInfo method) =>
            writer.Write(method.IsPropertyGetterOrSetter() ? ']' : ')');

        private static void WriteArgumentsListString(IOutputWriter writer, MethodInfo method, IEnumerable<IArgumentConstraint> argumentConstraints)
        {
            var constraints = GetArgumentConstraintsForArgumentsList(method, argumentConstraints);
            if (constraints.Any() || !method.IsPropertyGetterOrSetter())
            {
                WriteArgumentListPrefix(writer, method);
                int index = 0;
                var parameters = method.GetParameters();

                foreach (var constraint in constraints)
                {
                    if (index > 0)
                    {
                        writer.Write(", ");
                    }

                    var parameter = parameters[index];
                    writer.Write(parameter.Name).Write(": ");
                    constraint.WriteDescription(writer);
                    index++;
                }

                WriteArgumentListSuffix(writer, method);
            }

            if (method.IsPropertySetter())
            {
                writer.Write(" = ");
                var valueConstraint = argumentConstraints.Last();
                valueConstraint.WriteDescription(writer);
            }
        }
    }
}
