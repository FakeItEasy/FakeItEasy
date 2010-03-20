namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using FakeItEasy.Core;
    using System;
    using System.Collections;
    using System.Linq;

    public class EnumerableContainsConstraint<T>
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
                return "contains {0}".FormatInvariant(this.expectedValue);
            }
        }

        protected override bool Evaluate(T value)
        {
            return value != null && value.Cast<object>().Contains(this.expectedValue);
        }
    }
}
