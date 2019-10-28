namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///   A collection of method arguments.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Best name to describe the type.")]
    public class ArgumentCollection
        : IEnumerable<object?>
    {
        /// <summary>
        ///   The arguments this collection contains.
        /// </summary>
        private readonly object?[] arguments;
        private readonly Lazy<string[]> argumentNames;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ArgumentCollection" /> class.
        /// </summary>
        /// <param name = "arguments">The arguments.</param>
        /// <param name = "method">The method.</param>
        [DebuggerStepThrough]
        internal ArgumentCollection(object?[] arguments, MethodInfo method)
        {
            Guard.AgainstNull(arguments, nameof(arguments));
            Guard.AgainstNull(method, nameof(method));

            if (arguments.Length != method.GetParameters().Length)
            {
                throw new ArgumentException(ExceptionMessages.WrongNumberOfArguments);
            }

            this.arguments = arguments;
            this.Method = method;
            this.argumentNames = new Lazy<string[]>(this.GetArgumentNames);
        }

        /// <summary>
        ///   Gets the number of arguments in the list.
        /// </summary>
        public int Count
        {
            [DebuggerStepThrough]
            get => this.arguments.Length;
        }

        /// <summary>
        ///   Gets the names of the arguments in the list.
        /// </summary>
        public IEnumerable<string> ArgumentNames => this.argumentNames.Value;

        internal MethodInfo Method { get; }

        /// <summary>
        ///   Gets the argument at the specified index.
        /// </summary>
        /// <param name = "argumentIndex">The index of the argument to get.</param>
        /// <returns>The argument at the specified index.</returns>
        public object? this[int argumentIndex]
        {
            [DebuggerStepThrough]
            get { return this.arguments[argumentIndex]; }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection of argument values.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<object?> GetEnumerator()
        {
            return ((IEnumerable<object?>)this.arguments).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        ///   Gets the argument at the specified index.
        /// </summary>
        /// <typeparam name = "T">The type of the argument to get.</typeparam>
        /// <param name = "index">The index of the argument.</param>
        /// <returns>
        /// The argument at the specified index. Note that the value is taken from method's arguments and so may be <c>null</c>,
        /// even if <typeparamref name="T"/> is non-nullable.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to cast the argument to the specified type.")]
        [return: MaybeNull]
        public T Get<T>(int index)
        {
            return (T)this.arguments[index] !;
        }

        /// <summary>
        ///   Gets the argument with the specified name.
        /// </summary>
        /// <typeparam name = "T">The type of the argument to get.</typeparam>
        /// <param name = "argumentName">The name of the argument.</param>
        /// <returns>
        /// The argument with the specified name. Note that the value is taken from method's arguments and so may be <c>null</c>,
        /// even if <typeparamref name="T"/> is non-nullable.
        /// </returns>
        [return: MaybeNull]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to cast the argument to the specified type.")]
        public T Get<T>(string argumentName)
        {
            Guard.AgainstNull(argumentName, nameof(argumentName));

            var index = this.GetArgumentIndex(argumentName);
            return this.Get<T>(index);
        }

        internal object?[] GetUnderlyingArgumentsArray()
        {
            return this.arguments;
        }

        private string[] GetArgumentNames() => this.Method.GetParameters().Select(x => x.Name).ToArray();

        private int GetArgumentIndex(string argumentName)
        {
            var names = this.argumentNames.Value;
            for (int index = 0; index < names.Length; ++index)
            {
                if (names[index].Equals(argumentName, StringComparison.Ordinal))
                {
                    return index;
                }
            }

            throw new ArgumentException(ExceptionMessages.ArgumentNameDoesNotExist, nameof(argumentName));
        }
    }
}
