namespace FakeItEasy.Expressions.ArgumentConstraints
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Core;

    internal class EqualityArgumentConstraint
        : IArgumentConstraint
    {
        public EqualityArgumentConstraint(object expectedValue)
        {
            this.ExpectedValue = expectedValue;
        }

        public object ExpectedValue { get; }

        public string ConstraintDescription => this.ToString();

        public bool IsValid(object argument)
        {
            return object.Equals(this.ExpectedValue, argument);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any type of exception may be encountered.")]
        public override string ToString()
        {
            if (this.ExpectedValue == null)
            {
                return "<NULL>";
            }

            var stringValue = this.ExpectedValue as string;
            if (stringValue != null)
            {
                return $@"""{stringValue}""";
            }

            try
            {
                return this.ExpectedValue.ToString();
            }
            catch (Exception)
            {
                FakeManager manager = Fake.TryGetFakeManager(this.ExpectedValue);
                return manager != null
                    ? "Faked " + manager.FakeObjectType
                    : this.ExpectedValue.GetType().ToString();
            }
        }

        public void WriteDescription(IOutputWriter writer)
        {
            writer.Write(this.ConstraintDescription);
        }
    }
}
