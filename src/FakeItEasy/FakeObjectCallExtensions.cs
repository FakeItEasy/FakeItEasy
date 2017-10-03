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
            Guard.AgainstNull(call, nameof(call));

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
            Guard.AgainstNull(call, nameof(call));
            Guard.AgainstNull(argumentName, nameof(argumentName));

            return call.Arguments.Get<T>(argumentName);
        }

        /// <summary>
        /// Gets the description of a call to a fake object.
        /// </summary>
        /// <param name="fakeObjectCall">The call to describe.</param>
        /// <returns>A description of the call.</returns>
        internal static string GetDescription(this IFakeObjectCall fakeObjectCall)
        {
            var method = fakeObjectCall.Method;
            return $"{method.DeclaringType}.{method.Name}({GetParametersString(fakeObjectCall)})";
        }

        private static string GetParametersString(IFakeObjectCall fakeObjectCall)
        {
            var writer = ServiceLocator.Current.Resolve<StringBuilderOutputWriter>();
            writer.WriteArgumentValues(fakeObjectCall.Arguments);
            return writer.Builder.ToString();
        }
    }
}
