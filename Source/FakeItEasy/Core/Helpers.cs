namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class Helpers
    {
        public static void WriteCalls(this IEnumerable<IFakeObjectCall> calls, TextWriter writer)
        {
            var callWriter = ServiceLocator.Current.Resolve<CallWriter>();
            callWriter.WriteCalls(0, calls, writer);
        }

        public static string GetDescription(this IFakeObjectCall fakeObjectCall)
        {
            var method = fakeObjectCall.Method;

            return "{0}.{1}({2})".FormatInvariant(method.DeclaringType.FullName, method.Name, GetParametersString(fakeObjectCall));
        }

        private static string GetParametersString(IFakeObjectCall fakeObjectCall)
        {
            var result = new StringBuilder();

            using (var parameters = fakeObjectCall.Method.GetParameters().Cast<ParameterInfo>().GetEnumerator())
            using (var arguments = fakeObjectCall.Arguments.AsEnumerable().GetEnumerator())
            {
                while (parameters.MoveNext() && arguments.MoveNext())
                {
                    if (result.Length > 0)
                    {
                        result.Append(", ");
                    }

                    result.AppendFormat("{0}", GetArgumentAsString(arguments.Current));
                }
            }

            return result.ToString();
        }

        private static string GetArgumentAsString(object argument)
        {
            if (argument == null)
            {
                return "<NULL>";
            }

            string stringArgument = argument as string;
            if (stringArgument != null)
            {
                return stringArgument.Length > 0 ? string.Concat("\"", stringArgument, "\"") : "<string.Empty>";
            }

            return argument.ToString();
        }

        [DebuggerStepThrough]
        public static object GetDefaultValueOfType(Type type)
        {
            return type.IsValueType && !type.Equals(typeof(void)) ? Activator.CreateInstance(type) : null;
        }
    }
}
