using FakeItEasy.Expressions;
using System;
namespace FakeItEasy
{
    /// <summary>
    /// Provides validation extension to the ArgumentValidations{T} class.
    /// </summary>
    public static class ArgumentValidationsExtensions
    {
        /// <summary>
        /// Validates that an argument is null.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="validations">The scope of the validator.</param>
        /// <returns>An argument validator.</returns>
        public static ArgumentValidator<T> IsNull<T>(this ArgumentValidatorScope<T> scope) where T : class
        {
            return scope.CreateValidator(x => x == null, "NULL");
        }

        /// <summary>
        /// Validates that the string argument contains the specified text.
        /// </summary>
        /// <param name="validations">The scope of the validator.</param>
        /// <param name="value">The string the argument string should contain.</param>
        /// <returns>An argument validator.</returns>
        public static ArgumentValidator<string> Contains(this ArgumentValidatorScope<string> scope, string value)
        {
            return scope.CreateValidator(x => x != null && x.Contains(value), "String that contains \"{0}\"", value);
        }

        /// <summary>
        /// Validates that the string argument starts with the specified text.
        /// </summary>
        /// <param name="validations">The scope of the validator.</param>
        /// <param name="value">The string the argument string should start with.</param>
        /// <returns>An argument validator.</returns>
        public static ArgumentValidator<string> StartsWith(this ArgumentValidatorScope<string> scope, string value)
        {
            return scope.CreateValidator(x => x != null && x.StartsWith(value), "String that starts with \"{0}\"", value);
        }

        /// <summary>
        /// Validates that the string argument is null or the empty string.
        /// </summary>
        /// <param name="validations">The scope of the validator.</param>
        /// <returns>An argument validator.</returns>
        public static ArgumentValidator<string> IsNullOrEmpty(this ArgumentValidatorScope<string> scope)
        {
            return scope.CreateValidator(x => string.IsNullOrEmpty(x), "(NULL or string.Empty)");
        }

        /// <summary>
        /// Validates that the argument is greater than the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="scope">The scope of the validator.</param>
        /// <param name="value">The value that the argument has to be greatere than.</param>
        /// <returns>An argument validator.</returns>
        public static ArgumentValidator<T> IsGreaterThan<T>(this ArgumentValidatorScope<T> scope, T value) where T : IComparable
        {
            return scope.CreateValidator(x => x.CompareTo(value) > 0, "Greater than {0}", value);
        }

        private static ArgumentValidator<T> CreateValidator<T>(this ArgumentValidatorScope<T> scope, Func<T, bool> predicate, string description)
        {
            return ArgumentValidator.Create(scope, predicate, description);
        }
        
        private static ArgumentValidator<T> CreateValidator<T>(this ArgumentValidatorScope<T> scope, Func<T, bool> predicate, string descriptionFormat, params object[] args)
        {
            return ArgumentValidator.Create(scope, predicate, descriptionFormat.FormatInvariant(args));
        }
    }
}
