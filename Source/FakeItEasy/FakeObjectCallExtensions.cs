namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FakeItEasy.Core;

    /// <summary>
    /// Provides extension methods for <see cref="IFakeObjectCall"/>.
    /// </summary>
    public static class FakeObjectCallExtensions
    {
        /// <summary>
        /// Gets the argument at the specified index in the arguments collection
        /// for the call.
        /// </summary>
        /// <typeparam name="T">The type of the argument to get.</typeparam>
        /// <param name="call">The call to get the argument from.</param>
        /// <param name="argumentIndex">The index of the argument.</param>
        /// <returns>The value of the argument with the specified index.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generic argument is used to cast the result.")]
        public static T GetArgument<T>(this IFakeObjectCall call, int argumentIndex)
        {
            Guard.AgainstNull(call, "call");

            return call.Arguments.Get<T>(argumentIndex);
        }

        /// <summary>
        /// Gets the argument with the specified name in the arguments collection
        /// for the call.
        /// </summary>
        /// <typeparam name="T">The type of the argument to get.</typeparam>
        /// <param name="call">The call to get the argument from.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <returns>The value of the argument with the specified name.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generic argument is used to cast the result.")]
        public static T GetArgument<T>(this IFakeObjectCall call, string argumentName)
        {
            Guard.AgainstNull(call, "call");
            Guard.AgainstNull(argumentName, "argumentName");

            return call.Arguments.Get<T>(argumentName);
        }

        /// <summary>
        /// Writes the calls in the collection to the specified output writer.
        /// </summary>
        /// <typeparam name="T">The type of the calls.</typeparam>
        /// <param name="calls">The calls to write.</param>
        /// <param name="writer">The writer to write the calls to.</param>
        public static void Write<T>(this IEnumerable<T> calls, IOutputWriter writer) where T : IFakeObjectCall
        {
            Guard.AgainstNull(calls, "calls");
            Guard.AgainstNull(writer, "writer");

            var callWriter = ServiceLocator.Current.Resolve<CallWriter>();
            callWriter.WriteCalls(calls.Cast<IFakeObjectCall>(), writer);
        }

        /// <summary>
        /// Writes all calls in the collection to the console.
        /// </summary>
        /// <typeparam name="T">The type of the calls.</typeparam>
        /// <param name="calls">The calls to write.</param>
        public static void WriteToConsole<T>(this IEnumerable<T> calls) where T : IFakeObjectCall
        {
            calls.Write(new DefaultOutputWriter(Console.Write));
        }

        /// <summary>
        /// Gets the description of a call to a fake object.
        /// </summary>
        /// <param name="fakeObjectCall">The call to describe.</param>
        /// <returns>A description of the call.</returns>
        internal static string GetDescription(this IFakeObjectCall fakeObjectCall)
        {
            var method = fakeObjectCall.Method;
            return "{0}.{1}({2})".FormatInvariant(method.DeclaringType.FullName, method.Name, GetParametersString(fakeObjectCall));
        }

        private static string GetParametersString(IFakeObjectCall fakeObjectCall)
        {
            return fakeObjectCall.Arguments.ToCollectionString(x => GetArgumentAsString(x), ", ");
        }

        private static string GetArgumentAsString(object argument)
        {
            if (argument == null)
            {
                return "<NULL>";
            }

            var stringArgument = argument as string;
            if (stringArgument != null)
            {
                return stringArgument.Length > 0 ? string.Concat("\"", stringArgument, "\"") : "<string.Empty>";
            }

            return argument.ToString();
        }
    }
}