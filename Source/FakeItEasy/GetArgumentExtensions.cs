namespace FakeItEasy
{
    using System.Diagnostics.CodeAnalysis;

    using FakeItEasy.Core;

    /// <summary>
    /// Provides the GetArgument extension methods for getting arguments from fake object calls.
    /// </summary>
    public static class GetArgumentExtensions
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
    }
}