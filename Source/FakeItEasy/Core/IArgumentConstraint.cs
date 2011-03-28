namespace FakeItEasy.Core
{
    /// <summary>
    /// Validates an argument, checks that it's valid in a specific fake call.
    /// </summary>
    internal interface IArgumentConstraint
    {
        /// <summary>
        /// Gets a string describing the argument constraint.
        /// </summary>
        string ConstraintDescription { get; }

        /// <summary>
        /// Gets whether the argument is valid.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <returns>True if the argument is valid.</returns>
        bool IsValid(object argument);
    }

    /// <summary>
    /// Validates an argument, checks that it's valid in a specific fake call.
    /// </summary>
    internal interface IArgumentConstraint2
    {
        /// <summary>
        /// Writes a description of the arguemnt constraint to the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        void WriteDescription(IOutputWriter writer);

        /// <summary>
        /// Gets whether the argument is valid.
        /// </summary>
        /// <param name="argument">The argument to validate.</param>
        /// <returns>True if the argument is valid.</returns>
        bool IsValid(object argument);
    }
}