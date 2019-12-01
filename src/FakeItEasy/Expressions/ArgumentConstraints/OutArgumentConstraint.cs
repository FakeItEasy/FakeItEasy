namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using FakeItEasy.Core;

    internal class OutArgumentConstraint : IArgumentConstraint, IArgumentValueProvider
    {
        public OutArgumentConstraint(object? value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value that was used when specifying the constraint.
        /// Used for implicit assignment of out parameter values, not for matching.
        /// Since the called method has no access to the incoming parameter value,
        /// there's no use in accepting or rejecting calls based on the
        /// incoming parameter value.
        /// </summary>
        public object? Value { get; }

        public void WriteDescription(IOutputWriter writer)
        {
            writer.Write("<out parameter>");
        }

        public bool IsValid(object? argument)
        {
            return true;
        }
    }
}
