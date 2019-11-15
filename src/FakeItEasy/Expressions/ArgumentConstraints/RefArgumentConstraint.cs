namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using FakeItEasy.Core;

    internal class RefArgumentConstraint : IArgumentConstraint, IArgumentValueProvider
    {
        private readonly IArgumentConstraint baseConstraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefArgumentConstraint" /> class by
        /// wrapping an existing <see cref="IArgumentConstraint"/>.
        /// </summary>
        /// <param name="baseConstraint">The original constraint, which will be used for argument validation.</param>
        /// <param name="value">The value to be used when implicitly assigning values to a call's ref parameter.</param>
        public RefArgumentConstraint(IArgumentConstraint baseConstraint, object value)
        {
            Guard.AgainstNull(baseConstraint, nameof(baseConstraint));

            this.baseConstraint = baseConstraint;
            this.Value = value;
        }

        /// <summary>
        /// Gets the value that was used when specifying the constraint.
        /// Used for implicit assignment of ref parameter values.
        /// </summary>
        public object Value { get; }

        public void WriteDescription(IOutputWriter writer)
        {
            this.baseConstraint.WriteDescription(writer);
        }

        public bool IsValid(object? argument)
        {
            return this.baseConstraint.IsValid(argument);
        }
    }
}
