namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using FakeItEasy.Core;

    internal class RefArgumentConstraint : IArgumentConstraint, IArgumentValueProvider
    {
        private readonly EqualityArgumentConstraint baseConstraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefArgumentConstraint" /> class by
        /// wrapping an existing <see cref="IArgumentConstraint"/>. <see cref="Value"/> will be provided
        /// by the <paramref name="baseConstraint"/>'s <see cref="EqualityArgumentConstraint.ExpectedValue"/>
        /// </summary>
        /// <param name="baseConstraint">The original constraint, which will be used for argument validation.</param>
        public RefArgumentConstraint(EqualityArgumentConstraint baseConstraint)
        {
            Guard.AgainstNull(baseConstraint, "baseConstraint");

            this.baseConstraint = baseConstraint;
        }

        /// <summary>
        /// Gets the value that was used when specifying the constraint.
        /// Used for implicit assignment of out parameter values, not for matching.
        /// Since the called method has no access to the incoming parameter value,
        /// there's no use in accepting or rejecting calls based on the 
        /// incoming parameter value.
        /// </summary>
        public object Value
        {
            get { return this.baseConstraint.ExpectedValue; }
        }

        public void WriteDescription(IOutputWriter writer)
        {
            this.baseConstraint.WriteDescription(writer);
        }

        public bool IsValid(object argument)
        {
            return this.baseConstraint.IsValid(argument);
        }
    }
}