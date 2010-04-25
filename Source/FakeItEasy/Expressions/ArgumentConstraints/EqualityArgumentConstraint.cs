namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using FakeItEasy.Core;

    internal class EqualityArgumentConstraint
        : IArgumentConstraint
    {
        private object expectedValue;

        public EqualityArgumentConstraint(object expectedValue)
        {
            this.expectedValue = expectedValue;
        }

        public bool IsValid(object argument)
        {
            return object.Equals(this.expectedValue, argument);
        }

        public override string ToString()
        {
            if (this.expectedValue == null)
            {
                return "<NULL>";
            }

            var stringValue = this.expectedValue as string;
            if (stringValue != null)
            {
                return "\"{0}\"".FormatInvariant(stringValue);
            }

            return this.expectedValue.ToString();
        }
    }

    internal class EqualityArgumentConstraint<T>
        : ArgumentConstraint<T>
    {
        private T expectedValue;

        public EqualityArgumentConstraint(ArgumentConstraintScope<T> scope, T expectedValue)
            : base(scope)
        {
            this.expectedValue = expectedValue;    
        }

        protected override string Description
        {
            get 
            {

                return this.expectedValue != null ? this.expectedValue.ToString() : "NULL";
            }
        }

        protected override bool Evaluate(T value)
        {
            return object.Equals(this.expectedValue, value);
        }
    }


}