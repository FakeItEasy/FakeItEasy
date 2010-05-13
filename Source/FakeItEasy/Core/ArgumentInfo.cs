namespace FakeItEasy.Core
{
    using System;

    /// <summary>
    /// Represents an argument and a dummy value to use for that argument.
    /// </summary>
    public class ArgumentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentInfo"/> class.
        /// </summary>
        /// <param name="wasSuccessfullyResolved">A value indicating if the dummy value was successfully resolved.</param>
        /// <param name="typeOfArgument">The type of argument.</param>
        /// <param name="resolvedValue">The resolved value.</param>
        public ArgumentInfo(bool wasSuccessfullyResolved, Type typeOfArgument, object resolvedValue)
        {
            this.WasSuccessfullyResolved = wasSuccessfullyResolved;
            this.TypeOfArgument = typeOfArgument;
            this.ResolvedValue = resolvedValue;
        }

        /// <summary>
        /// Gets a value indicating if a dummy argument value was successfully
        /// resolved.
        /// </summary>
        public bool WasSuccessfullyResolved { get; private set; }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        public Type TypeOfArgument { get; private set; }

        /// <summary>
        /// Gets the resolved value.
        /// </summary>
        public object ResolvedValue { get; private set; }
    }
}
