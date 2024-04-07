namespace FakeItEasy.Expressions
{
    using System;
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
        /// <param name="callTarget">The object on which the method was called.</param>
        /// <param name="method">The method to describe.</param>
        /// <param name="argumentConstraints">The argument constraints applied to the method.</param>
        public static void DescribeCallOn(IOutputWriter writer, object? callTarget, MethodInfo method, IEnumerable<IArgumentConstraint> argumentConstraints)
        {
            WriteObjectType(writer, method.DeclaringType!);
            WriteMethodName(writer, method);

            WriteArgumentsListString(writer, method, argumentConstraints);

            WriteObjectName(writer, callTarget);
        }

        private static void WriteObjectType(IOutputWriter writer, Type type)
        {
            writer.Write(type.ToString());
            writer.Write(".");
        }

        private static void WriteMethodName(IOutputWriter writer, MethodInfo method)
        {
            writer.Write(method.IsPropertyGetterOrSetter() ? method.Name.Substring(4) : method.Name);
            writer.Write(method.GetGenericArgumentsString());
        }

        private static List<IArgumentConstraint> GetArgumentConstraintsForArgumentsList(MethodInfo method, IEnumerable<IArgumentConstraint> argumentConstraints)
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
            if (constraints.Count != 0 || !method.IsPropertyGetterOrSetter())
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

                    var parameter = parameters[index++];

                    // Incremented index above to get 1-based parameter name output.
                    // Usually parameters will be named, but in F# (at least) it's possible
                    // to declare a method with anonymous parameters. In that case, we try to
                    // help the user by outputting names like "param1", "param2", …
                    string parameterName = parameter.Name ?? $"param{index}";
                    writer.Write(parameterName).Write(": ");
                    constraint.WriteDescription(writer);
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

        private static void WriteObjectName(IOutputWriter writer, object? callTarget)
        {
            if (callTarget == null)
            {
                return;
            }

            Fake.TryGetFakeManager(callTarget!, out var fake);

            var objectName = fake?.FakeObjectName;

            if (string.IsNullOrEmpty(objectName))
            {
                return;
            }

            writer.Write($" on {objectName}");
        }
    }
}
