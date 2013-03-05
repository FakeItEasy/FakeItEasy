namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using FakeItEasy.Core;

    internal class EqualityArgumentConstraint
        : IArgumentConstraint
    {
        public EqualityArgumentConstraint(object expectedValue)
        {
            this.ExpectedValue = expectedValue;
        }

        public object ExpectedValue { get; private set; }

        public string ConstraintDescription
        {
            get { return this.ToString(); }
        }

        public bool IsValid(object argument)
        {
            return object.Equals(this.ExpectedValue, argument);
        }

        public override string ToString()
        {
            if (this.ExpectedValue == null)
            {
                return "<NULL>";
            }

            var stringValue = this.ExpectedValue as string;
            if (stringValue != null)
            {
                return "\"{0}\"".FormatInvariant(stringValue);
            }

            return this.ExpectedValue.ToString();
        }

        public void WriteDescription(IOutputWriter writer)
        {
            writer.Write(this.ConstraintDescription);
        }
    }

    ////internal class EqualityArgumentConstraint<T>
    ////    : ArgumentConstraint<T>
    ////{
    ////    private readonly T expectedValue;

    ////    public EqualityArgumentConstraint(ArgumentConstraintScope<T> scope, T expectedValue)
    ////        : base(scope)
    ////    {
    ////        this.expectedValue = expectedValue;
    ////    }

    ////    protected override string Description
    ////    {
    ////        get { return this.expectedValue != null ? this.expectedValue.ToString() : "NULL"; }
    ////    }

    ////    protected override bool Evaluate(T value)
    ////    {
    ////        return Equals(this.expectedValue, value);
    ////    }
    ////}
}