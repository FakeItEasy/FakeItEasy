namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using System.Collections;
    using System.Linq;

    internal class EnumerableContainsConstraint<T>
        : ArgumentConstraint<T> where T : IEnumerable
    {
        private object expectedValue;

        public EnumerableContainsConstraint(ArgumentConstraintScope<T> scope, object value)
            : base(scope)
        {
            this.expectedValue = value;
        }

        protected override string Description
        {
            get 
            {
                return "sequence that contains the value {0}".FormatInvariant(this.expectedValue);
            }
        }

        protected override bool Evaluate(T value)
        {
            return value != null && value.Cast<object>().Contains(this.expectedValue);
        }
    }
}
